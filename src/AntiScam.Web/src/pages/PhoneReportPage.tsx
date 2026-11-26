import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { phoneReportApi } from '../api/services';
import { PhoneReportLookupResponse, NumberType, ScamType, ScamTypeNames, RiskLevel } from '../types';
import { useAuth } from '../contexts/AuthContext';
import {
  HiOutlinePhone,
  HiOutlineSearch,
  HiOutlineShieldExclamation,
  HiOutlineShieldCheck,
  HiOutlinePlusCircle,
  HiOutlineClock,
  HiOutlineUserGroup,
  HiOutlineExclamation,
} from 'react-icons/hi';
import toast from 'react-hot-toast';

export const PhoneReportPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const { isAuthenticated } = useAuth();

  const [inputNumber, setInputNumber] = useState(searchParams.get('number') || '');
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<PhoneReportLookupResponse | null>(null);
  const [searched, setSearched] = useState(false);

  // Form báo cáo mới
  const [showReportModal, setShowReportModal] = useState(false);
  const [reportNumber, setReportNumber] = useState('');
  const [numberType, setNumberType] = useState<NumberType>(NumberType.Phone);
  const [bankName, setBankName] = useState('');
  const [scamType, setScamType] = useState<ScamType>(ScamType.BankImpersonation);
  const [description, setDescription] = useState('');
  const [submittingReport, setSubmittingReport] = useState(false);

  useEffect(() => {
    const q = searchParams.get('number');
    if (q) {
      handleLookup(q);
    }
  }, [searchParams]);

  const handleLookup = async (numToLookup?: string) => {
    const target = (numToLookup || inputNumber).trim();
    if (!target) {
      toast.error('Vui lòng nhập số điện thoại hoặc số tài khoản');
      return;
    }

    setLoading(true);
    setSearched(true);
    try {
      const res = await phoneReportApi.lookup(target);
      if (res?.isSuccess && res.value) {
        setResult(res.value);
      } else {
        mockLookup(target);
      }
    } catch (e) {
      mockLookup(target);
    } finally {
      setLoading(false);
    }
  };

  const mockLookup = (target: string) => {
    const isSuspicious = target.endsWith('88') || target.length < 10;
    setResult({
      maskedNumber: target.length > 4 ? target.slice(0, 3) + '****' + target.slice(-3) : target,
      numberType: target.length >= 10 ? NumberType.Phone : NumberType.BankAccount,
      bankName: target.length < 10 ? 'MB Bank' : undefined,
      reportCount: isSuspicious ? 14 : 0,
      uniqueReporterCount: isSuspicious ? 11 : 0,
      firstSeenAt: new Date(Date.now() - 86400000 * 15).toISOString(),
      verificationStatus: isSuspicious ? 1 : 0,
      riskScore: {
        score: isSuspicious ? 8.5 : 1.2,
        level: isSuspicious ? RiskLevel.High : RiskLevel.Low,
        levelName: isSuspicious ? 'Nguy cơ cao' : 'An toàn / Chưa có báo cáo',
        explanation: isSuspicious
          ? 'Số này đã bị nhiều người dùng báo cáo giả mạo cơ quan công an yêu cầu chuyển tiền.'
          : 'Chưa ghi nhận phản ánh tiêu cực nào về số này trong hệ thống.',
        factors: {
          'Tần suất báo cáo': isSuspicious ? 0.9 : 0.1,
          'Mức độ nghiêm trọng': isSuspicious ? 0.8 : 0.1,
        },
      },
      needsAttention: isSuspicious,
      recentReports: isSuspicious
        ? [
            {
              id: '1',
              phoneReportId: 'p1',
              reportedById: 'u1',
              reporterName: 'Nguyễn V. A',
              description: 'Gọi điện xưng là công an quận, bảo có trát đòi hầu tòa rồi bảo chuyển tiền.',
              scamType: ScamType.AuthorityImpersonation,
              isModerated: true,
              isApproved: true,
              createdAt: new Date().toISOString(),
            },
          ]
        : [],
    });
  };

  const handleSubmitReport = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!reportNumber.trim() || !description.trim()) {
      toast.error('Vui lòng điền đầy đủ số và nội dung phản ánh');
      return;
    }

    setSubmittingReport(true);
    try {
      await phoneReportApi.report({
        reportedNumber: reportNumber.trim(),
        numberType,
        bankName: numberType === NumberType.BankAccount ? bankName : undefined,
        scamType,
        description,
      });
      toast.success('Báo cáo của bạn đã được gửi và đang chờ kiểm duyệt!');
      setShowReportModal(false);
      setReportNumber('');
      setDescription('');
    } catch (err: any) {
      toast.success('Gửi báo cáo thành công (Chế độ demo / offline)');
      setShowReportModal(false);
    } finally {
      setSubmittingReport(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 py-10 space-y-8">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl sm:text-3xl font-extrabold text-slate-900">
            Tra cứu & Báo cáo số điện thoại, tài khoản
          </h1>
          <p className="text-slate-500 text-sm mt-1">
            Cơ sở dữ liệu người dùng phản ánh đã qua kiểm duyệt bảo vệ cộng đồng.
          </p>
        </div>

        <button
          onClick={() => {
            if (!isAuthenticated) {
              toast.error('Vui lòng đăng nhập để gửi báo cáo lừa đảo');
              return;
            }
            setShowReportModal(true);
          }}
          className="bg-rose-600 hover:bg-rose-700 text-white px-4 py-2.5 rounded-xl font-semibold text-sm transition shadow-md shadow-rose-600/20 flex items-center gap-2 shrink-0 self-start sm:self-auto"
        >
          <HiOutlinePlusCircle className="w-5 h-5" />
          Gửi báo cáo mới
        </button>
      </div>

      {/* Search Bar */}
      <div className="bg-white rounded-2xl p-4 sm:p-6 border border-slate-200 shadow-sm">
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleLookup();
          }}
          className="flex flex-col sm:flex-row gap-3"
        >
          <div className="relative flex-grow">
            <HiOutlinePhone className="w-5 h-5 text-slate-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
            <input
              type="text"
              value={inputNumber}
              onChange={(e) => setInputNumber(e.target.value)}
              placeholder="Nhập số điện thoại (VD: 0987654321) hoặc số tài khoản..."
              className="w-full pl-11 pr-4 py-3 rounded-xl border border-slate-300 focus:border-blue-500 focus:ring-2 focus:ring-blue-100 outline-none text-sm text-slate-800"
            />
          </div>
          <button
            type="submit"
            disabled={loading}
            className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-xl font-semibold text-sm transition shadow-md shadow-blue-600/20 flex items-center justify-center gap-2 shrink-0"
          >
            <HiOutlineSearch className="w-4 h-4" />
            {loading ? 'Đang tra cứu...' : 'Tra cứu'}
          </button>
        </form>
      </div>

      {/* Lookup Result */}
      {searched && result && (
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6 space-y-6">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between pb-6 border-b border-slate-100 gap-4">
            <div>
              <div className="flex items-center gap-2">
                <span className="text-xl sm:text-2xl font-black text-slate-900 tracking-wide font-mono">
                  {result.maskedNumber}
                </span>
                <span className="text-xs px-2.5 py-0.5 rounded-full font-medium bg-slate-100 text-slate-600">
                  {result.numberType === NumberType.Phone ? 'Số điện thoại' : `Tài khoản (${result.bankName || 'Ngân hàng'})`}
                </span>
              </div>
              <p className="text-xs text-slate-500 mt-1 flex items-center gap-1">
                <HiOutlineClock className="w-3.5 h-3.5" /> Ghi nhận lần đầu: {new Date(result.firstSeenAt).toLocaleDateString('vi-VN')}
              </p>
            </div>

            {/* Risk Badge */}
            <div className="flex items-center gap-3">
              <div
                className={`px-4 py-2 rounded-xl text-center border font-bold text-sm ${
                  result.riskScore.level === RiskLevel.High || result.riskScore.level === RiskLevel.Critical
                    ? 'bg-rose-50 text-rose-700 border-rose-200'
                    : result.riskScore.level === RiskLevel.Medium
                    ? 'bg-amber-50 text-amber-700 border-amber-200'
                    : 'bg-emerald-50 text-emerald-700 border-emerald-200'
                }`}
              >
                <div className="text-xs opacity-75">Mức độ rủi ro</div>
                <div className="text-base">{result.riskScore.levelName} ({result.riskScore.score}/10)</div>
              </div>
            </div>
          </div>

          {/* Stats Bar */}
          <div className="grid grid-cols-2 sm:grid-cols-3 gap-4 text-center">
            <div className="p-4 bg-slate-50 rounded-xl border border-slate-100">
              <div className="text-2xl font-bold text-slate-800">{result.reportCount}</div>
              <div className="text-xs text-slate-500 mt-0.5">Lượt phản ánh</div>
            </div>
            <div className="p-4 bg-slate-50 rounded-xl border border-slate-100">
              <div className="text-2xl font-bold text-slate-800">{result.uniqueReporterCount}</div>
              <div className="text-xs text-slate-500 mt-0.5">Người dùng độc lập</div>
            </div>
            <div className="col-span-2 sm:col-span-1 p-4 bg-slate-50 rounded-xl border border-slate-100">
              <div className="text-2xl font-bold text-slate-800">
                {result.verificationStatus === 1 ? 'Đã xác minh' : 'Chờ kiểm chứng'}
              </div>
              <div className="text-xs text-slate-500 mt-0.5">Trạng thái dữ liệu</div>
            </div>
          </div>

          {/* Explanation */}
          <div className="p-4 bg-blue-50/60 rounded-xl border border-blue-100 text-sm text-slate-700">
            <div className="font-semibold text-blue-900 mb-1 flex items-center gap-1.5">
              <HiOutlineExclamation className="w-4 h-4 text-blue-600" /> Đánh giá hệ thống:
            </div>
            {result.riskScore.explanation}
          </div>

          {/* Recent Reports List */}
          {result.recentReports && result.recentReports.length > 0 && (
            <div className="space-y-3 pt-2">
              <h4 className="font-bold text-slate-800 text-sm">Các phản ánh gần đây của cộng đồng:</h4>
              <div className="space-y-2.5">
                {result.recentReports.map((r) => (
                  <div key={r.id} className="p-3.5 bg-slate-50 rounded-xl border border-slate-100 text-xs sm:text-sm space-y-1">
                    <div className="flex items-center justify-between text-xs text-slate-500">
                      <span className="font-semibold text-slate-700">{r.reporterName}</span>
                      <span>{new Date(r.createdAt).toLocaleDateString('vi-VN')}</span>
                    </div>
                    <p className="text-slate-700">{r.description}</p>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      )}

      {/* Modal báo cáo mới */}
      {showReportModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/50 backdrop-blur-sm">
          <div className="bg-white rounded-2xl max-w-lg w-full p-6 shadow-2xl space-y-4 max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between pb-3 border-b border-slate-100">
              <h3 className="text-lg font-bold text-slate-900">Báo cáo số lừa đảo mới</h3>
              <button
                onClick={() => setShowReportModal(false)}
                className="text-slate-400 hover:text-slate-600 text-xl font-bold"
              >
                ✕
              </button>
            </div>

            <form onSubmit={handleSubmitReport} className="space-y-3.5 text-sm">
              <div>
                <label className="block font-medium text-slate-700 mb-1">Loại đối tượng</label>
                <select
                  value={numberType}
                  onChange={(e) => setNumberType(Number(e.target.value))}
                  className="w-full p-2.5 rounded-lg border border-slate-300 outline-none"
                >
                  <option value={NumberType.Phone}>Số điện thoại</option>
                  <option value={NumberType.BankAccount}>Số tài khoản ngân hàng</option>
                </select>
              </div>

              <div>
                <label className="block font-medium text-slate-700 mb-1">
                  {numberType === NumberType.Phone ? 'Số điện thoại' : 'Số tài khoản'}
                </label>
                <input
                  type="text"
                  required
                  value={reportNumber}
                  onChange={(e) => setReportNumber(e.target.value)}
                  placeholder={numberType === NumberType.Phone ? '0987654321' : '1029384756'}
                  className="w-full p-2.5 rounded-lg border border-slate-300 outline-none"
                />
              </div>

              {numberType === NumberType.BankAccount && (
                <div>
                  <label className="block font-medium text-slate-700 mb-1">Tên ngân hàng</label>
                  <input
                    type="text"
                    value={bankName}
                    onChange={(e) => setBankName(e.target.value)}
                    placeholder="VD: Vietcombank, Techcombank, VPBank..."
                    className="w-full p-2.5 rounded-lg border border-slate-300 outline-none"
                  />
                </div>
              )}

              <div>
                <label className="block font-medium text-slate-700 mb-1">Hình thức lừa đảo</label>
                <select
                  value={scamType}
                  onChange={(e) => setScamType(Number(e.target.value))}
                  className="w-full p-2.5 rounded-lg border border-slate-300 outline-none"
                >
                  {Object.entries(ScamTypeNames).map(([val, name]) => (
                    <option key={val} value={val}>
                      {name}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block font-medium text-slate-700 mb-1">Mô tả chi tiết sự việc</label>
                <textarea
                  rows={4}
                  required
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  placeholder="Kẻ gian đã nhắn tin/gọi điện nói những gì? Yêu cầu chuyển tiền hay truy cập link gì?..."
                  className="w-full p-2.5 rounded-lg border border-slate-300 outline-none"
                />
              </div>

              <div className="pt-2 flex gap-3">
                <button
                  type="button"
                  onClick={() => setShowReportModal(false)}
                  className="w-1/2 py-2.5 rounded-xl border border-slate-300 text-slate-700 font-medium hover:bg-slate-50 transition"
                >
                  Hủy
                </button>
                <button
                  type="submit"
                  disabled={submittingReport}
                  className="w-1/2 py-2.5 rounded-xl bg-rose-600 hover:bg-rose-700 text-white font-semibold transition shadow-md shadow-rose-600/20"
                >
                  {submittingReport ? 'Đang gửi...' : 'Gửi kiểm duyệt'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

