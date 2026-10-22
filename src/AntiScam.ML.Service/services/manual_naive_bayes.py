import numpy as np
import scipy.sparse as sp

class ManualMultinomialNaiveBayes:
    """
    Tự cài đặt thủ công thuật toán Multinomial Naive Bayes (Laplace Smoothing)
    Dùng để đối chiếu với bản thư viện scikit-learn.
    """
    def __init__(self, alpha: float = 1.0):
        self.alpha = alpha  # Laplace smoothing factor
        self.class_priors_ = None
        self.feature_log_probs_ = None
        self.classes_ = None
        self.n_features_ = 0

    def fit(self, X, y):
        if sp.issparse(X):
            X = X.toarray()
        else:
            X = np.asarray(X)
        y = np.asarray(y)

        self.classes_ = np.unique(y)
        n_classes = len(self.classes_)
        n_samples, self.n_features_ = X.shape

        self.class_priors_ = np.zeros(n_classes)
        feature_counts = np.zeros((n_classes, self.n_features_))

        for idx, c in enumerate(self.classes_):
            X_c = X[y == c]
            self.class_priors_[idx] = np.log(X_c.shape[0] / n_samples)
            feature_counts[idx, :] = np.sum(X_c, axis=0)

        # Laplace smoothing: (count + alpha) / (total_count + alpha * n_features)
        total_counts = np.sum(feature_counts, axis=1, keepdims=True)
        smoothed_probs = (feature_counts + self.alpha) / (total_counts + self.alpha * self.n_features_)
        self.feature_log_probs_ = np.log(smoothed_probs)

        return self

    def predict_log_proba(self, X):
        if sp.issparse(X):
            X = X.toarray()
        else:
            X = np.asarray(X)

        # log P(c|x) = log P(c) + sum(x_i * log P(w_i|c))
        return X @ self.feature_log_probs_.T + self.class_priors_

    def predict_proba(self, X):
        log_probs = self.predict_log_proba(X)
        # Log-sum-exp trick for numerical stability
        max_log = np.max(log_probs, axis=1, keepdims=True)
        exp_probs = np.exp(log_probs - max_log)
        return exp_probs / np.sum(exp_probs, axis=1, keepdims=True)

    def predict(self, X):
        probs = self.predict_proba(X)
        indices = np.argmax(probs, axis=1)
        return self.classes_[indices]

    def score(self, X, y):
        preds = self.predict(X)
        return np.mean(preds == y)
