import React, { useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import {
  HiShieldCheck,
  HiOutlinePhone,
  HiOutlineMap,
  HiOutlineBookOpen,
  HiOutlineBell,
  HiOutlineChartBar,
  HiOutlineMenu,
  HiOutlineX,
  HiOutlineLogout,
  HiOutlineLogin,
  HiOutlineUserAdd,
  HiOutlineSparkles,
} from 'react-icons/hi';

export const Navbar: React.FC = () => {
  const { user, isAuthenticated, isAdmin, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const navLinks = [
    { to: '/', label: 'Trang chủ', icon: HiShieldCheck },
    { to: '/classify', label: 'Quét tin nhắn AI', icon: HiOutlineSparkles },
    { to: '/phone-lookup', label: 'Tra cứu & Báo cáo', icon: HiOutlinePhone },
    { to: '/safe-route', label: 'Lộ trình an toàn', icon: HiOutlineMap },
    { to: '/scenarios', label: 'Kịch bản lừa đảo', icon: HiOutlineBookOpen },
    { to: '/emergency', label: 'Danh bạ khẩn cấp', icon: HiOutlineBell },
  ];

  if (isAdmin) {
    navLinks.push({ to: '/admin/dashboard', label: 'Quản trị', icon: HiOutlineChartBar });
  }

  const isActive = (path: string) => location.pathname === path;

  return (
    <nav className="sticky top-0 z-50 bg-white/95 backdrop-blur-md border-b border-slate-200 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link to="/" className="flex items-center gap-2.5 font-bold text-xl text-blue-600 hover:text-blue-700 transition">
            <div className="w-10 h-10 rounded-xl bg-blue-600 flex items-center justify-center text-white shadow-md shadow-blue-500/20">
              <HiShieldCheck className="w-6 h-6" />
            </div>
            <div className="flex flex-col">
              <span className="text-slate-900 tracking-tight leading-none text-lg font-extrabold">
                AntiScam<span className="text-blue-600">VN</span>
              </span>
              <span className="text-[10px] text-slate-500 font-medium tracking-wider uppercase mt-0.5">Bảo vệ toàn diện</span>
            </div>
          </Link>

          {/* Desktop Navigation Links */}
          <div className="hidden lg:flex items-center space-x-1">
            {navLinks.map((link) => {
              const Icon = link.icon;
              const active = isActive(link.to);
              return (
                <Link
                  key={link.to}
                  to={link.to}
                  className={`flex items-center gap-1.5 px-3 py-2 rounded-lg text-sm font-medium transition ${
                    active
                      ? 'bg-blue-50 text-blue-600 font-semibold'
                      : 'text-slate-650 hover:bg-slate-100 hover:text-slate-900'
                  }`}
                >
                  <Icon className={`w-4 h-4 ${active ? 'text-blue-600' : 'text-slate-400'}`} />
                  {link.label}
                </Link>
              );
            })}
          </div>

          {/* Auth Controls */}
          <div className="hidden md:flex items-center gap-3">
            {isAuthenticated ? (
              <div className="flex items-center gap-3">
                <div className="flex flex-col items-end">
                  <span className="text-sm font-semibold text-slate-800">{user?.fullName}</span>
                  <span className="text-xs text-blue-600 font-medium bg-blue-50 px-2 py-0.5 rounded-full">
                    {isAdmin ? 'Quản trị viên' : 'Thành viên'}
                  </span>
                </div>
                <button
                  onClick={() => {
                    logout();
                    navigate('/');
                  }}
                  className="p-2 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition"
                  title="Đăng xuất"
                >
                  <HiOutlineLogout className="w-5 h-5" />
                </button>
              </div>
            ) : (
              <div className="flex items-center gap-2">
                <Link
                  to="/login"
                  className="flex items-center gap-1.5 px-3.5 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100 rounded-lg transition"
                >
                  <HiOutlineLogin className="w-4 h-4" />
                  Đăng nhập
                </Link>
                <Link
                  to="/register"
                  className="flex items-center gap-1.5 px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg shadow-sm shadow-blue-500/20 transition"
                >
                  <HiOutlineUserAdd className="w-4 h-4" />
                  Đăng ký
                </Link>
              </div>
            )}
          </div>

          {/* Mobile hamburger menu button */}
          <div className="flex lg:hidden">
            <button
              onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
              className="p-2 text-slate-600 hover:bg-slate-100 rounded-lg"
              aria-label="Toggle menu"
            >
              {mobileMenuOpen ? <HiOutlineX className="w-6 h-6" /> : <HiOutlineMenu className="w-6 h-6" />}
            </button>
          </div>
        </div>
      </div>

      {/* Mobile Dropdown Menu */}
      {mobileMenuOpen && (
        <div className="lg:hidden border-t border-slate-200 bg-white px-4 pt-2 pb-6 space-y-1">
          {navLinks.map((link) => {
            const Icon = link.icon;
            const active = isActive(link.to);
            return (
              <Link
                key={link.to}
                to={link.to}
                onClick={() => setMobileMenuOpen(false)}
                className={`flex items-center gap-2.5 px-3 py-2.5 rounded-lg text-base font-medium transition ${
                  active ? 'bg-blue-50 text-blue-600 font-semibold' : 'text-slate-700 hover:bg-slate-50'
                }`}
              >
                <Icon className={`w-5 h-5 ${active ? 'text-blue-600' : 'text-slate-400'}`} />
                {link.label}
              </Link>
            );
          })}

          <div className="pt-4 border-t border-slate-100">
            {isAuthenticated ? (
              <div className="flex items-center justify-between px-2 pt-2">
                <div>
                  <div className="font-semibold text-slate-800">{user?.fullName}</div>
                  <div className="text-xs text-blue-600">{user?.email}</div>
                </div>
                <button
                  onClick={() => {
                    logout();
                    setMobileMenuOpen(false);
                    navigate('/');
                  }}
                  className="flex items-center gap-1.5 px-3 py-1.5 text-sm font-medium text-rose-600 bg-rose-50 rounded-lg"
                >
                  <HiOutlineLogout className="w-4 h-4" />
                  Thoát
                </button>
              </div>
            ) : (
              <div className="grid grid-cols-2 gap-2 pt-2">
                <Link
                  to="/login"
                  onClick={() => setMobileMenuOpen(false)}
                  className="text-center py-2 text-sm font-medium text-slate-700 border border-slate-200 rounded-lg"
                >
                  Đăng nhập
                </Link>
                <Link
                  to="/register"
                  onClick={() => setMobileMenuOpen(false)}
                  className="text-center py-2 text-sm font-medium text-white bg-blue-600 rounded-lg"
                >
                  Đăng ký
                </Link>
              </div>
            )}
          </div>
        </div>
      )}
    </nav>
  );
};

