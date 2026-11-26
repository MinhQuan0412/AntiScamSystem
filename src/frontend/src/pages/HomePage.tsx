import { Link } from 'react-router-dom'
import { Search, Phone, BookOpen, Map, AlertTriangle, Shield, ArrowRight } from 'lucide-react'

const features = [
  { to: '/classify', icon: Search,    color: 'bg-red-50 text-red-600',    title: 'Kiểm tra tin nhắn',     desc: 'Dán nội dung nghi ngờ — AI phân tích ngay' },
  { to: '/lookup',   icon: Phone,     color: 'bg-orange-50 text-orange-600', title: 'Tra cứu số điện thoại', desc: 'Kiểm tra SĐT / STK bị báo cáo lừa đảo' },
  { to: '/scenarios',icon: BookOpen,  color: 'bg-blue-50 text-blue-600',  title: 'Thư viện kịch bản',     desc: 'Nhận biết các chiêu trò lừa đảo phổ biến' },
  { to: '/emergency',icon: AlertTriangle, color: 'bg-yellow-50 text-yellow-600', title: 'Số khẩn cấp',   desc: '113, 114, 115 và số theo địa phương' },
  { to: '/travel',   icon: Map,       color: 'bg-green-50 text-green-600', title: 'An toàn du lịch',      desc: 'Lộ trình + cảnh báo lệch tuyến + SOS' },
]

export default function HomePage() {
  return (
    <div className="space-y-10">
      {/* Hero */}
      <section className="text-center py-10 md:py-16">
        <div className="flex justify-center mb-4">
          <div className="bg-red-100 p-4 rounded-full">
            <Shield className="w-12 h-12 text-red-600" />
          </div>
        </div>
        <h1 className="text-3xl md:text-4xl font-bold text-gray-900 mb-3">
          Hệ thống Cảnh báo Lừa đảo
        </h1>
        <p className="text-gray-500 text-base md:text-lg max-w-xl mx-auto mb-8">
          Kiểm tra tin nhắn nghi ngờ, tra cứu số điện thoại bị báo cáo và nhận cảnh báo an toàn khi đi du lịch.
        </p>

        {/* Quick check form */}
        <div className="max-w-xl mx-auto">
          <form onSubmit={e => { e.preventDefault(); const q = (e.currentTarget.querySelector('input') as HTMLInputElement).value; window.location.href = `/classify?q=${encodeURIComponent(q)}` }}
            className="flex gap-2">
            <input className="input flex-1 text-base py-3" placeholder="Dán tin nhắn nghi ngờ vào đây..." />
            <button type="submit" className="btn-primary py-3 px-5 whitespace-nowrap">
              <Search className="w-4 h-4" /> Kiểm tra
            </button>
          </form>
          <p className="text-xs text-gray-400 mt-2">Không cần đăng nhập · Miễn phí · Bảo mật</p>
        </div>
      </section>

      {/* Feature cards */}
      <section>
        <h2 className="text-xl font-bold text-gray-800 mb-4">Tính năng chính</h2>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {features.map(({ to, icon: Icon, color, title, desc }) => (
            <Link key={to} to={to}
              className="card hover:shadow-md transition-shadow flex items-start gap-4 group">
              <div className={`${color} p-3 rounded-xl shrink-0`}>
                <Icon className="w-6 h-6" />
              </div>
              <div className="flex-1 min-w-0">
                <h3 className="font-semibold text-gray-800 group-hover:text-red-600 transition-colors">{title}</h3>
                <p className="text-sm text-gray-500 mt-0.5">{desc}</p>
              </div>
              <ArrowRight className="w-4 h-4 text-gray-300 group-hover:text-red-500 shrink-0 mt-1 transition-colors" />
            </Link>
          ))}
        </div>
      </section>

      {/* Warning banner */}
      <section className="bg-red-50 border border-red-200 rounded-xl p-5 flex gap-4">
        <AlertTriangle className="w-6 h-6 text-red-500 shrink-0 mt-0.5" />
        <div>
          <p className="font-semibold text-red-800">Lưu ý quan trọng</p>
          <p className="text-sm text-red-700 mt-1">
            Kết quả phân tích chỉ mang tính tham khảo và cảnh báo ban đầu.
            Luôn xác minh trực tiếp với ngân hàng hoặc cơ quan công an trước khi thực hiện bất kỳ giao dịch nào.
          </p>
        </div>
      </section>
    </div>
  )
}
