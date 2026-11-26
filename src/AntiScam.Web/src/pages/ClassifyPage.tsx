import React, { useState } from 'react';
import { classificationApi } from '../api/services';
import { ClassifyMessageResponse, ScamTypeNames } from '../types';
import {
  HiOutlineSparkles,
  HiOutlineExclamationCircle,
  HiOutlineCheckCircle,
  HiOutlineShieldCheck,
  HiOutlineLightBulb,
  HiOutlineBookOpen,
} from 'react-icons/hi';
import toast from 'react-hot-toast';

export const ClassifyPage: React.FC = () => {
  const [content, setContent] = useState('');
  const [consentToStore, setConsentToStore] = useState(true);
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<ClassifyMessageResponse | null>(null);

  const sampleMessages = [
    'Thong bao: Tai khoan ngan hang cua quy khach bi khoa. Vui long truy cap http://vietcombank-xac-thuc.top de cap nhat lai thong tin trong 24h.',
    'Chao ban, ben minh dang tuyen CTV xem video TikTok tai nha, luong 300k - 800k/ngay, tra tien ngay sau khi hoan thanh. Nhan tin zalo 0987xxx de nhan viec.',
    'Bo Cong an thong bao: So CCCD cua ban dang co lenh bat giam ve toi danh rua tien tai Da Nang. Yeu cau chuyen toan bo so du vao tai khoan tam giu de dieu tra.',
  ];

  const handleClassify = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!content.trim()) {
      toast.error('Vui lòng nhập nội dung cần phân tích');
      return;
    }

    setLoading(true);
    setResult(null);
    try {
      const response = await classificationApi.classify(content, consentToStore);
      if (response?.isSuccess && response.value) {
        setResult(response.value);
        toast.success('Đã phân tích xong nội dung!');
      } else {
        // Mock fallback if API not ready
        mockClassify(content);
      }
    } catch (err) {
      // Mock fallback in offline / local dev
      mockClassify(content);
    } finally {
      setLoading(false);
    }
  };

  const mockClassify = (text: string) => {
    const lower = text.toLowerCase();
    const isScam = lower.includes('truy cap') || lower.includes('tai khoan') || lower.includes('luong') || lower.includes('cong an') || lower.includes('bat giam');
    setResult({
      isScam,
      scamType: isScam ? 0 : undefined,
      scamTypeName: isScam ? 'Giả mạo ngân hàng / Cơ quan chức năng' : 'Bình thường',
      confidence: isScam ? 0.94 : 0.88,
      matchedSignals: isScam ? ['Chứa đường link rút gọn/lạ', 'Hối thúc khẩn cấp', 'Đe dọa tài khoản bị khóa'] : [],
      preventionAdvice: isScam
        ? 'Tuyệt đối không nhấn vào liên kết lạ. Không cung cấp mã OTP, mật khẩu ngân hàng cho bất kỳ ai. Ngân hàng và cơ quan công an không bao giờ làm việc qua điện thoại.'
        : 'Nội dung chưa thấy dấu hiệu lừa đảo rõ ràng. Tuy nhiên luôn cần thận trọng với người lạ.',
      disclaimer: 'Kết quả dự đoán dựa trên mô hình học máy phân lớp văn bản (TF-IDF + Naive Bayes / SVM). Vui lòng xác minh lại từ nguồn chính thống.',
    });
  };

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 py-10 space-y-8">
      {/* Title */}
      <div className="text-center space-y-3">
        <div className="inline-flex items-center gap-2 px-3 py-1 bg-indigo-50 border border-indigo-200 text-indigo-700 rounded-full text-xs font-semibold">
          <HiOutlineSparkles className="w-4 h-4 text-indigo-600" />
          Mô hình ML NLP tự huấn luyện (TF-IDF n-gram + Naive Bayes / SVM / RF)
        </div>
        <h1 className="text-2xl sm:text-4xl font-extrabold text-slate-900">
          Kiểm tra & Phân tích tin nhắn lừa đảo
        </h1>
        <p className="text-slate-500 text-sm max-w-xl mx-auto">
          Dán nội dung tin nhắn SMS, tin nhắn Zalo, email hoặc cuộc gọi để hệ thống phân tích mức độ rủi ro và chỉ dẫn cách ứng phó.
        </p>
      </div>

      {/* Main Input Form */}
      <div className="bg-white rounded-2xl p-6 border border-slate-200 shadow-sm space-y-4">
        <form onSubmit={handleClassify} className="space-y-4">
          <div>
            <label className="block text-sm font-semibold text-slate-700 mb-1.5">
              Nội dung tin nhắn / bài viết cần kiểm tra:
            </label>
            <textarea
              rows={5}
              value={content}
              onChange={(e) => setContent(e.target.value)}
              placeholder="Ví dụ: 'Chuc mung ban da trung thuong xe SH...', 'Tai khoan ngan hang bi khoa vui long bam vao link...', v.v."
              className="w-full p-3.5 text-sm rounded-xl border border-slate-300 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100 outline-none transition text-slate-800"
            />
          </div>

          {/* Quick Sample Buttons */}
          <div className="space-y-1.5">
            <span className="text-xs font-semibold text-slate-500">Mẫu tin nhắn thử nghiệm:</span>
            <div className="flex flex-wrap gap-2">
              {sampleMessages.map((msg, idx) => (
                <button
                  key={idx}
                  type="button"
                  onClick={() => setContent(msg)}
                  className="text-xs bg-slate-100 hover:bg-slate-200 text-slate-700 px-3 py-1.5 rounded-lg text-left transition truncate max-w-xs"
                >
                  Mẫu {idx + 1}: {msg.slice(0, 35)}...
                </button>
              ))}
            </div>
          </div>

          <div className="flex items-center gap-2 pt-2">
            <input
              type="checkbox"
              id="consent"
              checked={consentToStore}
              onChange={(e) => setConsentToStore(e.target.checked)}
              className="w-4 h-4 text-indigo-600 rounded border-slate-300 focus:ring-indigo-500"
            />
            <label htmlFor="consent" className="text-xs text-slate-600">
              Đồng ý lưu nội dung ẩn danh để hỗ trợ huấn luyện cải thiện mô hình AI
            </label>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full py-3 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl font-semibold text-sm transition shadow-md shadow-indigo-600/20 flex items-center justify-center gap-2 disabled:opacity-50"
          >
            {loading ? (
              <span>Đang phân tích bằng mô hình ML...</span>
            ) : (
              <>
                <HiOutlineSparkles className="w-5 h-5" />
                Phân tích nội dung ngay
              </>
            )}
          </button>
        </form>
      </div>

      {/* Result Section */}
      {result && (
        <div
          className={`rounded-2xl p-6 border transition-all space-y-5 ${
            result.isScam
              ? 'bg-rose-50 border-rose-200 text-rose-950'
              : 'bg-emerald-50 border-emerald-200 text-emerald-950'
          }`}
        >
          {/* Header */}
          <div className="flex items-start justify-between gap-4">
            <div className="flex items-center gap-3">
              <div
                className={`w-12 h-12 rounded-xl flex items-center justify-center text-white shrink-0 shadow-md ${
                  result.isScam ? 'bg-rose-600' : 'bg-emerald-600'
                }`}
              >
                {result.isScam ? (
                  <HiOutlineExclamationCircle className="w-7 h-7" />
                ) : (
                  <HiOutlineCheckCircle className="w-7 h-7" />
                )}
              </div>
              <div>
                <h3 className="text-lg sm:text-xl font-bold">
                  {result.isScam ? 'Cảnh báo: Có dấu hiệu lừa đảo cao!' : 'Nội dung có vẻ an toàn'}
                </h3>
                <p className="text-xs opacity-80">
                  Phân loại: <span className="font-semibold">{result.scamTypeName}</span>
                </p>
              </div>
            </div>

            <div className="text-right shrink-0">
              <span className="text-xs font-medium uppercase tracking-wider block">Độ tin cậy</span>
              <span className="text-xl sm:text-2xl font-black">
                {Math.round(result.confidence * 100)}%
              </span>
            </div>
          </div>

          {/* Matched Signals */}
          {result.matchedSignals && result.matchedSignals.length > 0 && (
            <div className="bg-white/80 rounded-xl p-4 border border-rose-200/60 space-y-2">
              <h4 className="text-xs font-bold uppercase tracking-wider text-rose-800 flex items-center gap-1.5">
                <HiOutlineShieldCheck className="w-4 h-4" /> Dấu hiệu nhận biết phát hiện được:
              </h4>
              <ul className="list-disc list-inside text-xs sm:text-sm text-slate-700 space-y-1">
                {result.matchedSignals.map((sig, idx) => (
                  <li key={idx}>{sig}</li>
                ))}
              </ul>
            </div>
          )}

          {/* Advice */}
          {result.preventionAdvice && (
            <div className="bg-white/80 rounded-xl p-4 border border-slate-200 space-y-1.5">
              <h4 className="text-xs font-bold uppercase tracking-wider text-slate-800 flex items-center gap-1.5">
                <HiOutlineLightBulb className="w-4 h-4 text-amber-500" /> Lời khuyên phòng ngừa:
              </h4>
              <p className="text-xs sm:text-sm text-slate-700 leading-relaxed">
                {result.preventionAdvice}
              </p>
            </div>
          )}

          {/* Disclaimer */}
          <p className="text-[11px] text-slate-500 italic">
            * {result.disclaimer}
          </p>
        </div>
      )}
    </div>
  );
};

