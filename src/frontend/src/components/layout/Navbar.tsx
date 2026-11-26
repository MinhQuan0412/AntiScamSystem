import { useState } from 'react'
import { Link, NavLink, useNavigate } from 'react-router-dom'
import { Shield, Menu, X, AlertTriangle, Phone, Map, BarChart2, LogIn, LogOut, User, Settings } from 'lucide-react'
import { useAuth } from '../../contexts/AuthContext'
import clsx from 'clsx'

const NAV_PUBLIC = [
  { to: '/classify',   label: 'Kiểm tra tin nhắn', icon: AlertTriangle },
  { to: '/lookup',     label: 'Tra cứu số',         icon: Phone },
  { to: '/scenarios',  label: 'Kịch bản lừa đảo',  icon: Shield },
  { to: '/emergency',  label: 'Số khẩn cấp',        icon: Phone },
]
const NAV_USER = [
  { to: '/travel',     label: 'Lộ trình',           icon: Map },
]
const NAV_ADMIN = [
  { to: '/admin',      label: 'Quản trị',           icon: Settings },
  { to: '/statistics', label: 'Thống kê',           icon: BarChart2 },
]

export default function Navbar() {
  const { user, isAuthenticated, isAdmin, logout } = useAuth()
  const navigate = useNavigate()
  const [open, setOpen] = useState(false)

  const links = [
    ...NAV_PUBLIC,
    ...(isAuthenticated ? NAV_USER : []),
    ...(isAdmin ? NAV_ADMIN : []),
  ]

  const handleLogout = () => { logout(); navigate('/') }

  return (
    <header className="sticky top-0 z-50 bg-white border-b border-gray-200 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6">
        <div className="flex items-center justify-between h-16">

          {/* Logo */}
          <Link to="/" className="flex items-center gap-2 font-bold text-red-600 text-lg shrink-0">
            <Shield className="w-6 h-6" />
            <span className="hidden sm:block">AntiScam</span>
          </Link>

          {/* Desktop nav */}
          <nav className="hidden lg:flex items-center gap-1">
            {links.map(({ to, label }) => (
              <NavLink key={to} to={to}
                className={({ isActive }) => clsx(
                  'px-3 py-2 rounded-lg text-sm font-medium transition-colors',
                  isActive
                    ? 'bg-red-50 text-red-600'
                    : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'
                )}>
                {label}
              </NavLink>
            ))}
          </nav>

          {/* Desktop right */}
          <div className="hidden lg:flex items-center gap-2">
            {isAuthenticated ? (
              <>
                <span className="text-sm text-gray-600 max-w-[140px] truncate">{user?.fullName}</span>
                <button onClick={handleLogout} className="btn-ghost text-sm gap-1.5">
                  <LogOut className="w-4 h-4" /> Đăng xuất
                </button>
              </>
            ) : (
              <>
                <Link to="/login"    className="btn-ghost text-sm">Đăng nhập</Link>
                <Link to="/register" className="btn-primary text-sm">Đăng ký</Link>
              </>
            )}
          </div>

          {/* Mobile hamburger */}
          <button className="lg:hidden btn-ghost p-2" onClick={() => setOpen(!open)} aria-label="Menu">
            {open ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
          </button>
        </div>
      </div>

      {/* Mobile menu */}
      {open && (
        <div className="lg:hidden border-t border-gray-100 bg-white px-4 pb-4">
          <nav className="flex flex-col gap-1 pt-3">
            {links.map(({ to, label, icon: Icon }) => (
              <NavLink key={to} to={to} onClick={() => setOpen(false)}
                className={({ isActive }) => clsx(
                  'flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium',
                  isActive ? 'bg-red-50 text-red-600' : 'text-gray-700 hover:bg-gray-100'
                )}>
                <Icon className="w-4 h-4 shrink-0" />
                {label}
              </NavLink>
            ))}
          </nav>
          <div className="mt-3 pt-3 border-t border-gray-100 flex flex-col gap-2">
            {isAuthenticated ? (
              <>
                <div className="flex items-center gap-2 px-3 py-2 text-sm text-gray-600">
                  <User className="w-4 h-4" /> {user?.fullName}
                </div>
                <button onClick={() => { handleLogout(); setOpen(false) }}
                  className="btn-secondary w-full justify-center">
                  <LogOut className="w-4 h-4" /> Đăng xuất
                </button>
              </>
            ) : (
              <>
                <Link to="/login"    onClick={() => setOpen(false)} className="btn-secondary w-full justify-center"><LogIn className="w-4 h-4" /> Đăng nhập</Link>
                <Link to="/register" onClick={() => setOpen(false)} className="btn-primary w-full justify-center">Đăng ký miễn phí</Link>
              </>
            )}
          </div>
        </div>
      )}
    </header>
  )
}
