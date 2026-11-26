import { useQuery } from '@tanstack/react-query'
import { Phone, AlertTriangle, Loader2 } from 'lucide-react'
import { emergencyApi } from '../api/endpoints'
import { useState } from 'react'

const PROVINCES = [
  { code: '', name: 'Toàn quốc' },
  { code: 'HN', name: 'Hà Nội' },
  { code: 'HCM', name: 'TP. Hồ Chí Minh' },
  { code: 'DN', name: 'Đà Nẵng' },
  { code: 'CT', name: 'Cần Thơ' },
  { code: 'HP', name: 'Hải Phòng' },
]

export default function EmergencyPage() {
  const [province, setProvince] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['emergency', province],
    queryFn: () => province
      ? emergencyApi.getByProvince(province).then(r => r.data?.value ?? r.data)
      : emergencyApi.getNational().then(r => r.data?.value ?? r.data),
  })

  const contacts: any[] = Array.isArray(data) ? data : []

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Danh bạ số khẩn cấp</h1>
        <p className="text-gray-500 text-sm mt-1">Các số điện thoại khẩn cấp theo địa phương</p>
      </div>

      {/* National numbers */}
      <div className="grid grid-cols-3 gap-3">
        {[
          { number: '113', label: 'Công an', color: 'bg-blue-600' },
          { number: '114', label: 'Cứu hỏa', color: 'bg-orange-500' },
          { number: '115', label: 'Cấp cứu', color: 'bg-red-600' },
        ].map(({ number, label, color }) => (
          <a key={number} href={`tel:${number}`}
            className={`${color} text-white rounded-xl p-4 text-center hover:opacity-90 transition-opacity`}>
            <p className="text-3xl font-bold">{number}</p>
            <p className="text-sm mt-1 opacity-90">{label}</p>
            <p className="text-xs opacity-75 mt-0.5">Nhấn để gọi</p>
          </a>
        ))}
      </div>

      {/* Province filter */}
      <div className="card">
        <label className="block text-sm font-medium text-gray-700 mb-2">Chọn địa phương</label>
        <select value={province} onChange={e => setProvince(e.target.value)} className="input">
          {PROVINCES.map(p => <option key={p.code} value={p.code}>{p.name}</option>)}
        </select>
      </div>

      {/* Contact list */}
      {isLoading ? (
        <div className="flex justify-center py-8"><Loader2 className="w-6 h-6 animate-spin text-red-500" /></div>
      ) : contacts.length === 0 ? (
        <div className="text-center py-10 text-gray-400">
          <AlertTriangle className="w-10 h-10 mx-auto mb-2 opacity-30" />
          <p>Chưa có dữ liệu cho địa phương này</p>
        </div>
      ) : (
        <div className="space-y-3">
          {contacts.map((c: any) => (
            <div key={c.id} className="card flex items-center justify-between gap-4">
              <div className="flex items-center gap-3">
                <div className="bg-red-100 p-2 rounded-lg">
                  <Phone className="w-5 h-5 text-red-600" />
                </div>
                <div>
                  <p className="font-semibold text-gray-800">{c.name}</p>
                  <p className="text-xs text-gray-500">{c.category}{c.provinceName ? ` · ${c.provinceName}` : ''}</p>
                  {c.description && <p className="text-xs text-gray-400 mt-0.5">{c.description}</p>}
                </div>
              </div>
              <a href={`tel:${c.phoneNumber}`}
                className="btn-primary shrink-0 text-sm">
                <Phone className="w-4 h-4" /> {c.phoneNumber}
              </a>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
