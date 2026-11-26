import React from 'react';
import { HiShieldCheck, HiOutlinePhone, HiOutlineMail } from 'react-icons/hi';
import { Link } from 'react-router-dom';

export const Footer: React.FC = () => {
  return (
    <footer className="bg-slate-900 text-slate-400 text-sm mt-auto border-t border-slate-800">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {/* Brand */}
          <div className="md:col-span-1 space-y-3">
            <div className="flex items-center gap-2 text-white font-bold text-lg">
              <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center text-white">
                <HiShieldCheck className="w-5 h-5" />
              </div>
              AntiScamVN
            </div>
            <p className="text-xs text-slate-400 leading-relaxed">
              Hệ thống phòng chống lừa đảo trực tuyến toàn diện, phân loại tin nhắn bằng AI và bảo vệ an toàn lộ trình di chuyển.
            </p>
          </div>

          {/* Quick Links */}
          <div>
            <h4 className="text-white font-semibold mb-3 text-xs uppercase tracking-wider">Tính năng chính</h4>
            <ul className="space-y-2 text-xs">
              <li><Link to="/classify" className="hover:text-white transition">Phân tích tin nhắn lừa đảo bằng AI</Link></li>
              <li><Link to="/phone-lookup" className="hover:text-white transition">Tra cứu & Báo cáo số điện thoại/STK</Link></li>
              <li><Link to="/safe-route" className="hover:text-white transition">Lộ trình an toàn & Cảnh báo lệch tuyến</Link></li>
              <li><Link to="/scenarios" className="hover:text-white transition">Thư viện kịch bản lừa đảo mới nhất</Link></li>
            </ul>
          </div>

          {/* Emergency Numbers */}
          <div>
            <h4 className="text-white font-semibold mb-3 text-xs uppercase tracking-wider">Đường dây nóng</h4>
            <ul className="space-y-2 text-xs">
              <li className="flex items-center gap-1.5 text-rose-400 font-semibold">
                <HiOutlinePhone className="w-4 h-4" /> 113 - Công an khẩn cấp
              </li>
              <li className="flex items-center gap-1.5 text-amber-400 font-semibold">
                <HiOutlinePhone className="w-4 h-4" /> 156 / 5656 - Báo cáo tin nhắn rác, lừa đảo
              </li>
              <li className="flex items-center gap-1.5 text-blue-400">
                <HiOutlinePhone className="w-4 h-4" /> 069.234.8560 - Cục An ninh mạng (A05)
              </li>
            </ul>
          </div>

          {/* Tech Stack Info */}
          <div>
            <h4 className="text-white font-semibold mb-3 text-xs uppercase tracking-wider">Công nghệ</h4>
            <p className="text-xs text-slate-400 leading-relaxed">
              ASP.NET Core 8 Web API, MySQL 8, React 18, Tailwind CSS, Python FastAPI ML (Naive Bayes, SVM, RF), Leaflet OpenStreetMap.
            </p>
            <div className="mt-3 flex items-center gap-1.5 text-xs text-slate-500">
              <HiOutlineMail className="w-4 h-4" /> lienhe@antiscam.vn
            </div>
          </div>
        </div>

        <div className="mt-8 pt-6 border-t border-slate-800 text-center text-xs text-slate-500">
          © 2026 AntiScam VN. Dự án bảo vệ người dân trước thủ đoạn lừa đảo công nghệ cao.
        </div>
      </div>
    </footer>
  );
};

