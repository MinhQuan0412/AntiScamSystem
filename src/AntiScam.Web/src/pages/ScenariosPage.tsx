import React, { useState, useEffect } from 'react';
import { scamScenarioApi } from '../api/services';
import { ScamScenarioDto, ScamTypeNames, ScamType } from '../types';
import {
  HiOutlineBookOpen,
  HiOutlineSearch,
  HiOutlineShieldCheck,
  HiOutlineExclamationCircle,
  HiOutlineLightBulb,
  HiOutlineTag,
} from 'react-icons/hi';

export const ScenariosPage: React.FC = () => {
  const [scenarios, setScenarios] = useState<ScamScenarioDto[]>([]);
  const [selectedType, setSelectedType] = useState<number | undefined>(undefined);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedScenario, setSelectedScenario] = useState<ScamScenarioDto | null>(null);

  useEffect(() => {
    // Mock dữ liệu ban đầu nếu backend chưa có DB
    const initialScenarios: ScamScenarioDto[] = [
      {
        id: '1',
        title: 'Giả danh Cơ quan Công an, Viện kiểm sát gọi điện đe dọa bắt giữ',
        scamType: ScamType.AuthorityImpersonation,
        scamTypeName: 'Giả danh cơ quan chức năng',
        description: 'Kẻ lừa đảo gọi điện xưng là cán bộ công an hoặc viện kiểm sát, thông báo số CCCD/tài khoản của nạn nhân liên quan đến đường dây buôn ma túy hoặc rửa tiền xuyên quốc gia.',
        recognitionSigns: [
          'Đe dọa sẽ bắt giam trong 24 giờ tới nếu không hợp tác',
          'Yêu cầu nạn nhân giữ bí mật, không được nói cho người thân',
          'Yêu cầu chuyển toàn bộ tiền vào "tài khoản tạm giữ của cơ quan điều tra"',
          'Gửi lệnh bắt giả mạo qua Zalo/Telegram',
        ],
        anonymizedExample: 'Đối tượng tự xưng là Trung úy Nguyễn Văn Hưng - Công an TP Đà Nẵng gọi điện nói nạn nhân mở tài khoản rửa tiền 2 tỷ đồng...',
        preventionAdvice: 'Cơ quan công an, viện kiểm sát KHÔNG BAO GIỜ làm việc qua điện thoại hay mạng xã hội. Tuyệt đối không chuyển tiền vào bất kỳ tài khoản nào được yêu cầu qua điện thoại.',
        status: 0,
        statusName: 'Đang diễn ra',
        childScenariosCount: 0,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      },
      {
        id: '2',
        title: 'Lừa đảo tuyển Cộng tác viên "Việc nhẹ lương cao" trên mạng xã hội',
        scamType: ScamType.EasyJobHighPay,
        scamTypeName: 'Việc nhẹ lương cao',
        description: 'Đăng tin tuyển dụng làm việc online tại nhà: xem video TikTok, giật đơn hàng Shopee/Lazada, đánh giá ứng dụng với mức thù lao hấp dẫn từ vài trăm nghìn đến tiền triệu mỗi ngày.',
        recognitionSigns: [
          'Không yêu cầu kinh nghiệm hay bằng cấp',
          'Vài nhiệm vụ đầu trả tiền hoa hồng rất nhanh và sòng phẳng để lấy lòng tin',
          'Các nhiệm vụ sau yêu cầu nạp số tiền lớn dần (10 triệu, 50 triệu) để "mở khóa đơn hàng"',
          'Khi đòi rút tiền thì viện cớ lỗi cú pháp, bắt nạp thêm thuế thu nhập',
        ],
        anonymizedExample: 'Nạn nhân được mời vào nhóm Telegram làm nhiệm vụ Like TikTok, nạp 500k nhận 650k, sau đó nạp 50 triệu thì bị khóa nhóm...',
        preventionAdvice: 'Không có công việc nào "việc nhẹ lương cao" không cần kỹ năng mà kiếm tiền triệu mỗi ngày. Cảnh giác với mọi yêu cầu nạp tiền để nhận việc.',
        status: 0,
        statusName: 'Đang diễn ra',
        childScenariosCount: 0,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      },
      {
        id: '3',
        title: 'Lừa đảo đầu tư sàn tài chính, tiền ảo lợi nhuận "khủng"',
        scamType: ScamType.InvestmentFraud,
        scamTypeName: 'Lừa đảo đầu tư',
        description: 'Dụ dỗ nạn nhân tham gia các nhóm kín "kéo vốn", khoe lãi hàng ngày, cam kết lợi nhuận 20% - 50%/tháng.',
        recognitionSigns: [
          'Cam kết hoàn vốn 100% và bảo hiểm tiền gửi',
          'Các "chuyên gia đọc lệnh", "hotgirl tài chính" liên tục khoe nhà xe tiền mặt',
          'Giao diện sàn tự lập, tự chỉnh số dư ảo trên web',
          'Khi muốn rút tiền thì báo lỗi bảo trì, đóng băng tài khoản',
        ],
        preventionAdvice: 'Chỉ đầu tư qua các tổ chức tài chính được Ngân hàng Nhà nước cấp phép. Tránh xa các sàn không rõ nguồn gốc pháp lý.',
        status: 0,
        statusName: 'Đang diễn ra',
        childScenariosCount: 0,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      },
    ];

    setScenarios(initialScenarios);
    setSelectedScenario(initialScenarios[0]);
  }, []);

  const filteredScenarios = scenarios.filter((s) => {
    const matchType = selectedType === undefined || s.scamType === selectedType;
    const matchQuery = !searchQuery || s.title.toLowerCase().includes(searchQuery.toLowerCase()) || s.description.toLowerCase().includes(searchQuery.toLowerCase());
    return matchType && matchQuery;
  });

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 py-10 space-y-8">
      {/* Header */}
      <div>
        <h1 className="text-2xl sm:text-3xl font-extrabold text-slate-900">
          Thư viện kịch bản lừa đảo & Phương án cảnh giác
        </h1>
        <p className="text-slate-500 text-sm mt-1">
          Tổng hợp các chiêu trò, thủ đoạn lừa đảo công nghệ cao thực tế do cơ quan chức năng và cộng đồng ghi nhận.
        </p>
      </div>

      {/* Filter & Search */}
      <div className="flex flex-col sm:flex-row gap-3">
        <div className="relative flex-grow">
          <HiOutlineSearch className="w-5 h-5 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Tìm kiếm thủ đoạn, kịch bản theo từ khóa..."
            className="w-full pl-10 pr-4 py-2.5 rounded-xl border border-slate-300 text-sm outline-none focus:border-blue-500"
          />
        </div>
        <select
          value={selectedType === undefined ? '' : selectedType}
          onChange={(e) => setSelectedType(e.target.value === '' ? undefined : Number(e.target.value))}
          className="p-2.5 rounded-xl border border-slate-300 text-sm outline-none bg-white text-slate-700"
        >
          <option value="">Tất cả hình thức</option>
          {Object.entries(ScamTypeNames).map(([val, name]) => (
            <option key={val} value={val}>{name}</option>
          ))}
        </select>
      </div>

      {/* Main Grid: List + Detail */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left List */}
        <div className="space-y-3">
          {filteredScenarios.map((sc) => (
            <div
              key={sc.id}
              onClick={() => setSelectedScenario(sc)}
              className={`p-4 rounded-xl border cursor-pointer transition ${
                selectedScenario?.id === sc.id
                  ? 'bg-blue-50 border-blue-500 shadow-sm'
                  : 'bg-white border-slate-200 hover:border-slate-300'
              }`}
            >
              <div className="text-xs font-semibold text-blue-600 mb-1">{sc.scamTypeName}</div>
              <h3 className="text-sm font-bold text-slate-800 line-clamp-2">{sc.title}</h3>
              <p className="text-xs text-slate-500 mt-1 line-clamp-2">{sc.description}</p>
            </div>
          ))}
        </div>

        {/* Right Detail Pane */}
        {selectedScenario && (
          <div className="lg:col-span-2 bg-white rounded-2xl border border-slate-200 p-6 space-y-6 shadow-sm">
            <div>
              <span className="inline-block px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-xs font-semibold mb-2">
                {selectedScenario.scamTypeName}
              </span>
              <h2 className="text-xl sm:text-2xl font-black text-slate-900 leading-snug">
                {selectedScenario.title}
              </h2>
            </div>

            {/* Description */}
            <div className="space-y-2">
              <h4 className="text-xs font-bold uppercase tracking-wider text-slate-500">Mô tả thủ đoạn:</h4>
              <p className="text-sm text-slate-700 leading-relaxed">{selectedScenario.description}</p>
            </div>

            {/* Recognition Signs */}
            <div className="bg-amber-50 rounded-xl p-4 border border-amber-200 space-y-2">
              <h4 className="text-xs font-bold uppercase tracking-wider text-amber-900 flex items-center gap-1.5">
                <HiOutlineExclamationCircle className="w-4 h-4 text-amber-600" /> Dấu hiệu nhận biết:
              </h4>
              <ul className="list-disc list-inside text-xs sm:text-sm text-slate-700 space-y-1.5">
                {selectedScenario.recognitionSigns.map((sign, idx) => (
                  <li key={idx}>{sign}</li>
                ))}
              </ul>
            </div>

            {/* Prevention Advice */}
            <div className="bg-emerald-50 rounded-xl p-4 border border-emerald-200 space-y-2">
              <h4 className="text-xs font-bold uppercase tracking-wider text-emerald-900 flex items-center gap-1.5">
                <HiOutlineShieldCheck className="w-4 h-4 text-emerald-600" /> Phương án phòng ngừa & ứng phó:
              </h4>
              <p className="text-xs sm:text-sm text-slate-700 leading-relaxed font-medium">
                {selectedScenario.preventionAdvice}
              </p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

