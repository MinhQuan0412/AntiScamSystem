import { useState, useRef } from 'react'
import { useSearchParams } from 'react-router-dom'
import { Search, Upload, MessageSquare, AlertTriangle, CheckCircle, Loader2, Image, X } from 'lucide-react'
import { classifyApi } from '../api/endpoints'
import toast from 'react-hot-toast'
import clsx from 'clsx'

type Mode = 'text' | 'image' | 'chat'

interface ChatMsg { sender: 'them' | 'me'; text: string }

export default function ClassifyPage() {
  const [searchParams] = useSearchParams()
  const [mode, setMode]       = useState<Mode>('text')
  const [text, setText]       = useState(searchParams.get('q') || '')
  const [image, setImage]     = useState<File | null>(null)
  const [preview, setPreview] = useState<string | null>(null)
  const [chatMsgs, setChatMsgs] = useState<ChatMsg[]>([{ sender: 'them', text: '' }])
  const [consent, setConsent] = useState(false)
  const [loading, setLoading] = useState(false)
  const [result, setResult]   = useState<any>(null)
  // OCR fallback: khi trích xuất thất bại, cho user nhập tay
  const [ocrFallbackText, setOcrFallbackText] = useState<string | null>(null)
  const fileRef = useRef<HTMLInputElement>(null)

  async function handleSubmit() {
    setLoading(true); setResult(null); setOcrFallbackText(null)
    try {
      let res: any
      if (mode === 'text') {
        if (!text.trim()) { toast.error('Vui lòng nhập nội dung cần kiểm tra'); setLoading(false); return }
        res = await classifyApi.classifyText(text, consent)
        setResult({ type: 'text', data: res.data.value ?? res.data })
      } else if (mode === 'image') {
        if (!image) { toast.error('Vui lòng chọn ảnh'); setLoading(false); return }
        res = await classifyApi.classifyImage(image, consent)
        const imgData = res.data
        // Nếu OCR thất bại (không trích xuất được text), mở fallback textarea
        if (!imgData.hasMeaningfulText && !imgData.classificationResult) {
          setOcrFallbackText('')
          toast('Không đọc được chữ từ ảnh. Vui lòng nhập nội dung bên dưới để phân tích.', { icon: '📝', duration: 4000 })
        } else {
          setResult({ type: 'image', data: imgData })
        }
      } else {
        const msgs = chatMsgs.filter(m => m.text.trim())
        if (!msgs.length) { toast.error('Vui lòng nhập ít nhất 1 tin nhắn'); setLoading(false); return }
        res = await classifyApi.classifyChat(msgs, consent)
        setResult({ type: 'chat', data: res.data.value ?? res.data })
      }
    } catch (e: any) {
      toast.error(e?.response?.data?.errors?.[0] ?? 'Lỗi kết nối server')
    } finally { setLoading(false) }
  }

  // Phân tích text nhập tay từ fallback khi OCR thất bại
  async function handleOcrFallbackAnalyze() {
    if (!ocrFallbackText?.trim()) { toast.error('Vui lòng nhập nội dung từ ảnh'); return }
    setLoading(true)
    try {
      const res = await classifyApi.classifyText(ocrFallbackText, consent)
      setResult({ type: 'text', data: res.data.value ?? res.data })
      setOcrFallbackText(null)
    } catch (e: any) {
      toast.error(e?.response?.data?.errors?.[0] ?? 'Lỗi kết nối server')
    } finally { setLoading(false) }
  }

  function onFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const f = e.target.files?.[0]
    if (!f) return
    setImage(f)
    setPreview(URL.createObjectURL(f))
  }

  function addChatMsg() { setChatMsgs(p => [...p, { sender: 'them', text: '' }]) }
  function updateMsg(i: number, field: keyof ChatMsg, val: string) {
    setChatMsgs(p => p.map((m, idx) => idx === i ? { ...m, [field]: val } : m))
  }

  const tabs: { key: Mode; label: string; icon: React.ReactNode }[] = [
    { key: 'text',  label: 'Văn bản',    icon: <Search className="w-4 h-4" /> },
    { key: 'image', label: 'Hình ảnh',   icon: <Image  className="w-4 h-4" /> },
    { key: 'chat',  label: 'Đoạn chat',  icon: <MessageSquare className="w-4 h-4" /> },
  ]

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Kiểm tra lừa đảo</h1>
        <p className="text-gray-500 text-sm mt-1">Kiểm tra tin nhắn, hình ảnh hoặc đoạn chat nghi ngờ</p>
      </div>

      {/* Mode tabs */}
      <div className="flex gap-1 bg-gray-100 p-1 rounded-xl">
        {tabs.map(t => (
          <button key={t.key} onClick={() => { setMode(t.key); setResult(null) }}
            className={clsx('flex-1 flex items-center justify-center gap-1.5 py-2 px-3 rounded-lg text-sm font-medium transition-all',
              mode === t.key ? 'bg-white shadow text-red-600' : 'text-gray-500 hover:text-gray-700')}>
            {t.icon} {t.label}
          </button>
        ))}
      </div>

      {/* Input area */}
      <div className="card space-y-4">
        {mode === 'text' && (
          <textarea value={text} onChange={e => setText(e.target.value)}
            className="input resize-none" rows={6}
            placeholder="Dán nội dung tin nhắn, tình huống nghi ngờ vào đây..." />
        )}

        {mode === 'image' && (
          <div>
            <input ref={fileRef} type="file" accept="image/*" className="hidden" onChange={onFileChange} />
            {preview ? (
              <div className="relative">
                <img src={preview} alt="preview" className="rounded-lg max-h-72 w-full object-contain bg-gray-50 border" />
                <button onClick={() => { setImage(null); setPreview(null) }}
                  className="absolute top-2 right-2 bg-white rounded-full p-1 shadow">
                  <X className="w-4 h-4" />
                </button>
              </div>
            ) : (
              <button onClick={() => fileRef.current?.click()}
                className="w-full border-2 border-dashed border-gray-300 hover:border-red-400 rounded-xl p-10 text-center transition-colors">
                <Upload className="w-8 h-8 text-gray-400 mx-auto mb-2" />
                <p className="text-sm text-gray-500">Nhấn để chọn ảnh chụp màn hình tin nhắn</p>
                <p className="text-xs text-gray-400 mt-1">JPG, PNG, WebP — tối đa 10MB</p>
              </button>
            )}
          </div>
        )}

        {mode === 'chat' && (
          <div className="space-y-3">
            <p className="text-xs text-gray-500">Nhập từng tin nhắn, chọn người gửi (Họ / Tôi)</p>
            {chatMsgs.map((msg, i) => (
              <div key={i} className="flex gap-2 items-start">
                <select value={msg.sender} onChange={e => updateMsg(i, 'sender', e.target.value as any)}
                  className="input w-24 shrink-0 text-xs py-2.5">
                  <option value="them">Họ</option>
                  <option value="me">Tôi</option>
                </select>
                <input value={msg.text} onChange={e => updateMsg(i, 'text', e.target.value)}
                  className="input flex-1 text-sm" placeholder="Nội dung tin nhắn..." />
                {chatMsgs.length > 1 && (
                  <button onClick={() => setChatMsgs(p => p.filter((_, j) => j !== i))}
                    className="p-2.5 text-gray-400 hover:text-red-500">
                    <X className="w-4 h-4" />
                  </button>
                )}
              </div>
            ))}
            <button onClick={addChatMsg} className="text-sm text-red-600 hover:underline">+ Thêm tin nhắn</button>
          </div>
        )}

        <label className="flex items-center gap-2 text-sm text-gray-600 cursor-pointer select-none">
          <input type="checkbox" checked={consent} onChange={e => setConsent(e.target.checked)}
            className="rounded border-gray-300 text-red-600" />
          Đồng ý đóng góp nội dung (đã ẩn danh) để cải thiện mô hình
        </label>

        <button onClick={handleSubmit} disabled={loading} className="btn-primary w-full justify-center py-3">
          {loading ? <><Loader2 className="w-4 h-4 animate-spin" /> Đang phân tích...</>
                   : <><Search className="w-4 h-4" /> Kiểm tra ngay</>}
        </button>
      </div>

      {/* OCR Fallback: Nhập tay khi ảnh không đọc được */}
      {ocrFallbackText !== null && (
        <div className="card border-2 border-yellow-300 bg-yellow-50 space-y-3">
          <div className="flex items-center gap-2">
            <span className="text-2xl">📝</span>
            <div>
              <p className="font-semibold text-yellow-800 text-sm">Không đọc được chữ từ ảnh</p>
              <p className="text-xs text-yellow-700">Hãy gõ hoặc dán nội dung từ ảnh vào bên dưới để phân tích</p>
            </div>
          </div>
          <textarea
            value={ocrFallbackText}
            onChange={e => setOcrFallbackText(e.target.value)}
            className="input resize-none w-full" rows={5}
            placeholder="Dán hoặc gõ nội dung từ ảnh vào đây (ví dụ: nội dung tin nhắn lừa đảo)..."
            autoFocus
          />
          <div className="flex gap-2">
            <button
              onClick={handleOcrFallbackAnalyze}
              disabled={loading || !ocrFallbackText?.trim()}
              className="btn-primary flex-1 justify-center py-2.5"
            >
              {loading ? <><Loader2 className="w-4 h-4 animate-spin" /> Đang phân tích...</> : <><Search className="w-4 h-4" /> Phân tích nội dung này</>}
            </button>
            <button onClick={() => setOcrFallbackText(null)} className="px-4 py-2.5 rounded-lg border border-gray-300 text-sm text-gray-600 hover:bg-gray-100">
              Huỷ
            </button>
          </div>
        </div>
      )}

      {/* Result */}
      {result && <ClassifyResult result={result} />}
    </div>
  )
}

function ClassifyResult({ result }: { result: any }) {
  const { type, data } = result

  if (type === 'image') {
    // Nếu vẫn còn trường hợp không có text (phòng ngừa)
    if (!data.hasMeaningfulText && !data.classificationResult) {
      return null
    }
    return (
      <div className="space-y-3">
        {data.extractedText && (
          <div className="card bg-gray-50">
            <p className="text-xs text-gray-500 font-medium mb-1">Text trích xuất từ ảnh (OCR):</p>
            <p className="text-sm text-gray-700 whitespace-pre-wrap">{data.extractedText}</p>
          </div>
        )}
        {data.classificationResult && <TextResult data={data.classificationResult} />}
      </div>
    )
  }

  if (type === 'chat') {
    return <ChatResult data={data} />
  }

  return <TextResult data={data} />
}

function TextResult({ data }: { data: any }) {
  const isScam = data.isScam
  return (
    <div className={clsx('card border-2', isScam ? 'border-red-300 bg-red-50' : 'border-green-300 bg-green-50')}>
      <div className="flex items-center gap-3 mb-4">
        {isScam
          ? <AlertTriangle className="w-8 h-8 text-red-500 shrink-0" />
          : <CheckCircle   className="w-8 h-8 text-green-500 shrink-0" />}
        <div>
          <p className={clsx('text-lg font-bold', isScam ? 'text-red-700' : 'text-green-700')}>
            {isScam ? '⚠️ Có dấu hiệu lừa đảo' : '✅ Không phát hiện dấu hiệu lừa đảo'}
          </p>
          {data.scamTypeName && <p className="text-sm text-gray-600">Loại: <strong>{data.scamTypeName}</strong></p>}
          <p className="text-sm text-gray-500">Độ tin cậy: {Math.round(data.confidence * 100)}%</p>
        </div>
      </div>

      {data.matchedSignals?.length > 0 && (
        <div className="mb-3">
          <p className="text-sm font-semibold text-gray-700 mb-1">Dấu hiệu phát hiện:</p>
          <div className="flex flex-wrap gap-1.5">
            {data.matchedSignals.map((s: string, i: number) => (
              <span key={i} className="badge-scam">{s}</span>
            ))}
          </div>
        </div>
      )}

      {data.preventionAdvice && (
        <div className="bg-white rounded-lg p-3 border border-gray-200">
          <p className="text-sm font-semibold text-gray-700 mb-1">Khuyến nghị:</p>
          <p className="text-sm text-gray-600">{data.preventionAdvice}</p>
        </div>
      )}

      <p className="text-xs text-gray-400 mt-3">{data.disclaimer}</p>
    </div>
  )
}

function ChatResult({ data }: { data: any }) {
  const riskColorMap: Record<string, string> = {
    'Rất cao': 'border-red-400 bg-red-50',
    'Cao':     'border-orange-400 bg-orange-50',
    'Trung bình': 'border-yellow-400 bg-yellow-50',
    'Thấp':    'border-green-400 bg-green-50',
  }
  const riskColor = riskColorMap[data.riskLevel as string] ?? 'border-gray-200'

  return (
    <div className={clsx('card border-2 space-y-4', riskColor)}>
      <div className="flex items-center justify-between">
        <div>
          <p className="font-bold text-gray-800 text-lg">
            {data.isScam ? '⚠️ Phát hiện dấu hiệu lừa đảo' : '✅ Không phát hiện dấu hiệu nguy hiểm'}
          </p>
          <p className="text-sm text-gray-500">
            Mức rủi ro: <strong>{data.riskLevel}</strong> · {data.suspiciousMessageCount}/{data.totalMessageCount} tin nhắn đáng ngờ
          </p>
        </div>
        <div className="text-2xl font-bold text-gray-700">{Math.round(data.overallRiskScore * 100)}%</div>
      </div>

      {data.detectedTactics?.length > 0 && (
        <div>
          <p className="text-sm font-semibold text-gray-700 mb-2">Thủ đoạn phát hiện ({data.detectedTactics.length}):</p>
          <div className="space-y-2">
            {data.detectedTactics.map((t: any, i: number) => (
              <div key={i} className="bg-white rounded-lg p-3 border border-gray-200">
                <p className="text-sm font-semibold text-red-700">🔴 {t.name}</p>
                <p className="text-xs text-gray-600 mt-0.5">{t.description}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      <div>
        <p className="text-sm font-semibold text-gray-700 mb-2">Phân tích từng tin nhắn:</p>
        <div className="space-y-2 max-h-64 overflow-y-auto">
          {data.messageAnalyses?.map((m: any, i: number) => (
            <div key={i} className={clsx('rounded-lg p-2.5 text-sm',
              m.sender === 'me' ? 'bg-blue-50 ml-6' : m.isSuspicious ? 'bg-red-100' : 'bg-gray-100 mr-6')}>
              <span className={clsx('text-xs font-medium mr-2',
                m.sender === 'me' ? 'text-blue-600' : m.isSuspicious ? 'text-red-600' : 'text-gray-500')}>
                {m.sender === 'me' ? 'Tôi' : m.isSuspicious ? '⚠️ Họ' : 'Họ'}
              </span>
              {m.text}
              {m.detectedSignals?.length > 0 && (
                <div className="flex flex-wrap gap-1 mt-1">
                  {m.detectedSignals.map((s: string, j: number) => (
                    <span key={j} className="badge-scam text-xs">{s}</span>
                  ))}
                </div>
              )}
            </div>
          ))}
        </div>
      </div>

      {data.preventionAdvice && (
        <div className="bg-white rounded-lg p-3 border border-gray-200">
          <p className="text-sm font-semibold text-gray-700 mb-1">Khuyến nghị:</p>
          <p className="text-sm text-gray-600">{data.preventionAdvice}</p>
        </div>
      )}
    </div>
  )
}
