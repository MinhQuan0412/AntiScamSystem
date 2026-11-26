import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Shield, Loader2, Eye, EyeOff } from 'lucide-react'
import { authApi } from '../api/endpoints'
import { saveAuth } from '../store/authStore'
import toast from 'react-hot-toast'

export default function LoginPage() {
  const navigate = useNavigate()
  const [email, setEmail]       = useState('')
  const [password, setPassword] = useState('')
  const [showPw, setShowPw]     = useState(false)
  const [loading, setLoading]   = useState(false)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setLoading(true)
    try {
      const res  = await authApi.login(email, password)
      const data = res.data.value ?? res.data
      saveAuth(data.accessToken, {
        userId: data.userId, email: data.email,
        role: data.role, fullName: data.fullName
      })
      toast.success(`Chào mừng, ${data.fullName || data.email}!`)
      navigate(data.role?.toLowerCase() === 'admin' ? '/admin' : '/')
    } catch (e: any) {
      toast.error(e?.response?.data?.errors?.[0] ?? 'Email hoặc mật khẩu không đúng')
    } finally { setLoading(false) }
  }

  return (
    <div className="min-h-[70vh] flex items-center justify-center">
      <div className="w-full max-w-sm">
        <div className="text-center mb-8">
          <div className="inline-flex bg-red-100 p-3 rounded-full mb-3">
            <Shield className="w-8 h-8 text-red-600" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900">Đăng nhập</h1>
          <p className="text-gray-500 text-sm mt-1">Hệ thống AntiScam</p>
        </div>

        <form onSubmit={handleSubmit} className="card space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
            <input type="email" value={email} onChange={e => setEmail(e.target.value)}
              required className="input" placeholder="email@example.com" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Mật khẩu</label>
            <div className="relative">
              <input type={showPw ? 'text' : 'password'} value={password}
                onChange={e => setPassword(e.target.value)}
                required className="input pr-10" placeholder="••••••••" />
              <button type="button" onClick={() => setShowPw(!showPw)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400">
                {showPw ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
              </button>
            </div>
          </div>
          <button type="submit" disabled={loading} className="btn-primary w-full justify-center py-3">
            {loading ? <Loader2 className="w-4 h-4 animate-spin" /> : null}
            Đăng nhập
          </button>
          <p className="text-center text-sm text-gray-500">
            Chưa có tài khoản?{' '}
            <Link to="/register" className="text-red-600 hover:underline font-medium">Đăng ký</Link>
          </p>
        </form>

        <p className="text-center text-xs text-gray-400 mt-4">
          Admin mặc định: <code className="bg-gray-100 px-1 rounded">admin@antiscam.local</code>
        </p>
      </div>
    </div>
  )
}
