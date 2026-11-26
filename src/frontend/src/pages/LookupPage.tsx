import { useState } from 'react'
import { Search, AlertTriangle, CheckCircle, Loader2, Phone, CreditCard, Flag } from 'lucide-react'
import { phoneApi } from '../api/endpoints'
import { isLoggedIn } from '../store/authStore'
import { Link } from 'react-router-dom'
import toast from 'react-hot-toast'
import clsx from 'clsx'

export default function LookupPage() {
  const [number, setNumber]       = useState('')
  const [loading, setLoading]     = useState(false)
  const [result, setResult]       = useState<any>(null)
  const [showReport, setShowReport] = useState(false)
  const loggedIn = isLoggedIn()

  async function handleLookup(e: React.FormEvent) {
    e.preventDefault()
    if (!number.trim()) return
    setLoading(true); setResult(null)
    try {
      const res = await phoneApi.lookup(number.trim())
      setResult(res.data.value ?? res.data)
    } catch (e: any) {
      toast.error('Lỗi tra cứu: ' + (e?.response?.data?.message ?? 'Không thể kết nối server'))
    } finally { setLoading(false) }
  }

  const riskColor = (level: string) => ({
    Critical: 'text-red-700 bg-red-100',
    High:     'text-orange-700 bg-orange-100',
    Medium:   'text-yellow-700 bg-yellow-100',
    Low:      'text-green-700 bg-green-100',
  })[level] ?? 'text-gray-700 bg-gray-100'

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Tra cứu số điện thoại / Tài khoản</h1>
        <p className="text-gray-500 text-sm mt-1">Kiểm tra xem số có bị báo cáo lừa đảo không</p>
      </div>

      {/* Search */}
      <form onSubmit={handleLookup} className="card flex gap-2">
        <input value={number} onChange={e => setNumber(e.target.value)}
          className="input flex-1" placeholder="Nhập số điện thoại hoặc số tài khoản..." />
        <button type="submit" disabled={loading} className="btn-primary shrink-0">
          {loading ? <Loader2 className="w-4 h-4 animate-spin" /> : <Search className="w-4 h-4" />}
          Tra cứu
        </button>
      </form>

      {/* Result */}
      {result && (
        <div className="card space-y-4">
          {/* Header */}
          <div className="flex items-start justify-between gap-4">
            <div className="flex items-center gap-3">
              {result.numberType === 1
                ? <CreditCard className="w-8 h-8 text-gray-400 shrink-0" />
                : <Phone      className="w-8 h-8 text-gray-400 shrink-0" />}
              <div>
                <p className="text-xl font-bold text-gray-900 tracking-widest">{result.maskedNumber}</p>
                <p className="text-xs text-gray-500">
                  {result.numberType === 1 ? 'Số tài khoản ngân hàng' : 'Số điện thoại'}
                  {result.bankName && ` · ${result.bankName}`}
                </p>
              </div>
            </div>
            {result.riskScore && (
              <span className={clsx('px-3 py-1 rounded-full text-sm font-bold shrink-0',
                riskColor(result.riskScore.level))}>
                {result.riskScore.level === 'Low' ? '✅ An toàn' :
                 result.riskScore.level === 'Medium' ? '⚠️ Cảnh giác' :
                 result.riskScore.level === 'High' ? '🔴 Nguy hiểm' : '🚨 Rất nguy hiểm'}
              </span>
            )}
          </div>

          {/* Stats */}
          <div className="grid grid-cols-3 gap-3 text-center">
            <div className="bg-gray-50 rounded-lg p-3">
              <p className="text-2xl font-bold text-gray-800">{result.reportCount}</p>
              <p className="text-xs text-gray-500">Lượt báo cáo</p>
            </div>
            <div className="bg-gray-50 rounded-lg p-3">
              <p className="text-2xl font-bold text-gray-800">{result.uniqueReporterCount}</p>
              <p className="text-xs text-gray-500">Người báo cáo</p>
            </div>
            <div className="bg-gray-50 rounded-lg p-3">
              <p className="text-2xl font-bold text-gray-800">
                {result.riskScore ? Math.round(result.riskScore.score) : 0}
              </p>
              <p className="text-xs text-gray-500">Điểm rủi ro</p>
            </div>
          </div>

          {/* Risk explanation */}
          {result.riskScore?.explanation && (
            <div className={clsx('rounded-lg p-3 text-sm', riskColor(result.riskScore.level))}>
              {result.riskScore.explanation}
            </div>
          )}

          {/* Recent reports */}
          {result.recentReports?.length > 0 && (
            <div>
              <p className="text-sm font-semibold text-gray-700 mb-2">Báo cáo gần đây:</p>
              <div className="space-y-2">
                {result.recentReports.map((r: any, i: number) => (
                  <div key={i} className="bg-gray-50 rounded-lg p-3 text-sm">
                    <div className="flex items-center justify-between mb-1">
                      <span className="text-xs text-gray-500">{r.reporterName} · {new Date(r.createdAt).toLocaleDateString('vi-VN')}</span>
                      {r.scamType && <span className="badge-scam">{r.scamType}</span>}
                    </div>
                    <p className="text-gray-700">{r.description || 'Không có mô tả chi tiết'}</p>
                  </div>
                ))}
              </div>
            </div>
          )}

          {result.reportCount === 0 && (
            <div className="flex items-center gap-2 text-green-700 bg-green-50 rounded-lg p-3">
              <CheckCircle className="w-5 h-5 shrink-0" />
              <p className="text-sm">Chưa có ghi nhận báo cáo nào trong hệ thống. Vẫn cần cẩn thận với người lạ.</p>
            </div>
          )}

          {/* Report button */}
          <div className="pt-2 border-t border-gray-100">
            {loggedIn ? (
              <button onClick={() => setShowReport(!showReport)}
                className="btn-secondary text-sm w-full justify-center">
                <Flag className="w-4 h-4" /> Báo cáo số này
              </button>
            ) : (
              <p className="text-xs text-gray-400 text-center">
                <Link to="/login" className="text-red-600 hover:underline">Đăng nhập</Link> để báo cáo số điện thoại này
              </p>
            )}
          </div>

          {showReport && loggedIn && (
            <ReportForm number={number} onDone={() => setShowReport(false)} />
          )}
        </div>
      )}
    </div>
  )
}

function ReportForm({ number, onDone }: { number: string; onDone: () => void }) {
  const [desc, setDesc]     = useState('')
  const [scamType, setScamType] = useState('')
  const [loading, setLoading] = useState(false)

  async function submit(e: React.FormEvent) {
    e.preventDefault()
    setLoading(true)
    try {
      await phoneApi.report({ reportedNumber: number, numberType: 0, description: desc, scamType: scamType || undefined })
      toast.success('Báo cáo đã được gửi, cảm ơn bạn!')
      onDone()
    } catch (e: any) {
      toast.error(e?.response?.data?.errors?.[0] ?? 'Lỗi gửi báo cáo')
    } finally { setLoading(false) }
  }

  return (
    <form onSubmit={submit} className="space-y-3 bg-gray-50 rounded-lg p-4">
      <p className="text-sm font-semibold text-gray-700">Báo cáo số: {number}</p>
      <select value={scamType} onChange={e => setScamType(e.target.value)} className="input text-sm">
        <option value="">-- Chọn loại lừa đảo --</option>
        <option value="0">Giả danh ngân hàng</option>
        <option value="1">Việc nhẹ lương cao</option>
        <option value="2">Đầu tư tài chính</option>
        <option value="3">Giả danh cơ quan</option>
        <option value="4">Lừa đảo tình cảm</option>
        <option value="7">Mua bán online</option>
        <option value="99">Khác</option>
      </select>
      <textarea value={desc} onChange={e => setDesc(e.target.value)} required
        className="input resize-none" rows={3} placeholder="Mô tả ngắn về cách thức lừa đảo..." />
      <div className="flex gap-2">
        <button type="submit" disabled={loading} className="btn-primary text-sm">
          {loading ? <Loader2 className="w-3 h-3 animate-spin" /> : null} Gửi báo cáo
        </button>
        <button type="button" onClick={onDone} className="btn-secondary text-sm">Huỷ</button>
      </div>
    </form>
  )
}
