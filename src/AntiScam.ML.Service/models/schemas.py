from typing import List, Optional, Dict, Any
from pydantic import BaseModel

class ClassifyRequest(BaseModel):
    content: str
    model_version: str = "latest"

class ClassifyResponse(BaseModel):
    is_scam: bool
    scam_type: Optional[str] = None
    confidence: float
    matched_signals: List[str] = []
    advice: str
    model_version: str = "v1.0-scikit"

class TrainRequest(BaseModel):
    data_batch: str = "old"  # "old", "new", or "all"
    model_type: str = "naive_bayes"  # "naive_bayes", "manual_naive_bayes", "logistic_regression", "svm", "random_forest"
    hyperparameters: Optional[Dict[str, Any]] = None

class TrainingResponse(BaseModel):
    job_id: str
    status: str
    message: Optional[str] = None

class ModelVersionInfo(BaseModel):
    version: str
    model_type: str
    task: str
    accuracy: float
    precision: float
    recall: float
    f1_score: float
    trained_at: str
    is_active: bool
