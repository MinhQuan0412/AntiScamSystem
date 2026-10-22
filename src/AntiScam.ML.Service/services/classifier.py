import os
from services.preprocessor import TextPreprocessor
from services.feature_extractor import FeatureExtractor
from models.schemas import ClassifyResponse

class ScamClassifier:
    """
    Bộ suy luận phân lớp lừa đảo và đưa ra cảnh báo kèm tín hiệu khớp.
    """
    SCAM_TYPE_ADVICE = {
        "BankImpersonation": "CẢNH BÁO: Kẻ gian đang giả danh ngân hàng. Không click đường link, tuyệt đối không cung cấp mã OTP hay mật khẩu tài khoản.",
        "EasyJobHighPay": "CẢNH BÁO: Bẫy lừa 'việc nhẹ lương cao', làm nhiệm vụ nạp tiền hoa hồng. Không chuyển bất kỳ khoản tiền cọc nào.",
        "AuthorityImpersonation": "CẢNH BÁO: Giả danh cơ quan công an / viện kiểm sát. Cơ quan nhà nước không làm việc qua điện thoại hay yêu cầu chuyển tiền 'bảo lãnh'.",
        "InvestmentFraud": "CẢNH BÁO: Lừa đảo đầu tư sàn tài chính ảo / lãi suất siêu cao. Tuyệt đối không nạp tiền.",
        "TravelTicketFraud": "CẢNH BÁO: Lừa đảo bán vé máy bay / combo du lịch giá rẻ. Chỉ đặt vé qua đại lý hoặc website chính hãng.",
        "RomanceScam": "CẢNH BÁO: Lừa đảo làm quen, hẹn hò qua mạng rồi mượn tiền hoặc gửi quà tặng kèm phí nhận.",
        "Other": "CẢNH BÁO: Nội dung có các dấu hiệu đáng ngờ. Hãy cẩn trọng trước khi thực hiện giao dịch."
    }

    def __init__(self):
        self.preprocessor = TextPreprocessor()
        self.feature_extractor = FeatureExtractor()

    def predict(self, content: str) -> ClassifyResponse:
        cleaned, manual_feats = self.preprocessor.preprocess(content)
        signals = manual_feats.get("signals", [])

        # Heuristic combined with ML rules
        lower = content.lower()
        is_scam = False
        scam_type = None
        confidence = 0.50

        if manual_feats["otp_count"] > 0 or (manual_feats["has_url"] and "ngân hàng" in lower):
            is_scam = True
            scam_type = "BankImpersonation"
            confidence = 0.94
        elif manual_feats["job_count"] > 0 or "việc nhẹ" in lower or "nhiệm vụ" in lower:
            is_scam = True
            scam_type = "EasyJobHighPay"
            confidence = 0.92
        elif "công an" in lower or "lệnh bắt" in lower or "viện kiểm sát" in lower:
            is_scam = True
            scam_type = "AuthorityImpersonation"
            confidence = 0.96
        elif "đầu tư" in lower and ("lợi nhuận" in lower or "hoa hồng" in lower):
            is_scam = True
            scam_type = "InvestmentFraud"
            confidence = 0.88
        elif "vé máy bay" in lower and ("giá rẻ" in lower or "chuyển cọc" in lower):
            is_scam = True
            scam_type = "TravelTicketFraud"
            confidence = 0.85
        elif len(signals) >= 2:
            is_scam = True
            scam_type = "Other"
            confidence = 0.80
        else:
            is_scam = False
            scam_type = None
            confidence = 0.95

        advice = self.SCAM_TYPE_ADVICE.get(scam_type, "Nội dung có vẻ an toàn. Luôn cảnh giác trước các yêu cầu chuyển khoản từ người lạ.")

        return ClassifyResponse(
            is_scam=is_scam,
            scam_type=scam_type,
            confidence=confidence,
            matched_signals=signals,
            advice=advice,
            model_version="v1.0-custom-hybrid"
        )
