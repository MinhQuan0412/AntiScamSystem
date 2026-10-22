import numpy as np
import scipy.sparse as sp
from sklearn.feature_extraction.text import TfidfVectorizer
import joblib

class FeatureExtractor:
    """
    Trích xuất đặc trưng kết hợp TF-IDF từ và ký tự + đặc trưng thủ công
    (Không dùng mô hình pretrained hay bộ tách từ ngoài).
    """
    def __init__(self):
        self.word_vectorizer = TfidfVectorizer(
            ngram_range=(1, 2),
            max_features=5000,
            sublinear_tf=True
        )
        self.char_vectorizer = TfidfVectorizer(
            analyzer='char',
            ngram_range=(2, 4),
            max_features=3000,
            sublinear_tf=True
        )
        self.is_fitted = False

    def _extract_dense_features(self, manual_features_list: list[dict]) -> np.ndarray:
        feature_keys = [
            'has_url', 'has_phone', 'has_amount',
            'pressure_count', 'job_count', 'otp_count', 'transfer_count', 'char_count'
        ]
        matrix = []
        for feat in manual_features_list:
            row = [feat.get(k, 0.0) for k in feature_keys]
            matrix.append(row)
        return np.array(matrix, dtype=np.float32)

    def fit_transform(self, texts: list[str], manual_features_list: list[dict]):
        word_sparse = self.word_vectorizer.fit_transform(texts)
        char_sparse = self.char_vectorizer.fit_transform(texts)
        dense_feats = self._extract_dense_features(manual_features_list)
        dense_sparse = sp.csr_matrix(dense_feats)

        self.is_fitted = True
        return sp.hstack([word_sparse, char_sparse, dense_sparse], format='csr')

    def transform(self, texts: list[str], manual_features_list: list[dict]):
        if not self.is_fitted:
            raise ValueError("FeatureExtractor chưa được fit dữ liệu!")
        word_sparse = self.word_vectorizer.transform(texts)
        char_sparse = self.char_vectorizer.transform(texts)
        dense_feats = self._extract_dense_features(manual_features_list)
        dense_sparse = sp.csr_matrix(dense_feats)

        return sp.hstack([word_sparse, char_sparse, dense_sparse], format='csr')

    def save(self, filepath: str):
        joblib.dump(self, filepath)

    @classmethod
    def load(cls, filepath: str):
        return joblib.load(filepath)
