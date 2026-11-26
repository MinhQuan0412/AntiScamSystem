import { useState, useEffect, useRef } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { MapContainer, TileLayer, Marker, Polyline, useMapEvents } from 'react-leaflet'
import { Map, Plus, Play, Square, AlertTriangle, Radio, Loader2, Trash2 } from 'lucide-react'
import { travelApi } from '../api/endpoints'
import { isLoggedIn } from '../store/authStore'
import { Link } from 'react-router-dom'
import toast from 'react-hot-toast'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'

// Fix leaflet default icon
delete (L.Icon.Default.prototype as any)._getIconUrl
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
})

interface Waypoint { latitude: number; longitude: number; name?: string; order: number }

function MapClickHandler({ onAdd }: { onAdd: (wp: Waypoint) => void }) {
  let order = useRef(0)
  useMapEvents({
    click(e) {
      order.current += 1
      onAdd({ latitude: e.latlng.lat, longitude: e.latlng.lng, order: order.current })
    }
  })
  return null
}

export default function TravelPage() {
  const qc = useQueryClient()
  const [creating, setCreating] = useState(false)
  const [routeName, setRouteName] = useState('')
  const [waypoints, setWaypoints] = useState<Waypoint[]>([])
  const [threshold, setThreshold] = useState(200)
  const [tracking, setTracking] = useState(false)
  const [watchId, setWatchId]   = useState<number | null>(null)
  const [alert, setAlert]       = useState<any>(null)
  const loggedIn = isLoggedIn()

  const { data: activeRoute } = useQuery({
    queryKey: ['active-route'],
    queryFn: () => travelApi.getActive().then(r => r.data?.value ?? r.data),
    enabled: loggedIn,
    refetchInterval: tracking ? 10000 : false,
  })

  const { data: myRoutes } = useQuery({
    queryKey: ['my-routes'],
    queryFn: () => travelApi.getMyRoutes().then(r => r.data?.value ?? r.data),
    enabled: loggedIn,
  })

  const createMut = useMutation({
    mutationFn: (data: any) => travelApi.create(data),
    onSuccess: () => {
      toast.success('Tạo lộ trình thành công!')
      qc.invalidateQueries({ queryKey: ['my-routes'] })
      setCreating(false); setWaypoints([]); setRouteName('')
    },
    onError: () => toast.error('Lỗi tạo lộ trình'),
  })

  const startMut = useMutation({
    mutationFn: (id: string) => travelApi.start(id),
    onSuccess: () => {
      toast.success('Bắt đầu theo dõi lộ trình!'); setTracking(true)
      qc.invalidateQueries({ queryKey: ['active-route'] })
      startGPS()
    },
  })

  const stopMut = useMutation({
    mutationFn: (id: string) => travelApi.stop(id),
    onSuccess: () => {
      toast.success('Đã dừng theo dõi'); setTracking(false)
      stopGPS(); qc.invalidateQueries({ queryKey: ['active-route', 'my-routes'] })
    },
  })

  const sosMut = useMutation({
    mutationFn: (id: string) => travelApi.sos(id),
    onSuccess: () => toast.success('🆘 Đã gửi tín hiệu SOS đến người thân!'),
    onError: () => toast.error('Lỗi gửi SOS'),
  })

  function startGPS() {
    if (!navigator.geolocation || !activeRoute?.id) return
    const id = navigator.geolocation.watchPosition(
      async pos => {
        try {
          const res = await travelApi.reportLocation({
            travelRouteId: activeRoute.id,
            latitude: pos.coords.latitude,
            longitude: pos.coords.longitude,
            accuracy: pos.coords.accuracy,
            speed: pos.coords.speed,
          })
          const result = res.data?.value ?? res.data
          if (result?.isDeviating) setAlert(result)
        } catch {}
      },
      () => toast.error('Không thể lấy vị trí GPS'),
      { enableHighAccuracy: true, maximumAge: 15000, timeout: 10000 }
    )
    setWatchId(id)
  }

  function stopGPS() {
    if (watchId !== null) navigator.geolocation.clearWatch(watchId)
    setWatchId(null)
  }

  async function respondAlert(action: string) {
    if (!alert?.alertId) return
    await travelApi.respondToAlert({ alertId: alert.alertId, action })
    setAlert(null)
    toast.success(action === 'ImOkay' ? 'Đã xác nhận an toàn!' : 'Đã cập nhật lộ trình!')
  }

  if (!loggedIn) {
    return (
      <div className="text-center py-16">
        <Map className="w-16 h-16 mx-auto mb-4 text-gray-300" />
        <h2 className="text-xl font-bold text-gray-700 mb-2">Cần đăng nhập</h2>
        <p className="text-gray-500 mb-4">Đăng nhập để sử dụng tính năng lộ trình du lịch</p>
        <Link to="/login" className="btn-primary inline-flex">Đăng nhập ngay</Link>
      </div>
    )
  }

  const routes: any[] = Array.isArray(myRoutes) ? myRoutes : []

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Lộ trình du lịch</h1>
          <p className="text-gray-500 text-sm mt-1">Theo dõi vị trí và nhận cảnh báo lệch tuyến</p>
        </div>
        <button onClick={() => setCreating(true)} className="btn-primary text-sm">
          <Plus className="w-4 h-4" /> Tạo lộ trình
        </button>
      </div>

      {/* Alert modal */}
      {alert && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-2xl p-6 max-w-sm w-full shadow-2xl">
            <AlertTriangle className="w-12 h-12 text-orange-500 mx-auto mb-3" />
            <h3 className="text-lg font-bold text-center text-gray-800 mb-2">Cảnh báo lệch lộ trình</h3>
            <p className="text-sm text-gray-600 text-center mb-4">
              Bạn đã lệch <strong>{Math.round(alert.currentDistanceMeters)}m</strong> so với lộ trình đã đặt.
            </p>
            <div className="flex flex-col gap-2">
              <button onClick={() => respondAlert('ImOkay')} className="btn-primary justify-center">✅ Tôi ổn, tắt cảnh báo</button>
              <button onClick={() => respondAlert('ChangeRoute')} className="btn-secondary justify-center">🗺️ Đổi lộ trình mới</button>
              <button onClick={() => sosMut.mutate(activeRoute?.id)} className="bg-red-700 text-white rounded-lg py-2 font-semibold hover:bg-red-800 flex items-center justify-center gap-2">
                🆘 Gửi SOS khẩn cấp
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Active route */}
      {activeRoute?.id && (
        <div className="card border-2 border-green-300 bg-green-50">
          <div className="flex items-center justify-between flex-wrap gap-3">
            <div className="flex items-center gap-2">
              <Radio className="w-5 h-5 text-green-600 animate-pulse" />
              <div>
                <p className="font-semibold text-green-800">Đang theo dõi: {activeRoute.routeName}</p>
                <p className="text-xs text-green-600">GPS đang hoạt động</p>
              </div>
            </div>
            <div className="flex gap-2">
              <button onClick={() => sosMut.mutate(activeRoute.id)}
                className="bg-red-600 text-white text-sm font-bold px-4 py-2 rounded-lg hover:bg-red-700 flex items-center gap-1">
                🆘 SOS
              </button>
              <button onClick={() => stopMut.mutate(activeRoute.id)}
                className="btn-secondary text-sm">
                <Square className="w-4 h-4" /> Dừng
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Create form */}
      {creating && (
        <div className="card space-y-4">
          <h3 className="font-semibold text-gray-800">Tạo lộ trình mới</h3>
          <input value={routeName} onChange={e => setRouteName(e.target.value)}
            className="input" placeholder="Tên lộ trình (VD: Hà Nội - Đà Lạt)" />
          <div>
            <label className="text-sm text-gray-600">Ngưỡng lệch cho phép: <strong>{threshold}m</strong></label>
            <input type="range" min={50} max={1000} step={50} value={threshold}
              onChange={e => setThreshold(Number(e.target.value))}
              className="w-full mt-1 accent-red-600" />
          </div>

          <div>
            <p className="text-sm text-gray-600 mb-2">
              Nhấn vào bản đồ để thêm điểm dừng ({waypoints.length} điểm)
            </p>
            <div className="rounded-xl overflow-hidden border h-64">
              <MapContainer center={[16.047, 108.206]} zoom={6} className="h-full w-full">
                <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
                <MapClickHandler onAdd={wp => setWaypoints(p => [...p, wp])} />
                {waypoints.map((wp, i) => (
                  <Marker key={i} position={[wp.latitude, wp.longitude]} />
                ))}
                {waypoints.length > 1 && (
                  <Polyline positions={waypoints.map(w => [w.latitude, w.longitude])} color="red" />
                )}
              </MapContainer>
            </div>
          </div>

          {waypoints.length > 0 && (
            <div className="text-xs text-gray-500 space-y-1">
              {waypoints.map((wp, i) => (
                <div key={i} className="flex justify-between">
                  <span>Điểm {i + 1}: {wp.latitude.toFixed(4)}, {wp.longitude.toFixed(4)}</span>
                  <button onClick={() => setWaypoints(p => p.filter((_, j) => j !== i))}
                    className="text-red-500 hover:text-red-700"><Trash2 className="w-3 h-3" /></button>
                </div>
              ))}
            </div>
          )}

          <div className="flex gap-2">
            <button onClick={() => createMut.mutate({ routeName, waypoints, deviationThresholdMeters: threshold })}
              disabled={!routeName || waypoints.length < 2 || createMut.isPending}
              className="btn-primary text-sm">
              {createMut.isPending ? <Loader2 className="w-4 h-4 animate-spin" /> : null} Tạo lộ trình
            </button>
            <button onClick={() => { setCreating(false); setWaypoints([]) }} className="btn-secondary text-sm">Huỷ</button>
          </div>
        </div>
      )}

      {/* Route list */}
      {routes.length > 0 && (
        <div className="space-y-3">
          <h3 className="font-semibold text-gray-700">Lộ trình của bạn</h3>
          {routes.map((r: any) => (
            <div key={r.id} className="card flex items-center justify-between gap-3 flex-wrap">
              <div>
                <p className="font-medium text-gray-800">{r.routeName}</p>
                <p className="text-xs text-gray-500">
                  {r.waypoints?.length ?? 0} điểm · Ngưỡng {r.deviationThresholdMeters}m ·{' '}
                  <span className={r.status === 1 ? 'text-green-600 font-medium' : 'text-gray-400'}>
                    {r.status === 0 ? 'Chờ' : r.status === 1 ? '● Đang theo dõi' : r.status === 3 ? 'Hoàn thành' : 'Đã huỷ'}
                  </span>
                </p>
              </div>
              {r.status === 0 && !activeRoute?.id && (
                <button onClick={() => startMut.mutate(r.id)}
                  disabled={startMut.isPending}
                  className="btn-primary text-sm">
                  <Play className="w-4 h-4" /> Bắt đầu
                </button>
              )}
            </div>
          ))}
        </div>
      )}

      {routes.length === 0 && !creating && (
        <div className="text-center py-12 text-gray-400">
          <Map className="w-12 h-12 mx-auto mb-3 opacity-30" />
          <p className="text-sm">Chưa có lộ trình nào. Tạo lộ trình để bắt đầu theo dõi an toàn.</p>
        </div>
      )}
    </div>
  )
}
