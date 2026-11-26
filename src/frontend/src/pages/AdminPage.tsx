import { useQuery } from '@tanstack/react-query'
import { statsApi, phoneApi } from '../api/endpoints'
import { BarChart2, Phone, Shield, TrendingUp, Loader2, CheckCircle, XCircle } from 'lucide-react'
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, Legend } from 'recharts'
import { isAdmin } from '../store/authStore'
import { Navigate } from 'react-router-dom'
import { useState } from 'react'
import toast from 'react-hot-toast'

const COLORS = ['#ef4444','#f97316','#eab308','#8b5cf6','#ec4899','#3b82f6','#10b981','#6366f1']

export default function AdminPage() {
  if (!isAdmin()) return <Navigate to="/login" replace />

  const { data: stats, isLoading } = useQuery({
    queryKey: ['stats'],
    queryFn: () => statsApi.get().then(r => r.data?.value ?? r.data),
  })

  const { data: pendingData } = useQuery({
    queryKey: ['pending-moderation'],
    queryFn: () => phoneApi.getPending().then(r => r.data?.items ?? r.data?.value?.items ?? []),
  })

  const pending: any[] = Array.isArray(pendingData) ? pendingData : []

  const pieData = Object.entries(stats?.totalByType ?? {}).map(([k, v]) => ({ name: k, value: v as number }))

  const barData = stats?.topReportedNumbers?.map((r: any) => ({
    name: r.reportedNumber?.slice(-4) ?? '****',
    score: Math.round(r.riskScore ?? 0),
    reports: r.reportCount ?? 0,
  })) ?? []

  async function downloadExcel() {
    try {
      const res = await statsApi.exportExcel()
      const url = URL.createObjectURL(new Blob([res.data]))
      const a = document.createElement('a'); a.href = url
      a.download = `BaoCaoLuaDao_${new Date().toISOString().slice(0,10)}.xlsx`
      a.click()
      toast.success('Đã tải Excel!')
    } catch { toast.error('Lỗi xuất Excel') }
  }

  async function downloadPdf() {
    try {
      const res = await statsApi.exportPdf()
      const url = URL.createObjectURL(new Blob([res.data], { type: 'application/pdf' }))
      const a = document.createElement('a'); a.href = url
      a.download = `BaoCaoLuaDao_${new Date().toISOString().slice(0,10)}.pdf`
      a.click()
      toast.success('Đã tải PDF!')
    } catch { toast.error('Lỗi xuất PDF') }
  }

  async function moderate(id: string, approve: boolean) {
    try {
      await phoneApi.moderate(id, approve, approve ? 'Đã xác minh' : 'Thông tin sai')
      toast.success(approve ? 'Đã duyệt báo cáo' : 'Đã từ chối báo cáo')
    } catch { toast.error('Lỗi kiểm duyệt') }
  }

  if (isLoading) return <div className="flex justify-center py-20"><Loader2 className="w-8 h-8 animate-spin text-red-500" /></div>

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-3">
        <h1 className="text-2xl font-bold text-gray-900">Admin Dashboard</h1>
        <div className="flex gap-2">
          <button onClick={downloadExcel} className="btn-secondary text-sm">📊 Excel</button>
          <button onClick={downloadPdf}   className="btn-secondary text-sm">📄 PDF</button>
        </div>
      </div>

      {/* Stats cards */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {[
          { label: 'Phát hiện lừa đảo', value: stats?.totalScamsDetected ?? 0, icon: Shield, color: 'text-red-600 bg-red-50' },
          { label: 'SĐT bị báo cáo',   value: stats?.topReportedNumbers?.length ?? 0, icon: Phone, color: 'text-orange-600 bg-orange-50' },
          { label: 'Chờ kiểm duyệt',   value: pending.length, icon: BarChart2, color: 'text-yellow-600 bg-yellow-50' },
          { label: 'Phiên bản mô hình', value: stats?.modelPerformanceSummary?.length ?? 0, icon: TrendingUp, color: 'text-blue-600 bg-blue-50' },
        ].map(({ label, value, icon: Icon, color }) => (
          <div key={label} className="card flex items-center gap-3">
            <div className={`p-2.5 rounded-xl ${color}`}><Icon className="w-5 h-5" /></div>
            <div>
              <p className="text-2xl font-bold text-gray-800">{value}</p>
              <p className="text-xs text-gray-500">{label}</p>
            </div>
          </div>
        ))}
      </div>

      {/* Charts */}
      <div className="grid md:grid-cols-2 gap-6">
        {pieData.length > 0 && (
          <div className="card">
            <h3 className="font-semibold text-gray-800 mb-4">Phân bố theo loại lừa đảo</h3>
            <ResponsiveContainer width="100%" height={220}>
              <PieChart>
                <Pie data={pieData} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={80}>
                  {pieData.map((_, i) => <Cell key={i} fill={COLORS[i % COLORS.length]} />)}
                </Pie>
                <Tooltip /><Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
        )}

        {barData.length > 0 && (
          <div className="card">
            <h3 className="font-semibold text-gray-800 mb-4">Số bị báo cáo nhiều nhất (điểm rủi ro)</h3>
            <ResponsiveContainer width="100%" height={220}>
              <BarChart data={barData}>
                <XAxis dataKey="name" /><YAxis />
                <Tooltip />
                <Bar dataKey="score" fill="#ef4444" radius={[4,4,0,0]} name="Điểm rủi ro" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        )}
      </div>

      {/* Moderation queue */}
      {pending.length > 0 && (
        <div className="card">
          <h3 className="font-semibold text-gray-800 mb-4">Hàng chờ kiểm duyệt ({pending.length})</h3>
          <div className="space-y-3">
            {pending.map((r: any) => (
              <div key={r.id} className="flex items-start justify-between gap-3 p-3 bg-gray-50 rounded-lg flex-wrap">
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-gray-800">Báo cáo #{r.id?.slice(-6)}</p>
                  <p className="text-xs text-gray-500 mt-0.5 line-clamp-2">{r.description || 'Không có mô tả'}</p>
                  <p className="text-xs text-gray-400 mt-1">Người báo: {r.reporterName} · {new Date(r.createdAt).toLocaleDateString('vi-VN')}</p>
                </div>
                <div className="flex gap-2 shrink-0">
                  <button onClick={() => moderate(r.id, true)}
                    className="flex items-center gap-1 text-xs text-green-700 bg-green-100 hover:bg-green-200 px-3 py-1.5 rounded-lg font-medium">
                    <CheckCircle className="w-3 h-3" /> Duyệt
                  </button>
                  <button onClick={() => moderate(r.id, false)}
                    className="flex items-center gap-1 text-xs text-red-700 bg-red-100 hover:bg-red-200 px-3 py-1.5 rounded-lg font-medium">
                    <XCircle className="w-3 h-3" /> Từ chối
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Model performance */}
      {stats?.modelPerformanceSummary?.length > 0 && (
        <div className="card">
          <h3 className="font-semibold text-gray-800 mb-4">Hiệu năng mô hình ML</h3>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead><tr className="text-xs text-gray-500 border-b">
                <th className="text-left pb-2">Phiên bản</th>
                <th className="text-right pb-2">Accuracy</th>
                <th className="text-right pb-2">Precision</th>
                <th className="text-right pb-2">Recall</th>
                <th className="text-right pb-2">F1</th>
                <th className="text-right pb-2">Active</th>
              </tr></thead>
              <tbody>
                {stats.modelPerformanceSummary.map((m: any) => (
                  <tr key={m.id} className="border-b border-gray-50 hover:bg-gray-50">
                    <td className="py-2 font-medium">{m.versionLabel || m.versionTag}</td>
                    <td className="text-right text-gray-600">{m.accuracy ? (m.accuracy * 100).toFixed(1) + '%' : '-'}</td>
                    <td className="text-right text-gray-600">{m.precision ? (m.precision * 100).toFixed(1) + '%' : '-'}</td>
                    <td className="text-right text-gray-600">{m.recall ? (m.recall * 100).toFixed(1) + '%' : '-'}</td>
                    <td className="text-right text-gray-600">{m.f1Score ? (m.f1Score * 100).toFixed(1) + '%' : '-'}</td>
                    <td className="text-right">{m.isActive ? '✅' : '—'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  )
}
