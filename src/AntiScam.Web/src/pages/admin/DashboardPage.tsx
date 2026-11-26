import React from 'react';
import { useAuth } from '../../contexts/AuthContext';
import {
  HiOutlineDocumentDownload,
  HiOutlineShieldCheck,
  HiOutlineUsers,
  HiOutlineExclamationCircle,
  HiOutlineChip,
} from 'react-icons/hi';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  LineChart,
  Line,
} from 'recharts';
import toast from 'react-hot-toast';

export const AdminDashboardPage: React.FC = () => {
  const { user } = useAuth();

  // Dữ liệu mẫu thống kê báo cáo theo tháng
  const monthlyData = [
    { month: 'T5/2026', count: 45 },
    { month: 'T6/2026', count: 78 },
    { month: 'T7/2026', count: 112 },
    { month: 'T8/2026', count: 165 },
    { month: 'T9/2026', count: 210 },
    { month: 'T10/2026', count: 280 },
    { month: 'T11/2026', count: 320 },
  ];

  // Phân bố các loại hình thức lừa đảo
  const scamDistribution = [
    { name: 'Giả mạo ngân hàng', value: 35, color: '#3b82f6' },
    { name: 'Việc nhẹ lương cao', value: 25, color: '#10b981' },
    { name: 'Giả danh công an', value: 20, color: '#ef4444' },
    { name: 'Đầu tư tài chính', value: 12, color: '#f59e0b' },
    { name: 'Lừa tình cảm & Khác', value: 8, color: '#8b5cf6' },
  ];

  // Các phiên bản mô hình ML được huấn luyện
  const modelVersions = [
    { name: 'v1.0 (Naive Bayes Baseline)', acc: '88.4%', prec: '86.2%', rec: '89.1%', f1: '87.6%', status: 'Active' },
    { name: 'v1.1 (Linear SVM + TF-IDF)', acc: '92.5%', prec: '91.8%', rec: '93.0%', f1: '92.4%', status: 'Active' },
    { name: 'v1.2 (Random Forest Classifier)', acc: '94.1%', prec: '93.7%', rec: '94.5%', f1: '94.1%', status: 'Testing' },
  ];

  const handleExport = (type: 'excel' | 'pdf') => {
    toast.success(`Đang tạo và tải file báo cáo ${type.toUpperCase()} (ClosedXML/QuestPDF)...`);
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 py-10 space-y-8">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <div className="inline-flex items-center gap-1.5 px-3 py-1 bg-purple-50 text-purple-700 border border-purple-200 rounded-full text-xs font-semibold mb-1">
            <HiOutlineShieldCheck className="w-4 h-4" />
            Bảng điều khiển Quản trị viên
          </div>
          <h1 className="text-2xl sm:text-3xl font-extrabold text-slate-900">
            Tổng quan hệ thống & Thống kê lừa đảo
          </h1>
          <p className="text-slate-500 text-sm">
            Theo dõi xu hướng các vụ việc, tỷ lệ kiểm duyệt và chất lượng các phiên bản mô hình học máy.
          </p>
        </div>

        {/* Export Buttons */}
        <div className="flex items-center gap-2">
          <button
            onClick={() => handleExport('excel')}
            className="bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-2 rounded-xl text-xs font-bold transition shadow-sm flex items-center gap-1.5"
          >
            <HiOutlineDocumentDownload className="w-4 h-4" /> Xuất Excel
          </button>
          <button
            onClick={() => handleExport('pdf')}
            className="bg-rose-600 hover:bg-rose-700 text-white px-4 py-2 rounded-xl text-xs font-bold transition shadow-sm flex items-center gap-1.5"
          >
            <HiOutlineDocumentDownload className="w-4 h-4" /> Xuất PDF
          </button>
        </div>
      </div>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
        <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm flex items-center justify-between">
          <div>
            <div className="text-xs text-slate-500 font-medium">Tổng số phản ánh</div>
            <div className="text-2xl font-black text-slate-900 mt-1">1,210</div>
            <div className="text-[11px] text-emerald-600 font-semibold mt-0.5">↑ +18% so với tháng trước</div>
          </div>
          <div className="w-12 h-12 rounded-xl bg-blue-50 text-blue-600 flex items-center justify-center">
            <HiOutlineExclamationCircle className="w-6 h-6" />
          </div>
        </div>

        <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm flex items-center justify-between">
          <div>
            <div className="text-xs text-slate-500 font-medium">Chờ kiểm duyệt</div>
            <div className="text-2xl font-black text-amber-600 mt-1">28</div>
            <div className="text-[11px] text-slate-500 mt-0.5">Cần xử lý trong ngày</div>
          </div>
          <div className="w-12 h-12 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center">
            <HiOutlineShieldCheck className="w-6 h-6" />
          </div>
        </div>

        <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm flex items-center justify-between">
          <div>
            <div className="text-xs text-slate-500 font-medium">Độ chính xác AI (F1-score)</div>
            <div className="text-2xl font-black text-indigo-600 mt-1">94.1%</div>
            <div className="text-[11px] text-indigo-600 mt-0.5">Model v1.2 (Random Forest)</div>
          </div>
          <div className="w-12 h-12 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center">
            <HiOutlineChip className="w-6 h-6" />
          </div>
        </div>

        <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm flex items-center justify-between">
          <div>
            <div className="text-xs text-slate-500 font-medium">Lộ trình an toàn bật</div>
            <div className="text-2xl font-black text-emerald-600 mt-1">142</div>
            <div className="text-[11px] text-emerald-600 mt-0.5">Không có sự cố SOS hôm nay</div>
          </div>
          <div className="w-12 h-12 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center">
            <HiOutlineUsers className="w-6 h-6" />
          </div>
        </div>
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Trend Bar Chart */}
        <div className="lg:col-span-2 bg-white p-6 rounded-2xl border border-slate-200 shadow-sm space-y-4">
          <h3 className="font-bold text-slate-800 text-sm">Xu hướng phản ánh lừa đảo qua các tháng</h3>
          <div className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={monthlyData}>
                <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f1f5f9" />
                <XAxis dataKey="month" stroke="#94a3b8" fontSize={12} tickLine={false} />
                <YAxis stroke="#94a3b8" fontSize={12} tickLine={false} />
                <Tooltip
                  contentStyle={{ backgroundColor: '#1e293b', borderRadius: '8px', color: '#fff', fontSize: '12px' }}
                />
                <Bar dataKey="count" fill="#3b82f6" radius={[6, 6, 0, 0]} name="Số vụ việc" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Pie Chart */}
        <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm space-y-4">
          <h3 className="font-bold text-slate-800 text-sm">Cơ cấu hình thức lừa đảo</h3>
          <div className="h-48">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={scamDistribution}
                  cx="50%"
                  cy="50%"
                  innerRadius={50}
                  outerRadius={75}
                  paddingAngle={4}
                  dataKey="value"
                >
                  {scamDistribution.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </div>
          <div className="space-y-1.5 text-xs text-slate-600">
            {scamDistribution.map((item, idx) => (
              <div key={idx} className="flex items-center justify-between">
                <span className="flex items-center gap-1.5">
                  <span className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: item.color }} />
                  {item.name}
                </span>
                <span className="font-bold">{item.value}%</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* ML Model Performance Table */}
      <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm space-y-4">
        <h3 className="font-bold text-slate-800 text-sm">Hiệu năng các mô hình phân lớp văn bản (ML Models)</h3>
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs sm:text-sm">
            <thead>
              <tr className="border-b border-slate-100 text-slate-500">
                <th className="pb-3 font-semibold">Tên phiên bản & Thuật toán</th>
                <th className="pb-3 font-semibold">Accuracy</th>
                <th className="pb-3 font-semibold">Precision</th>
                <th className="pb-3 font-semibold">Recall</th>
                <th className="pb-3 font-semibold">F1-Score</th>
                <th className="pb-3 font-semibold">Trạng thái</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 text-slate-700">
              {modelVersions.map((m, idx) => (
                <tr key={idx} className="hover:bg-slate-50">
                  <td className="py-3 font-medium text-slate-900">{m.name}</td>
                  <td className="py-3">{m.acc}</td>
                  <td className="py-3">{m.prec}</td>
                  <td className="py-3">{m.rec}</td>
                  <td className="py-3 font-bold text-blue-600">{m.f1}</td>
                  <td className="py-3">
                    <span
                      className={`px-2 py-0.5 rounded-full text-xs font-semibold ${
                        m.status === 'Active'
                          ? 'bg-emerald-100 text-emerald-800'
                          : 'bg-amber-100 text-amber-800'
                      }`}
                    >
                      {m.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};
