import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import {
  HiShieldCheck,
  HiOutlinePhone,
  HiOutlineSearch,
  HiOutlineSparkles,
  HiOutlineMap,
  HiOutlineBookOpen,
  HiOutlineBell,
  HiOutlineExclamationCircle,
  HiOutlineCheckCircle,
  HiOutlineArrowRight,
} from 'react-icons/hi';
import { phoneReportApi } from '../api/services';
import toast from 'react-hot-toast';

export const HomePage: React.FC = () => {
  const navigate = useNavigate();
  const [searchNumber, setSearchNumber] = useState('');
  const [quickSearching, setQuickSearching] = useState(false);

  const handleQuickLookup = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!searchNumber.trim()) {
      toast.error('Vui lòng nhập số điện thoại hoặc số tài khoản');
      return;
    }
    navigate(`/phone-lookup?number=${encodeURIComponent(searchNumber.trim())}`);
  };

  const featureCards = [
    {
      title: 'Quét tin nhắn AI',
      desc: 'Dùng trí tuệ nhân tạo (Naive Bayes, SVM, RF) nhận diện tin nhắn rác, lừa đảo, giả mạo với độ chính xác cao.',
      icon: HiOutlineSparkles,
      to: '/classify',
      color: 'bg-indigo-50 text-indigo-600 border-indigo-200',
      btnText: 'Quét tin nhắn ngay',
    },
    {
      title: 'Tra cứu SĐT & STK',
      desc: 'Kiểm tra điểm rủi ro của số điện thoại lạ hoặc số tài khoản trước khi thực hiện chuyển tiền.',
      icon: HiOutlinePhone,
      to: '/phone-lookup',
      color: 'bg-emerald-50 text-emerald-600 border-emerald-200',
      btnText: 'Tra cứu số',
    },
    {
      title: 'Lộ trình an toàn & SOS',
      desc: 'Bản đồ định vị Geolocation, tự động đếm ngược gửi cảnh báo tới người thân nếu phát hiện lệch lộ trình.',
      icon: HiOutlineMap,
      to: '/safe-route',
      color: 'bg-rose-50 text-rose-600 border-rose-200',
      btnText: 'Bật lộ trình an toàn',
    },
    {
      title: 'Kho kịch bản lừa đảo',
      desc: 'Cập nhật liên tục các thủ đoạn mới nhất: việc nhẹ lương cao, giả mạo công an, trúng thưởng, đầu tư sinh lời.',
      icon: HiOutlineBookOpen,
      to: '/scenarios',
      color: 'bg-amber-50 text-amber-600 border-amber-200',
      btnText: 'Xem các kịch bản',
    },
  ];

  return (
    <div className="space-y-12 pb-16">
      {/* Hero Section */}
      <section className="relative overflow-hidden bg-gradient-to-b from-blue-900 via-blue-800 to-indigo-900 text-white pt-16 pb-20 px-4 sm:px-6 lg:px-8 shadow-xl">
        <div className="max-w-4xl mx-auto text-center space-y-6">
          <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-blue-500/20 border border-blue-400/30 text-blue-200 text-xs font-semibold backdrop-blur-md">
            <HiShieldCheck className="w-4 h-4 text-emerald-400" />
            Hệ thống phòng chống lừa đảo & bảo vệ công dân thế hệ mới
          </div>

          <h1 className="text-3xl sm:text-5xl font-black tracking-tight leading-tight">
            Chủ động phát hiện & ngăn chặn <br />
            <span className="text-transparent bg-clip-text bg-gradient-to-r from-blue-300 via-cyan-300 to-emerald-300">
              Mọi thủ đoạn lừa đảo tinh vi
            </span>
          </h1>

          <p className="text-slate-200 text-base sm:text-lg max-w-2xl mx-auto font-normal leading-relaxed">
            Hỗ trợ tra cứu tức thời số điện thoại, số tài khoản đáng ngờ, phân tích nội dung lừa đảo bằng mô hình học máy tự huấn luyện và bảo vệ lộ trình di chuyển ngoài đời thực.
          </p>

          {/* Search Box */}
          <div className="max-w-xl mx-auto pt-4">
            <form onSubmit={handleQuickLookup} className="relative flex items-center shadow-2xl rounded-2xl bg-white p-2">
              <HiOutlineSearch className="w-6 h-6 text-slate-400 ml-3 shrink-0" />
              <input
                type="text"
                placeholder="Nhập số điện thoại hoặc số tài khoản cần kiểm tra..."
                value={searchNumber}
                onChange={(e) => setSearchNumber(e.target.value)}
                className="w-full px-3 py-2.5 text-slate-800 text-sm sm:text-base outline-none bg-transparent placeholder-slate-400"
              />
              <button
                type="submit"
                disabled={quickSearching}
                className="shrink-0 bg-blue-600 hover:bg-blue-700 text-white px-5 py-2.5 rounded-xl font-medium text-sm transition shadow-md shadow-blue-600/30 flex items-center gap-1.5"
              >
                Kiểm tra ngay
              </button>
            </form>
            <div className="mt-3 flex items-center justify-center gap-4 text-xs text-blue-200">
              <span>Ví dụ tra cứu: 0912345678, 190365789...</span>
            </div>
          </div>
        </div>
      </section>

      {/* Emergency Hotlines Banner */}
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="bg-rose-50 border-2 border-rose-200 rounded-2xl p-4 sm:p-6 flex flex-col md:flex-row items-center justify-between gap-4">
          <div className="flex items-center gap-3">
            <div className="w-12 h-12 rounded-xl bg-rose-600 flex items-center justify-center text-white shrink-0 shadow-lg shadow-rose-500/30">
              <HiOutlineBell className="w-7 h-7 animate-bounce" />
            </div>
            <div>
              <h3 className="font-bold text-rose-900 text-base sm:text-lg">Đang gặp tình huống khẩn cấp hoặc vừa bị lừa tiền?</h3>
              <p className="text-rose-700 text-xs sm:text-sm">Liên hệ ngay cơ quan chức năng hoặc sử dụng tính năng SOS để gửi vị trí cho người thân.</p>
            </div>
          </div>
          <div className="flex items-center gap-2 shrink-0">
            <a
              href="tel:113"
              className="bg-rose-600 hover:bg-rose-700 text-white font-bold px-4 py-2 rounded-xl text-sm shadow-md transition flex items-center gap-1.5"
            >
              <HiOutlinePhone className="w-4 h-4" /> Gọi 113
            </a>
            <Link
              to="/safe-route"
              className="bg-white border border-rose-300 text-rose-700 hover:bg-rose-100 font-semibold px-4 py-2 rounded-xl text-sm transition"
            >
              Kích hoạt SOS
            </Link>
          </div>
        </div>
      </section>

      {/* Core Features Grid */}
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="text-center space-y-2 mb-10">
          <h2 className="text-2xl sm:text-3xl font-extrabold text-slate-900">Tính năng bảo vệ toàn diện</h2>
          <p className="text-slate-500 text-sm max-w-xl mx-auto">
            Kết hợp trí tuệ nhân tạo, cơ sở dữ liệu cộng đồng và định vị thời gian thực.
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {featureCards.map((feat, idx) => {
            const Icon = feat.icon;
            return (
              <div
                key={idx}
                className="bg-white rounded-2xl p-6 border border-slate-200 hover:border-blue-400 shadow-sm hover:shadow-lg transition-all flex flex-col justify-between group"
              >
                <div className="space-y-4">
                  <div className={`w-12 h-12 rounded-xl flex items-center justify-center border ${feat.color}`}>
                    <Icon className="w-6 h-6" />
                  </div>
                  <h3 className="text-lg font-bold text-slate-800 group-hover:text-blue-600 transition">
                    {feat.title}
                  </h3>
                  <p className="text-slate-500 text-xs sm:text-sm leading-relaxed">
                    {feat.desc}
                  </p>
                </div>

                <div className="pt-6">
                  <Link
                    to={feat.to}
                    className="inline-flex items-center gap-1.5 text-xs font-semibold text-blue-600 hover:text-blue-700 group-hover:translate-x-1 transition"
                  >
                    {feat.btnText}
                    <HiOutlineArrowRight className="w-3.5 h-3.5" />
                  </Link>
                </div>
              </div>
            );
          })}
        </div>
      </section>

      {/* Common Scams Notice */}
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="bg-slate-900 text-white rounded-3xl p-8 sm:p-10 relative overflow-hidden">
          <div className="max-w-2xl space-y-4 relative z-10">
            <span className="text-amber-400 text-xs font-bold uppercase tracking-wider">Cảnh giác cao độ</span>
            <h3 className="text-2xl sm:text-3xl font-bold">4 dấu hiệu lừa đảo phổ biến nhất hiện nay</h3>
            <div className="space-y-3 pt-2 text-sm text-slate-300">
              <div className="flex items-start gap-2">
                <HiOutlineExclamationCircle className="w-5 h-5 text-amber-400 shrink-0 mt-0.5" />
                <span><strong>Giả danh công an, viện kiểm sát:</strong> Đe dọa liên quan án ma túy, rửa tiền, yêu cầu chuyển tiền vào "tài khoản tạm giữ".</span>
              </div>
              <div className="flex items-start gap-2">
                <HiOutlineExclamationCircle className="w-5 h-5 text-amber-400 shrink-0 mt-0.5" />
                <span><strong>Việc nhẹ lương cao:</strong> Tuyển cộng tác viên like video TikTok, giật đơn Shopee, nạp tiền làm nhiệm vụ rồi khóa tài khoản.</span>
              </div>
              <div className="flex items-start gap-2">
                <HiOutlineExclamationCircle className="w-5 h-5 text-amber-400 shrink-0 mt-0.5" />
                <span><strong>Đầu tư tài chính siêu lợi nhuận:</strong> Hứa hẹn lãi suất 20-30%/tháng qua các sàn Forex, tiền ảo không có giấy phép.</span>
              </div>
            </div>
            <div className="pt-4">
              <Link
                to="/scenarios"
                className="bg-blue-600 hover:bg-blue-700 text-white font-semibold px-5 py-2.5 rounded-xl text-sm transition inline-flex items-center gap-2"
              >
                Xem chi tiết các kịch bản
                <HiOutlineArrowRight className="w-4 h-4" />
              </Link>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};

