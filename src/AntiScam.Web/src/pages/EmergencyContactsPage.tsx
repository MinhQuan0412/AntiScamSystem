import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import {
  HiOutlinePhone,
  HiOutlineUserGroup,
  HiOutlinePlus,
  HiOutlineTrash,
  HiOutlineShieldCheck,
  HiOutlineExclamation,
} from 'react-icons/hi';
import toast from 'react-hot-toast';

export const EmergencyContactsPage: React.FC = () => {
  const { isAuthenticated } = useAuth();

  // Danh bạ khẩn cấp quốc gia chính thức
  const nationalHotlines = [
    { name: 'Cảnh sát phản ứng nhanh (Công an)', phone: '113', desc: 'Tiếp nhận tố giác tội phạm, khẩn cấp 24/7', color: 'bg-rose-50 border-rose-200 text-rose-700' },
    { name: 'Cứu hỏa & Cứu hộ cứu nạn', phone: '114', desc: 'Ứng phó sự cố cháy nổ, cứu hộ khẩn cấp', color: 'bg-amber-50 border-amber-200 text-amber-700' },
    { name: 'Cấp cứu y tế', phone: '115', desc: 'Vận chuyển cấp cứu y tế', color: 'bg-emerald-50 border-emerald-200 text-emerald-700' },
    { name: 'Tổng đài 156 / 5656', phone: '156', desc: 'Tiếp nhận phản ánh tin nhắn rác, cuộc gọi lừa đảo (Cục Viễn thông)', color: 'bg-blue-50 border-blue-200 text-blue-700' },
    { name: 'Cục An ninh mạng (A05 - Bộ Công an)', phone: '069.234.8560', desc: 'Phòng chống tội phạm sử dụng công nghệ cao', color: 'bg-indigo-50 border-indigo-200 text-indigo-700' },
  ];

  // Danh bạ người thân nhận SOS
  interface RelativeItem {
    id: string;
    name: string;
    phone: string;
    type: string;
    email?: string;
  }

  const [relatives, setRelatives] = useState<RelativeItem[]>([
    { id: '1', name: 'Bố (Nguyễn Văn Minh)', phone: '0988112233', type: 'Phụ huynh', email: 'minh.nguyen@example.com' },
    { id: '2', name: 'Mẹ (Trần Thị Lan)', phone: '0977223344', type: 'Phụ huynh', email: 'lan.tran@example.com' },
  ]);

  const [newName, setNewName] = useState('');
  const [newPhone, setNewPhone] = useState('');
  const [newEmail, setNewEmail] = useState('');
  const [newType, setNewType] = useState('Người thân');
  const [showAddModal, setShowAddModal] = useState(false);

  const handleAddRelative = (e: React.FormEvent) => {
    e.preventDefault();
    if (!newName.trim() || !newPhone.trim()) {
      toast.error('Vui lòng nhập tên và số điện thoại người thân');
      return;
    }

    const newItem = {
      id: Date.now().toString(),
      name: newName.trim(),
      phone: newPhone.trim(),
      email: newEmail.trim() || undefined,
      type: newType,
    };

    setRelatives([...relatives, newItem]);
    toast.success('Đã thêm người thân vào danh bạ nhận SOS!');
    setShowAddModal(false);
    setNewName('');
    setNewPhone('');
    setNewEmail('');
  };

  const handleDeleteRelative = (id: string) => {
    setRelatives(relatives.filter((r) => r.id !== id));
    toast.success('Đã xóa người thân khỏi danh bạ SOS');
  };

  return (
    <div className="max-w-5xl mx-auto px-4 sm:px-6 py-10 space-y-10">
      {/* Header */}
      <div>
        <h1 className="text-2xl sm:text-3xl font-extrabold text-slate-900">
          Danh bạ khẩn cấp & Thiết lập người thân SOS
        </h1>
        <p className="text-slate-500 text-sm mt-1">
          Số điện thoại cơ quan công an, đường dây nóng phòng chống lừa đảo và danh sách người thân tự động nhận cảnh báo khi có sự cố.
        </p>
      </div>

      {/* Official Hotlines */}
      <section className="space-y-4">
        <h2 className="text-lg font-bold text-slate-900 flex items-center gap-2">
          <HiOutlineShieldCheck className="w-5 h-5 text-blue-600" />
          Đường dây nóng cơ quan chức năng chính thức
        </h2>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {nationalHotlines.map((hotline, idx) => (
            <div key={idx} className={`p-4 rounded-2xl border ${hotline.color} flex flex-col justify-between space-y-3`}>
              <div>
                <h3 className="font-bold text-slate-800 text-sm">{hotline.name}</h3>
                <p className="text-xs text-slate-600 mt-1">{hotline.desc}</p>
              </div>
              <div className="flex items-center justify-between pt-2 border-t border-slate-200/50">
                <span className="font-mono text-lg font-black tracking-wide">{hotline.phone}</span>
                <a
                  href={`tel:${hotline.phone}`}
                  className="bg-white px-3 py-1.5 rounded-lg text-xs font-bold shadow-sm hover:bg-slate-50 transition border flex items-center gap-1"
                >
                  <HiOutlinePhone className="w-3.5 h-3.5" /> Gọi ngay
                </a>
              </div>
            </div>
          ))}
        </div>
      </section>

      {/* Relatives for SOS */}
      <section className="space-y-4 pt-4 border-t border-slate-200">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-lg font-bold text-slate-900 flex items-center gap-2">
              <HiOutlineUserGroup className="w-5 h-5 text-emerald-600" />
              Người thân nhận thông báo khẩn cấp (SOS & Lệch tuyến)
            </h2>
            <p className="text-xs text-slate-500 mt-0.5">
              Khi bạn phát tín hiệu SOS hoặc đi lệch lộ trình quá thời gian đếm ngược, hệ thống sẽ tự động gửi Email/Telegram kèm vị trí tới danh sách này.
            </p>
          </div>

          <button
            onClick={() => {
              if (!isAuthenticated) {
                toast.error('Vui lòng đăng nhập để quản lý người thân SOS');
                return;
              }
              setShowAddModal(true);
            }}
            className="bg-emerald-600 hover:bg-emerald-700 text-white px-3.5 py-2 rounded-xl text-xs font-bold transition flex items-center gap-1.5 shadow-sm shrink-0"
          >
            <HiOutlinePlus className="w-4 h-4" /> Thêm người thân
          </button>
        </div>

        <div className="bg-white rounded-2xl border border-slate-200 divide-y divide-slate-100 overflow-hidden shadow-sm">
          {relatives.map((rel) => (
            <div key={rel.id} className="p-4 flex items-center justify-between gap-4">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-full bg-emerald-100 text-emerald-700 font-bold flex items-center justify-center text-sm shrink-0">
                  {rel.name.slice(0, 1)}
                </div>
                <div>
                  <div className="font-bold text-slate-800 text-sm flex items-center gap-2">
                    {rel.name}
                    <span className="text-[10px] bg-slate-100 text-slate-600 px-2 py-0.5 rounded-full font-normal">
                      {rel.type}
                    </span>
                  </div>
                  <div className="text-xs text-slate-500 mt-0.5 flex gap-3">
                    <span>SĐT: <strong className="font-mono text-slate-700">{rel.phone}</strong></span>
                    {rel.email && <span>Email: {rel.email}</span>}
                  </div>
                </div>
              </div>

              <button
                onClick={() => handleDeleteRelative(rel.id)}
                className="p-2 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition"
                title="Xóa người thân"
              >
                <HiOutlineTrash className="w-4 h-4" />
              </button>
            </div>
          ))}
        </div>
      </section>

      {/* Modal Add Relative */}
      {showAddModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
          <div className="bg-white rounded-2xl max-w-md w-full p-6 shadow-2xl space-y-4">
            <div className="flex items-center justify-between pb-3 border-b">
              <h3 className="text-base font-bold text-slate-900">Thêm người thân nhận tin khẩn cấp</h3>
              <button onClick={() => setShowAddModal(false)} className="text-slate-400 font-bold">✕</button>
            </div>

            <form onSubmit={handleAddRelative} className="space-y-3.5 text-sm">
              <div>
                <label className="block font-medium text-slate-700 mb-1">Họ và tên</label>
                <input
                  type="text"
                  required
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  placeholder="VD: Bố Nguyễn Văn A"
                  className="w-full p-2.5 rounded-lg border border-slate-300 outline-none"
                />
              </div>

              <div>
                <label className="block font-medium text-slate-700 mb-1">Số điện thoại</label>
                <input
                  type="text"
                  required
                  value={newPhone}
                  onChange={(e) => setNewPhone(e.target.value)}
                  placeholder="0912345678"
                  className="w-full p-2.5 rounded-lg border border-slate-300 outline-none"
                />
              </div>

              <div>
                <label className="block font-medium text-slate-700 mb-1">Email (để nhận tọa độ bản đồ)</label>
                <input
                  type="email"
                  value={newEmail}
                  onChange={(e) => setNewEmail(e.target.value)}
                  placeholder="nguoithan@gmail.com"
                  className="w-full p-2.5 rounded-lg border border-slate-300 outline-none"
                />
              </div>

              <div>
                <label className="block font-medium text-slate-700 mb-1">Mối quan hệ</label>
                <input
                  type="text"
                  value={newType}
                  onChange={(e) => setNewType(e.target.value)}
                  placeholder="Bố mẹ, vợ/chồng, bạn bè..."
                  className="w-full p-2.5 rounded-lg border border-slate-300 outline-none"
                />
              </div>

              <div className="pt-2 flex gap-3">
                <button
                  type="button"
                  onClick={() => setShowAddModal(false)}
                  className="w-1/2 py-2 rounded-xl border border-slate-300 text-slate-700"
                >
                  Hủy
                </button>
                <button
                  type="submit"
                  className="w-1/2 py-2 rounded-xl bg-emerald-600 hover:bg-emerald-700 text-white font-bold"
                >
                  Lưu liên hệ
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
