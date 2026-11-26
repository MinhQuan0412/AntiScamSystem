import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Search, BookOpen, ChevronRight, AlertTriangle } from 'lucide-react'
import { scenarioApi } from '../api/endpoints'
import { Link } from 'react-router-dom'
import clsx from 'clsx'

const SCAM_TYPES = [
  { value: '', label: 'Tất cả' },
  { value: '0', label: 'Giả danh ngân hàng' },
  { value: '1', label: 'Việc nhẹ lương cao' },
  { value: '2', label: 'Đầu tư tài chính' },
  { value: '3', label: 'Giả danh cơ quan' },
  { value: '4', label: 'Lừa đảo tình cảm' },
  { value: '5', label: 'Vé du lịch giả' },
  { value: '6', label: 'Trúng thưởng' },
  { value: '7', label: 'Mua bán online' },
  { value: '99', label: 'Khác' },
]

const TYPE_COLORS: Record<string, string> = {
  '0': 'bg-red-100 text-red-700', '1': 'bg-orange-100 text-orange-700',
  '2': 'bg-yellow-100 text-yellow-700', '3': 'bg-purple-100 text-purple-700',
  '4': 'bg-pink-100 text-pink-700', '5': 'bg-blue-100 text-blue-700',
  '6': 'bg-green-100 text-green-700', '7': 'bg-indigo-100 text-indigo-700',
  '99': 'bg-gray-100 text-gray-700',
}

export default function ScenariosPage() {
  const [search, setSearch]   = useState('')
  const [type, setType]       = useState('')
  const [page, setPage]       = useState(1)

  const { data, isLoading } = useQuery({
    queryKey: ['scenarios', page, type],
    queryFn: () => scenarioApi.getAll(page, 9, type || undefined).then(r => r.data),
  })

  const { data: searchData } = useQuery({
    queryKey: ['scenarios-search', search],
    queryFn: () => scenarioApi.search(search).then(r => r.data),
    enabled: search.length >= 2,
  })

  const items = search.length >= 2
    ? (searchData?.value ?? searchData ?? [])
    : (data?.value?.items ?? data?.items ?? [])

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Thư viện kịch bản lừa đảo</h1>
        <p className="text-gray-500 text-sm mt-1">Tìm hiểu các chiêu trò và cách phòng tránh</p>
      </div>

      {/* Filters */}
      <div className="flex flex-col sm:flex-row gap-3">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input value={search} onChange={e => setSearch(e.target.value)}
            className="input pl-9" placeholder="Tìm kiếm kịch bản..." />
        </div>
        <select value={type} onChange={e => { setType(e.target.value); setPage(1) }}
          className="input sm:w-52">
          {SCAM_TYPES.map(t => <option key={t.value} value={t.value}>{t.label}</option>)}
        </select>
      </div>

      {/* Grid */}
      {isLoading ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {[...Array(6)].map((_, i) => (
            <div key={i} className="card animate-pulse space-y-3">
              <div className="h-4 bg-gray-200 rounded w-3/4" />
              <div className="h-3 bg-gray-200 rounded w-1/2" />
              <div className="h-12 bg-gray-200 rounded" />
            </div>
          ))}
        </div>
      ) : items.length === 0 ? (
        <div className="text-center py-16 text-gray-400">
          <BookOpen className="w-12 h-12 mx-auto mb-3 opacity-30" />
          <p>Chưa có kịch bản nào{search ? ` cho "${search}"` : ''}</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {items.map((s: any) => (
            <Link key={s.id} to={`/scenarios/${s.id}`}
              className="card hover:shadow-md transition-shadow flex flex-col gap-3 group">
              <div className="flex items-start justify-between gap-2">
                <span className={clsx('px-2 py-0.5 rounded-full text-xs font-medium',
                  TYPE_COLORS[String(s.scamType)] ?? 'bg-gray-100 text-gray-700')}>
                  {SCAM_TYPES.find(t => t.value === String(s.scamType))?.label ?? s.scamTypeName}
                </span>
                <span className={clsx('px-2 py-0.5 rounded-full text-xs',
                  s.status === 0 ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-500')}>
                  {s.status === 0 ? 'Đang phổ biến' : s.status === 1 ? 'Đã biến đổi' : 'Lưu trữ'}
                </span>
              </div>
              <h3 className="font-semibold text-gray-800 group-hover:text-red-600 transition-colors line-clamp-2">
                {s.title}
              </h3>
              <p className="text-sm text-gray-500 line-clamp-2 flex-1">{s.description}</p>
              <div className="flex items-center text-xs text-red-600 font-medium mt-auto">
                Xem chi tiết <ChevronRight className="w-3 h-3 ml-1" />
              </div>
            </Link>
          ))}
        </div>
      )}

      {/* Pagination */}
      {!search && data && (
        <div className="flex justify-center gap-2">
          <button disabled={page <= 1} onClick={() => setPage(p => p - 1)} className="btn-secondary text-sm disabled:opacity-40">← Trước</button>
          <span className="px-4 py-2 text-sm text-gray-600">Trang {page}</span>
          <button disabled={!data?.value?.hasNextPage && !data?.hasNextPage} onClick={() => setPage(p => p + 1)} className="btn-secondary text-sm disabled:opacity-40">Sau →</button>
        </div>
      )}
    </div>
  )
}
