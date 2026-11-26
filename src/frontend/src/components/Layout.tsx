import { useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { Shield, Menu, X, Search, Phone, BookOpen, Map, BarChart2, LogOut, User, ChevronDown } from 'lucide-react'
import { getUser, isAdmin, isLoggedIn, clearAuth } from '../store/authStore'
import clsx from 'clsx'

const navLinks = [
  { to: '/classify',   label: 'Kiểm tra',       icon: Search  },
  { to: '/lookup',     label: 'Tra cứu số',      icon: Phone   },
  { to: '/scenarios',  label: 'Kịch bản',        icon: BookOpen },
  { to: '/emergency',  label: 'Số khẩn',         icon: Phone   },
  { to: '/travel',     label: 'Lộ trình',        icon: Map     },
]

export default function Layout({ children }: { children: React.ReactNode }) {
  const [menuOpen, setMenuOpen] = useState(false)
  const [dropOpen, setDropOpen]  = useState(false)
  const location = useLocation()
  const navigate = useNavigate()
  const user     = getUser()
  const loggedIn = isLoggedIn()
  const admin    = isAdmin()

  function handleLogout() {
    clearAuth()
    navigate('/login')
  }

  return (
    <div className="min-h-screen flex flex-col">
      {/* ── Navbar ── */}
      <nav className="bg-red-700 text-white shadow-lg sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4">
          <div className="flex items-center justify-between h-14">
            {/* Logo */}
            <Link to="/" className="flex items-center gap-2 font-bold text-lg">
              <Shield className="w-6 h-6" />
              <span className="hidden sm:inline">AntiScam</span>
            </Link>

            {/* Desktop nav */}
            <div className="hidden md:flex items-center gap-1">
              {navLinks.map(({ to, label }) => (
                <Link key={to} to={to}
                  className={clsx(
                    'px-3 py-1.5 rounded-lg text-sm font-medium transition-colors',
                    location.pathname.startsWith(to)
                      ? 'bg-red-900 text-white'
                      : 'hover:bg-red-600 text-red-100'
                  )}>
                  {label}
                </Link>
              ))}
              {admin && (
                <Link to="/admin"
                  className={clsx('px-3 py-1.5 rounded-lg text-sm font-medium transition-colors',
                    location.pathname.startsWith('/admin')
                      ? 'bg-red-900 text-white' : 'hover:bg-red-600 text-red-100')}>
                  Admin
                </Link>
              )}
            </div>

            {/* Auth */}
            <div className="hidden md:flex items-center gap-2">
              {loggedIn ? (
                <div className="relative">
                  <button onClick={() => setDropOpen(!dropOpen)}
                    className="flex items-center gap-1.5 hover:bg-red-600 px-3 py-1.5 rounded-lg text-sm">
                    <User className="w-4 h-4" />
                    <span className="max-w-24 truncate">{user?.email}</span>
                    <ChevronDown className="w-3 h-3" />
                  </button>
                  {dropOpen && (
                    <div className="absolute right-0 top-10 bg-white text-gray-800 rounded-lg shadow-lg py-1 w-44 z-50">
                      <button onClick={handleLogout}
                        className="w-full px-4 py-2 text-sm text-left hover:bg-gray-50 flex items-center gap-2 text-red-600">
                        <LogOut className="w-4 h-4" /> Đăng xuất
                      </button>
                    </div>
                  )}
                </div>
              ) : (
                <>
                  <Link to="/login"  className="text-sm hover:bg-red-600 px-3 py-1.5 rounded-lg">Đăng nhập</Link>
                  <Link to="/register" className="text-sm bg-white text-red-700 hover:bg-red-50 px-3 py-1.5 rounded-lg font-semibold">Đăng ký</Link>
                </>
              )}
            </div>

            {/* Mobile menu button */}
            <button className="md:hidden p-2" onClick={() => setMenuOpen(!menuOpen)}>
              {menuOpen ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
            </button>
          </div>
        </div>

        {/* Mobile menu */}
        {menuOpen && (
          <div className="md:hidden border-t border-red-600 px-4 py-3 space-y-1">
            {navLinks.map(({ to, label, icon: Icon }) => (
              <Link key={to} to={to}
                onClick={() => setMenuOpen(false)}
                className={clsx('flex items-center gap-2 px-3 py-2 rounded-lg text-sm',
                  location.pathname.startsWith(to) ? 'bg-red-900' : 'hover:bg-red-600')}>
                <Icon className="w-4 h-4" /> {label}
              </Link>
            ))}
            {admin && (
              <Link to="/admin" onClick={() => setMenuOpen(false)}
                className="flex items-center gap-2 px-3 py-2 rounded-lg text-sm hover:bg-red-600">
                <BarChart2 className="w-4 h-4" /> Admin
              </Link>
            )}
            <div className="pt-2 border-t border-red-600 mt-2">
              {loggedIn ? (
                <button onClick={handleLogout}
                  className="flex items-center gap-2 px-3 py-2 rounded-lg text-sm w-full hover:bg-red-600 text-red-200">
                  <LogOut className="w-4 h-4" /> Đăng xuất
                </button>
              ) : (
                <div className="flex gap-2">
                  <Link to="/login"    onClick={() => setMenuOpen(false)} className="flex-1 text-center py-2 text-sm hover:bg-red-600 rounded-lg">Đăng nhập</Link>
                  <Link to="/register" onClick={() => setMenuOpen(false)} className="flex-1 text-center py-2 text-sm bg-white text-red-700 rounded-lg font-semibold">Đăng ký</Link>
                </div>
              )}
            </div>
          </div>
        )}
      </nav>

      {/* ── Main content ── */}
      <main className="flex-1 max-w-7xl mx-auto w-full px-4 py-6">
        {children}
      </main>

      {/* ── Footer ── */}
      <footer className="bg-gray-800 text-gray-400 text-center text-xs py-4">
        © 2026 AntiScam – Hệ thống cảnh báo lừa đảo & an toàn du lịch
      </footer>
    </div>
  )
}
