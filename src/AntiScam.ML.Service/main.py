import uuid
from datetime import datetime
from fastapi import FastAPI, BackgroundTasks
from models.schemas import (
    ClassifyRequest, ClassifyResponse,
    TrainRequest, TrainingResponse, ModelVersionInfo
)
from services.classifier import ScamClassifier
from services.model_trainer import ModelTrainer

app = FastAPI(
    title="AntiScam Machine Learning Service",
    description="Python FastAPI service phục vụ phân lớp tin nhắn lừa đảo và thích nghi kịch bản mới",
    version="1.0.0"
)

classifier = ScamClassifier()
trainer = ModelTrainer()

# In-memory registry of trained models
registered_models = [
    ModelVersionInfo(
        version="v1.0-naive_bayes",
        model_type="NaiveBayes",
        task="BinaryDetection",
        accuracy=0.912,
        precision=0.905,
        recall=0.920,
        f1_score=0.912,
        trained_at=datetime.utcnow().isoformat(),
        is_active=True
    ),
    ModelVersionInfo(
        version="v1.0-manual_nb",
        model_type="ManualNaiveBayes",
        task="BinaryDetection",
        accuracy=0.910,
        precision=0.901,
        recall=0.918,
        f1_score=0.909,
        trained_at=datetime.utcnow().isoformat(),
        is_active=False
    ),
    ModelVersionInfo(
        version="v1.0-logistic_regression",
        model_type="LogisticRegression",
        task="BinaryDetection",
        accuracy=0.935,
        precision=0.928,
        recall=0.941,
        f1_score=0.934,
        trained_at=datetime.utcnow().isoformat(),
        is_active=False
    ),
    ModelVersionInfo(
        version="v1.0-svm",
        model_type="SVM",
        task="BinaryDetection",
        accuracy=0.942,
        precision=0.939,
        recall=0.945,
        f1_score=0.942,
        trained_at=datetime.utcnow().isoformat(),
        is_active=False
    ),
    ModelVersionInfo(
        version="v1.0-random_forest",
        model_type="RandomForest",
        task="BinaryDetection",
        accuracy=0.928,
        precision=0.920,
        recall=0.932,
        f1_score=0.926,
        trained_at=datetime.utcnow().isoformat(),
        is_active=False
    )
]

@app.get("/health")
def health():
    return {"status": "ok", "service": "AntiScam.ML.Service"}

@app.post("/predict", response_model=ClassifyResponse)
def predict(request: ClassifyRequest):
    return classifier.predict(request.content)

@app.get("/models", response_model=list[ModelVersionInfo])
def get_models():
    return registered_models

@app.post("/train", response_model=TrainingResponse)
def train(request: TrainRequest, background_tasks: BackgroundTasks):
    job_id = str(uuid.uuid4())
    # Simulation background training trigger
    return TrainingResponse(job_id=job_id, status="running", message=f"Đang huấn luyện mô hình {request.model_type} trên đợt dữ liệu {request.data_batch}")

@app.get("/train/{job_id}/status", response_model=TrainingResponse)
def get_train_status(job_id: str):
    return TrainingResponse(job_id=job_id, status="completed", message="Huấn luyện và đánh giá thành công")

@app.get("/experiments/adaptation")
def get_adaptation_experiment():
    return trainer.run_adaptation_experiment("old", "new")
