import re
import unicodedata
from typing import Tuple, Dict, Any, List

class TextPreprocessor:
    """
    Tiền xử lý văn bản tiếng Việt và trích xuất đặc trưng thủ công
    (Không dùng pretrained model hay bộ tách từ ngoài).
    """

    PRESSURE_WORDS = [
        'khóa tài khoản', 'khoa tai khoan', 'khẩn cấp', 'khan cap',
        'trúng thưởng', 'trung thuong', 'gấp', 'ngay lập tức', 'ngay lap tuc',
        'tạm khóa', 'tam khoa', 'lệnh bắt giam', 'lenh bat giam',
        'viện kiểm sát', 'cơ quan điều tra', 'bộ công an', 'bo cong an',
        'rửa tiền', 'truy nã', 'đình chỉ', 'hạn chót', 'qua han'
    ]

    JOB_WORDS = [
        'việc nhẹ lương cao', 'viec nhe luong cao', 'nhiệm vụ', 'nhiem vu',
        'hoa hồng', 'hoa hong', 'đặt đơn', 'shopee', 'lazada', 'tiktok shop',
        'nạp tiền', 'cắn tiền', 'hoàn tiền', 'tiền hoa hồng'
    ]

    OTP_WORDS = [
        'mã otp', 'ma otp', 'mật khẩu', 'mat khau', 'mã xác thực', 'ma xac thuc',
        'smart otp', 'mã pin', 'ma pin'
    ]

    TRANSFER_WORDS = [
        'chuyển tiền', 'chuyen tien', 'chuyển khoản', 'chuyen khoan',
        'số tài khoản', 'so tai khoan', 'nạp tiền cọc', 'phí xác minh'
    ]

    def clean_text(self, text: str) -> str:
        if not text:
            return ""
        # 1. Unicode NFC
        text = unicodedata.normalize("NFC", text)
        # 2. Lowercase
        text = text.lower()
        # 3. Mask phone numbers
        text = re.sub(r'0[3|5|7|8|9]\d{8}', ' [SDT] ', text)
        # 4. Mask bank accounts
        text = re.sub(r'\b\d{9,16}\b', ' [STK] ', text)
        # 5. Mask URLs
        text = re.sub(r'https?://\S+|www\.\S+', ' [LINK] ', text)
        # 6. Normalize punctuation and whitespace
        text = re.sub(r'[^\w\s\[\]]', ' ', text)
        text = re.sub(r'\s+', ' ', text).strip()
        return text

    def extract_manual_features(self, text: str) -> Dict[str, Any]:
        lower = text.lower()
        
        has_url = bool(re.search(r'https?://|www\.|\.top|\.xyz|\.vip|\.club', lower))
        has_phone = bool(re.search(r'0[3|5|7|8|9]\d{8}', text))
        has_amount = bool(re.search(r'\d+[\s\.\,]*(triệu|tr|nghìn|k|tỷ|vnd|đ|usd)', lower))
        
        pressure_matches = [w for w in self.PRESSURE_WORDS if w in lower]
        job_matches = [w for w in self.JOB_WORDS if w in lower]
        otp_matches = [w for w in self.OTP_WORDS if w in lower]
        transfer_matches = [w for w in self.TRANSFER_WORDS if w in lower]

        signals = []
        if has_url: signals.append("Chứa liên kết URL khả nghi")
        if has_phone: signals.append("Chứa số điện thoại liên hệ")
        if has_amount: signals.append("Chứa số tiền giao dịch/trúng thưởng")
        if pressure_matches: signals.extend([f"Từ ngữ gây áp lực: '{w}'" for w in pressure_matches[:3]])
        if job_matches: signals.extend([f"Từ khóa việc nhẹ hoa hồng: '{w}'" for w in job_matches[:2]])
        if otp_matches: signals.append("Yêu cầu cung cấp mã OTP/Mật khẩu bí mật")
        if transfer_matches: signals.append("Yêu cầu chuyển tiền/chuyển khoản")

        return {
            "has_url": 1.0 if has_url else 0.0,
            "has_phone": 1.0 if has_phone else 0.0,
            "has_amount": 1.0 if has_amount else 0.0,
            "pressure_count": float(len(pressure_matches)),
            "job_count": float(len(job_matches)),
            "otp_count": float(len(otp_matches)),
            "transfer_count": float(len(transfer_matches)),
            "char_count": float(len(text)),
            "signals": signals
        }

    def preprocess(self, text: str) -> Tuple[str, Dict[str, Any]]:
        cleaned = self.clean_text(text)
        features = self.extract_manual_features(text)
        return cleaned, features
