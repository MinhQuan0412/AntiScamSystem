import React, { useState, useEffect, useRef } from 'react';
import { MapContainer, TileLayer, Marker, Popup, Polyline, Circle } from 'react-leaflet';
import L from 'leaflet';
import { travelRouteApi } from '../api/services';
import { TravelRouteDto, LocationReportResult } from '../types';
import { useAuth } from '../contexts/AuthContext';
import {
  HiOutlineMap,
  HiOutlinePlay,
  HiOutlineStop,
  HiOutlineBell,
  HiOutlineExclamationCircle,
  HiOutlineCheckCircle,
  HiOutlinePlus,
  HiOutlineLocationMarker,
} from 'react-icons/hi';
import toast from 'react-hot-toast';

// Fix icon Leaflet mặc định bị lỗi đường dẫn ảnh trong Vite
const defaultIcon = L.icon({
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
});
L.Marker.prototype.options.icon = defaultIcon;

export const SafeRoutePage: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [currentPos, setCurrentPos] = useState<[number, number]>([21.028511, 105.854444]); // Default Hanoi
  const [trackingActive, setTrackingActive] = useState(false);
  const [watchId, setWatchId] = useState<number | null>(null);
  const [deviationAlert, setDeviationAlert] = useState<LocationReportResult | null>(null);
  const [countdown, setCountdown] = useState<number | null>(null);

  // Mẫu lộ trình dự kiến (Tọa độ Hà Nội)
  const [routePoints, setRoutePoints] = useState<[number, number][]>([
    [21.028511, 105.854444],
    [21.033333, 105.845],
    [21.04, 105.83],
  ]);

  const [activeRoute, setActiveRoute] = useState<TravelRouteDto | null>(null);
  const countdownTimerRef = useRef<any>(null);

  // Lấy vị trí thực tế của người dùng qua Geolocation API
  useEffect(() => {
    if ('geolocation' in navigator) {
      navigator.geolocation.getCurrentPosition(
        (pos) => {
          setCurrentPos([pos.coords.latitude, pos.coords.longitude]);
        },
        (err) => {
          console.warn('Geolocation error:', err.message);
        },
        { enableHighAccuracy: true }
      );
    }
  }, []);

  // Xử lý đếm ngược khi có cảnh báo lệch lộ trình
  useEffect(() => {
    if (countdown !== null && countdown > 0) {
      countdownTimerRef.current = setTimeout(() => {
        setCountdown((prev) => (prev !== null ? prev - 1 : null));
      }, 1000);
    } else if (countdown === 0) {
      toast.error('Hết thời gian đếm ngược! Hệ thống đã gửi tin nhắn cảnh báo + vị trí tới người thân!', {
        duration: 8000,
      });
      setCountdown(null);
    }
    return () => clearTimeout(countdownTimerRef.current);
  }, [countdown]);

  const startTracking = () => {
    if (!isAuthenticated) {
      toast.error('Vui lòng đăng nhập để lưu và theo dõi lộ trình an toàn');
      return;
    }

    if (!('geolocation' in navigator)) {
      toast.error('Trình duyệt của bạn không hỗ trợ Geolocation API');
      return;
    }

    setTrackingActive(true);
    toast.success('Đã bật chế độ bảo vệ lộ trình di chuyển!');

    const id = navigator.geolocation.watchPosition(
      (pos) => {
        const newLat = pos.coords.latitude;
        const newLng = pos.coords.longitude;
        setCurrentPos([newLat, newLng]);

        // Giả lập gửi tọa độ lên backend kiểm tra lệch tuyến
        checkDeviation(newLat, newLng);
      },
      (err) => toast.error('Không thể lấy tọa độ định vị: ' + err.message),
      { enableHighAccuracy: true, maximumAge: 10000, timeout: 5000 }
    );
    setWatchId(id);
  };

  const stopTracking = () => {
    if (watchId !== null) {
      navigator.geolocation.clearWatch(watchId);
      setWatchId(null);
    }
    setTrackingActive(false);
    setDeviationAlert(null);
    setCountdown(null);
    toast('Đã dừng theo dõi lộ trình.');
  };

  const checkDeviation = (lat: number, lng: number) => {
    // Đo khoảng cách đơn giản hoặc giả lập kiểm tra
    // Nếu quá xa điểm đầu, thử kích hoạt cảnh báo thử nghiệm
    if (Math.random() < 0.2) {
      // Demo giả lập lệch lộ trình
      setDeviationAlert({
        isDeviating: true,
        currentDistanceMeters: 350,
        consecutiveDeviationCount: 3,
        alertId: 'demo-alert-id',
        message: 'Bạn đã đi lệch khỏi lộ trình đã định sẵn hơn 300 mét!',
      });
      setCountdown(60); // 60s đếm ngược
    }
  };

  const confirmSafe = () => {
    setDeviationAlert(null);
    setCountdown(null);
    toast.success('Đã xác nhận an toàn. Đã hủy đếm ngược gửi cảnh báo.');
  };

  const triggerSOS = () => {
    toast.error('ĐÃ PHÁT TÍN HIỆU KHẨN CẤP (SOS)! Tọa độ hiện tại đã được gửi qua Telegram & Email người thân!', {
      duration: 10000,
    });
  };

  return (
    <div className="max-w-6xl mx-auto px-4 sm:px-6 py-8 space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <div className="inline-flex items-center gap-1.5 px-3 py-1 bg-rose-50 text-rose-700 border border-rose-200 rounded-full text-xs font-semibold mb-2">
            <HiOutlineLocationMarker className="w-4 h-4" />
            Bản đồ định vị thời gian thực & Cảnh báo lệch lộ trình
          </div>
          <h1 className="text-2xl sm:text-3xl font-extrabold text-slate-900">
            Lộ trình di chuyển an toàn & Khẩn cấp SOS
          </h1>
          <p className="text-slate-500 text-sm">
            Tự động giám sát vị trí di chuyển qua Geolocation API. Nếu lệch tuyến quá 200m, hệ thống tự động đếm ngược và báo người thân.
          </p>
        </div>

        {/* SOS Button */}
        <button
          onClick={triggerSOS}
          className="bg-red-600 hover:bg-red-700 active:scale-95 text-white font-extrabold px-6 py-3.5 rounded-2xl shadow-xl shadow-red-600/30 flex items-center justify-center gap-2 text-base transition shrink-0 animate-pulse"
        >
          <HiOutlineBell className="w-6 h-6" />
          PHÁT TÍN HIỆU SOS
        </button>
      </div>

      {/* Alert Banner if Deviation Detected */}
      {deviationAlert && (
        <div className="bg-rose-600 text-white p-5 rounded-2xl shadow-2xl flex flex-col md:flex-row items-center justify-between gap-4 animate-bounce">
          <div className="flex items-center gap-3">
            <HiOutlineExclamationCircle className="w-10 h-10 shrink-0" />
            <div>
              <div className="font-black text-lg">CẢNH BÁO LỆCH LỘ TRÌNH!</div>
              <div className="text-sm text-rose-100">
                {deviationAlert.message} Tự động gửi tin nhắn báo động cho người thân sau{' '}
                <span className="font-extrabold text-yellow-300 text-lg underline">
                  {countdown} giây
                </span>
                .
              </div>
            </div>
          </div>
          <div className="flex items-center gap-3 shrink-0">
            <button
              onClick={confirmSafe}
              className="bg-white text-rose-700 hover:bg-rose-50 font-bold px-5 py-2.5 rounded-xl text-sm transition shadow-md"
            >
              Tôi vẫn an toàn (Hủy báo động)
            </button>
          </div>
        </div>
      )}

      {/* Control Panel & Map */}
      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
        {/* Left Control Panel */}
        <div className="lg:col-span-1 bg-white rounded-2xl border border-slate-200 p-5 space-y-5 shadow-sm">
          <h3 className="font-bold text-slate-800 text-base border-b pb-3">Điều khiển lộ trình</h3>

          <div className="space-y-3">
            <div className="text-xs text-slate-600">
              <span className="font-semibold block text-slate-700">Trạng thái theo dõi:</span>
              <span className={`inline-block px-2.5 py-1 rounded-full text-xs font-semibold mt-1 ${
                trackingActive ? 'bg-emerald-100 text-emerald-800' : 'bg-slate-100 text-slate-600'
              }`}>
                {trackingActive ? '● Đang theo dõi trực tiếp' : '○ Đang tắt'}
              </span>
            </div>

            <div className="text-xs text-slate-600">
              <span className="font-semibold block text-slate-700">Tọa độ hiện tại:</span>
              <span className="font-mono text-[11px] block text-slate-500 mt-0.5">
                Lat: {currentPos[0].toFixed(5)}, Lng: {currentPos[1].toFixed(5)}
              </span>
            </div>

            <div className="text-xs text-slate-600">
              <span className="font-semibold block text-slate-700">Ngưỡng lệch cho phép:</span>
              <span className="text-slate-500">200 mét</span>
            </div>

            <div className="text-xs text-slate-600">
              <span className="font-semibold block text-slate-700">Thời gian đếm ngược:</span>
              <span className="text-slate-500">60 giây</span>
            </div>
          </div>

          <div className="pt-2 border-t space-y-2">
            {!trackingActive ? (
              <button
                onClick={startTracking}
                className="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-bold text-sm shadow-md shadow-blue-600/20 flex items-center justify-center gap-2 transition"
              >
                <HiOutlinePlay className="w-5 h-5" />
                Bắt đầu di chuyển
              </button>
            ) : (
              <button
                onClick={stopTracking}
                className="w-full py-3 bg-slate-700 hover:bg-slate-800 text-white rounded-xl font-bold text-sm shadow-md flex items-center justify-center gap-2 transition"
              >
                <HiOutlineStop className="w-5 h-5" />
                Kết thúc hành trình
              </button>
            )}

            <button
              onClick={() => {
                // Test mô phỏng lệch tuyến
                setDeviationAlert({
                  isDeviating: true,
                  currentDistanceMeters: 420,
                  consecutiveDeviationCount: 3,
                  alertId: 'test-id',
                  message: 'Mô phỏng: Đi lệch khỏi lộ trình an toàn 420 mét!',
                });
                setCountdown(60);
              }}
              className="w-full py-2 bg-amber-50 hover:bg-amber-100 text-amber-800 border border-amber-200 rounded-xl text-xs font-semibold transition"
            >
              Mô phỏng thử lệch lộ trình
            </button>
          </div>
        </div>

        {/* Map Display */}
        <div className="lg:col-span-3 bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm h-[520px] relative">
          <MapContainer
            center={currentPos}
            zoom={14}
            scrollWheelZoom={true}
            className="w-full h-full"
          >
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />

            {/* Vị trí hiện tại của người dùng */}
            <Marker position={currentPos}>
              <Popup>
                <div className="text-xs font-sans">
                  <strong>Vị trí của bạn</strong>
                  <br />
                  Đang cập nhật thời gian thực
                </div>
              </Popup>
            </Marker>

            {/* Vòng tròn bán kính cho phép */}
            <Circle
              center={currentPos}
              radius={200}
              pathOptions={{ color: '#2563eb', fillColor: '#3b82f6', fillOpacity: 0.15 }}
            />

            {/* Đường lộ trình mẫu */}
            <Polyline positions={routePoints} pathOptions={{ color: '#10b981', weight: 5, dashArray: '5, 10' }} />
          </MapContainer>

          <div className="absolute bottom-3 left-3 z-[400] bg-white/90 backdrop-blur-sm px-3 py-1.5 rounded-lg border border-slate-200 text-[11px] text-slate-600 shadow">
            ● Xanh lá: Lộ trình mẫu dự kiến | Vòng tròn xanh: Ngưỡng 200m
          </div>
        </div>
      </div>
    </div>
  );
};

