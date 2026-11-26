import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Shield, Loader2 } from 'lucide-react'
import { authApi } from '../api/endpoints'
import { saveAuth } from '../store/authStore'
import toast from 'react-hot-toast'

export default function RegisterPage() {
  const navigate = useNavigate()
  const [form, setForm]     = useState({ fullName: '', email: '', password: '', confirmPassword: '' })
  const [loading, setLoading] = useState(false)
  const set = (k: string) => (e: React.ChangeEvent<HTMLInputElement>) => setForm(p => ({ ...p, [k]: e.target.value }))

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    if (form.password !== form.confirmPassword) { toast.error('Mật khẩu xác nhận không khớp'); return }
    setLoading(true)
    try {
      const res  = await authApi.register(form.email, form.password, form.confirmPassword, form.fullName)
      const data = res.data.value ?? res.data
      saveAuth(data.accessToken, { userId: data.userId, email: data.email, role: data.role, fullName: data.fullName })
      toast.success('Đăng ký thành công!')
      navigate('/')
    } catch (e: any) {
      toast.error(e?.response?.data?.errors?.[0] ?? 'Đăng ký thất bại')
    } finally { setLoading(false) }
  }

  return (
    <div className="min-h-[70vh] flex items-center justify-center">
      <div className="w-full max-w-sm">
        <div className="text-center mb-8">
          <div className="inline-flex bg-red-100 p-3 rounded-full mb-3">
            <Shield className="w-8 h-8 text-red-600" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900">Đăng ký tài khoản</h1>
        </div>
        <form onSubmit={handleSubmit} className="card space-y-4">
          {[
            { k: 'fullName',        label: 'Họ và tên',         type: 'text',     ph: 'Nguyễn Văn A' },
            { k: 'email',           label: 'Email',              type: 'email',    ph: 'email@example.com' },
            { k: 'password',        label: 'Mật khẩu',           type: 'password', ph: 'Ít nhất 8 ký tự' },
            { k: 'confirmPassword', label: 'Xác nhận mật khẩu',  type: 'password', ph: '••••••••' },
          ].map(({ k, label, type, ph }) => (
            <div key={k}>
              <label className="block text-sm font-medium text-gray-700 mb-1">{label}</label>
              <input type={type} value={(form as any)[k]} onChange={set(k)}
                required className="input" placeholder={ph} />
            </div>
          ))}
          <button type="submit" disabled={loading} className="btn-primary w-full justify-center py-3">
            {loading ? <Loader2 className="w-4 h-4 animate-spin" /> : null} Đăng ký
          </button>
          <p className="text-center text-sm text-gray-500">
            Đã có tài khoản? <Link to="/login" className="text-red-600 hover:underline font-medium">Đăng nhập</Link>
          </p>
        </form>
      </div>
    </div>
  )
}
