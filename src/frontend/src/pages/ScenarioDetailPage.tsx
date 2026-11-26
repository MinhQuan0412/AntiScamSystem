import { useParams, Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { ChevronLeft, AlertTriangle, Shield, Eye, BookOpen, Loader2 } from 'lucide-react'
import { scenarioApi } from '../api/endpoints'

export default function ScenarioDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data, isLoading } = useQuery({
    queryKey: ['scenario', id],
    queryFn: () => scenarioApi.getById(id!).then(r => r.data?.value ?? r.data),
    enabled: !!id,
  })

  const { data: variantsData } = useQuery({
    queryKey: ['scenario-variants', id],
    queryFn: () => scenarioApi.getById(id!).then(r => r.data),
    enabled: !!id,
  })

  if (isLoading) return (
    <div className="flex justify-center py-20"><Loader2 className="w-8 h-8 animate-spin text-red-500" /></div>
  )

  if (!data) return (
    <div className="text-center py-20 text-gray-400">Không tìm thấy kịch bản</div>
  )

  const signs: string[] = typeof data.recognitionSigns === 'string'
    ? data.recognitionSigns.split(';').map((s: string) => s.trim()).filter(Boolean)
    : data.recognitionSigns ?? []

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <Link to="/scenarios" className="flex items-center gap-1 text-sm text-gray-500 hover:text-red-600">
        <ChevronLeft className="w-4 h-4" /> Quay lại thư viện
      </Link>

      <div className="card space-y-5">
        {/* Header */}
        <div className="flex items-start gap-3">
          <div className="bg-red-100 p-2 rounded-lg shrink-0">
            <AlertTriangle className="w-6 h-6 text-red-600" />
          </div>
          <div>
            <span className="text-xs text-gray-500 uppercase tracking-wide">{data.scamTypeName}</span>
            <h1 className="text-xl font-bold text-gray-900 mt-0.5">{data.title}</h1>
            <span className={`text-xs px-2 py-0.5 rounded-full ${
              data.status === 0 ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-500'}`}>
              {data.status === 0 ? '● Đang phổ biến' : data.status === 1 ? 'Đã biến đổi' : 'Lưu trữ'}
            </span>
          </div>
        </div>

        {/* Description */}
        <div>
          <h3 className="font-semibold text-gray-800 flex items-center gap-2 mb-2">
            <BookOpen className="w-4 h-4 text-gray-500" /> Mô tả
          </h3>
          <p className="text-gray-600 text-sm leading-relaxed">{data.description}</p>
        </div>

        {/* Recognition signs */}
        {signs.length > 0 && (
          <div>
            <h3 className="font-semibold text-gray-800 flex items-center gap-2 mb-2">
              <Eye className="w-4 h-4 text-orange-500" /> Dấu hiệu nhận biết
            </h3>
            <ul className="space-y-1.5">
              {signs.map((s, i) => (
                <li key={i} className="flex items-start gap-2 text-sm text-gray-700">
                  <span className="text-orange-500 mt-0.5 shrink-0">⚠</span> {s}
                </li>
              ))}
            </ul>
          </div>
        )}

        {/* Example */}
        {data.anonymizedExample && (
          <div className="bg-gray-50 rounded-xl p-4 border-l-4 border-gray-300">
            <h3 className="text-sm font-semibold text-gray-700 mb-2">Ví dụ thực tế (đã ẩn danh)</h3>
            <p className="text-sm text-gray-600 italic">"{data.anonymizedExample}"</p>
          </div>
        )}

        {/* Prevention */}
        {data.preventionAdvice && (
          <div className="bg-green-50 rounded-xl p-4 border border-green-200">
            <h3 className="font-semibold text-green-800 flex items-center gap-2 mb-2">
              <Shield className="w-4 h-4" /> Cách phòng tránh
            </h3>
            <p className="text-sm text-green-700">{data.preventionAdvice}</p>
          </div>
        )}
      </div>

      {/* CTA */}
      <div className="card bg-red-50 border border-red-200 text-center">
        <p className="text-sm text-red-800 font-medium mb-3">Nhận được tin nhắn tương tự?</p>
        <Link to="/classify" className="btn-primary inline-flex">
          <AlertTriangle className="w-4 h-4" /> Kiểm tra ngay
        </Link>
      </div>
    </div>
  )
}
