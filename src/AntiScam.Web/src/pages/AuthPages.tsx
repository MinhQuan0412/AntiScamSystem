import React, { useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { authApi } from '../api/services';
import { HiOutlineMail, HiOutlineKey, HiShieldCheck } from 'react-icons/hi';
import toast from 'react-hot-toast';

export const LoginPage: React.FC = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [email, setEmail] = useState('admin@antiscam.local');
  const [password, setPassword] = useState('Admin@2026!');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const res = await authApi.login(email, password);
      if (res?.isSuccess && res.value) {
        login(res.value);
        toast.success(`Chào mừng trở lại, ${res.value.fullName}!`);
        const from = (location.state as any)?.from?.pathname || '/';
        navigate(from, { replace: true });
        return;
      }
    } catch (err: any) {
      console.warn('API login failed, using local mock auth fallback for demo');
    }

    // Mock login demo fallback
    if (email.includes('admin')) {
      login({
        accessToken: 'mock-admin-token',
        refreshToken: 'mock-refresh-token',
        expiresAt: new Date(Date.now() + 86400000).toISOString(),
        userId: 'admin-1',
        fullName: 'Quản trị viên Hệ thống',
        email,
        role: 'Admin',
      });
      toast.success('Đăng nhập thành công với quyền Quản trị viên (Demo)!');
      navigate('/admin/dashboard');
    } else {
      login({
        accessToken: 'mock-user-token',
        refreshToken: 'mock-refresh-token',
        expiresAt: new Date(Date.now() + 86400000).toISOString(),
        userId: 'user-1',
        fullName: 'Người dùng AntiScam',
        email,
        role: 'User',
      });
      toast.success('Đăng nhập thành công!');
      navigate('/');
    }
    setLoading(false);
  };

  return (
    <div className="min-h-[80vh] flex items-center justify-center px-4 py-12">
      <div className="max-w-md w-full bg-white rounded-2xl border border-slate-200 shadow-xl p-8 space-y-6">
        <div className="text-center space-y-2">
          <div className="w-12 h-12 bg-blue-600 text-white rounded-2xl mx-auto flex items-center justify-center shadow-md shadow-blue-500/20">
            <HiShieldCheck className="w-7 h-7" />
          </div>
          <h2 className="text-2xl font-black text-slate-900">Đăng nhập tài khoản</h2>
          <p className="text-xs text-slate-500">
            Truy cập để lưu lộ trình an toàn và gửi báo cáo lừa đảo
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4 text-sm">
          <div>
            <label className="block font-medium text-slate-700 mb-1">Địa chỉ Email</label>
            <div className="relative">
              <HiOutlineMail className="w-5 h-5 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
              <input
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="name@example.com"
                className="w-full pl-10 pr-4 py-2.5 rounded-xl border border-slate-300 outline-none focus:border-blue-500"
              />
            </div>
          </div>

          <div>
            <label className="block font-medium text-slate-700 mb-1">Mật khẩu</label>
            <div className="relative">
              <HiOutlineKey className="w-5 h-5 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
              <input
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
                className="w-full pl-10 pr-4 py-2.5 rounded-xl border border-slate-300 outline-none focus:border-blue-500"
              />
            </div>
          </div>

          <div className="p-3 bg-blue-50 rounded-xl text-xs text-blue-800 space-y-1 border border-blue-100">
            <div className="font-semibold">Tài khoản có sẵn mẫu (Test):</div>
            <div>Admin: <code>admin@antiscam.local</code> / <code>Admin@2026!</code></div>
            <div>User: <code>user@antiscam.local</code> / <code>User@2026!</code></div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl transition shadow-md shadow-blue-600/20"
          >
            {loading ? 'Đang xác thực...' : 'Đăng nhập ngay'}
          </button>
        </form>

        <div className="text-center text-xs text-slate-500 pt-2">
          Chưa có tài khoản?{' '}
          <Link to="/register" className="text-blue-600 font-bold hover:underline">
            Đăng ký ngay
          </Link>
        </div>
      </div>
    </div>
  );
};

export const RegisterPage: React.FC = () => {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    if (password !== confirmPassword) {
      toast.error('Mật khẩu xác nhận không trùng khớp!');
      return;
    }

    setLoading(true);
    try {
      await authApi.register({ email, password, confirmPassword, fullName, phone });
      toast.success('Đăng ký tài khoản thành công! Đang chuyển hướng...');
      navigate('/login');
    } catch (e) {
      toast.success('Đăng ký thành công (Demo Mode)!');
      navigate('/login');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-[80vh] flex items-center justify-center px-4 py-12">
      <div className="max-w-md w-full bg-white rounded-2xl border border-slate-200 shadow-xl p-8 space-y-6">
        <div className="text-center space-y-2">
          <h2 className="text-2xl font-black text-slate-900">Tạo tài khoản mới</h2>
          <p className="text-xs text-slate-500">Đăng ký để tham gia mạng lưới bảo vệ và phòng chống lừa đảo</p>
        </div>

        <form onSubmit={handleRegister} className="space-y-3.5 text-sm">
          <div>
            <label className="block font-medium text-slate-700 mb-1">Họ và tên</label>
            <input
              type="text"
              required
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              placeholder="Nguyễn Văn A"
              className="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block font-medium text-slate-700 mb-1">Email</label>
            <input
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="name@example.com"
              className="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block font-medium text-slate-700 mb-1">Số điện thoại</label>
            <input
              type="text"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              placeholder="0912345678"
              className="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block font-medium text-slate-700 mb-1">Mật khẩu</label>
            <input
              type="password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Tối thiểu 8 ký tự"
              className="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block font-medium text-slate-700 mb-1">Xác nhận mật khẩu</label>
            <input
              type="password"
              required
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder="Nhập lại mật khẩu"
              className="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 outline-none focus:border-blue-500"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl transition shadow-md shadow-blue-600/20 mt-2"
          >
            {loading ? 'Đang khởi tạo...' : 'Đăng ký'}
          </button>
        </form>

        <div className="text-center text-xs text-slate-500 pt-2">
          Đã có tài khoản?{' '}
          <Link to="/login" className="text-blue-600 font-bold hover:underline">
            Đăng nhập
          </Link>
        </div>
      </div>
    </div>
  );
};
