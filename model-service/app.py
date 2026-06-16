from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from joblib import load
import numpy as np
import os
import json
import asyncio
from kafka import KafkaConsumer, KafkaProducer
from typing import Optional, List, Dict, Any

app = FastAPI(title="Pregnancy Classification Model API")

# Конфигурация Kafka
KAFKA_BOOTSTRAP_SERVERS = os.environ.get("KAFKA_BOOTSTRAP_SERVERS", "localhost:9092")
PREDICTION_REQUEST_TOPIC = "prediction-requests"
PREDICTION_RESPONSE_TOPIC = "prediction-responses"

# Пути к трём моделям
MODEL_PATHS = {
    1: os.environ.get("MODEL_PATH_T1", "/app/models/model_trimester_1.joblib"),
    2: os.environ.get("MODEL_PATH_T2", "/app/models/model_trimester_2.joblib"),
    3: os.environ.get("MODEL_PATH_T3", "/app/models/model_trimester_3.joblib"),
}

# Загрузка трёх моделей
models = {1: None, 2: None, 3: None}

for trimester, path in MODEL_PATHS.items():
    try:
        models[trimester] = load(path)
        print(f"✅ Model for trimester {trimester} loaded from {path}")
    except Exception as e:
        print(f"❌ Error loading model for trimester {trimester}: {e}")

# Kafka producer
producer = None

def get_kafka_producer():
    global producer
    if producer is None:
        try:
            producer = KafkaProducer(
                bootstrap_servers=KAFKA_BOOTSTRAP_SERVERS,
                value_serializer=lambda v: json.dumps(v, ensure_ascii=False).encode('utf-8')
            )
        except Exception as e:
            print(f"Error creating Kafka producer: {e}")
    return producer

# Названия признаков
FEATURE_NAMES = [
    'Боль_внизу_живота',
    'Тошнота', 
    'Количество_рвотных_позывов_за_день',
    'Признаки_ОРВИ',
    'Температура_тела',
    'Сатурация',
    'BLD',
    'KET',
    'LEU',
    'GLU',
    'NIT',
    'URO',
    'BIL',
    'VC',
    'PRO',
    'PH',
    'SG',
    'Артериальное_давление_верхнее',
    'Артериальное_давление_нижнее',
    'Пульс',
    'Глюкоза'
]

# ================================
# НОРМЫ ПОКАЗАТЕЛЕЙ ПО ТРИМЕСТРАМ
# ================================
NORMAL_RANGES = {
    1: {  # 1 триместр
        'Боль_внизу_живота': (0, 0),
        'Тошнота': (0, 0),
        'Количество_рвотных_позывов_за_день': (0, 0),
        'Признаки_ОРВИ': (0, 0),
        'Температура_тела': (36.2, 37.2),
        'Сатурация': (95, 99),
        'BLD': (0, 0),
        'KET': (0, 1),
        'LEU': (0, 1),
        'GLU': (0, 0),
        'NIT': (0, 0),
        'URO': (0, 1),
        'BIL': (0, 0),
        'VC': (0, 0),
        'PRO': (0, 0),
        'PH': (5, 7),
        'SG': (1.010, 1.035),
        'Артериальное_давление_верхнее': (100, 130),
        'Артериальное_давление_нижнее': (60, 90),
        'Пульс': (60, 80),
        'Глюкоза': (4.0, 5.5)
    },
    2: {  # 2 триместр
        'Боль_внизу_живота': (0, 0),
        'Тошнота': (0, 0),
        'Количество_рвотных_позывов_за_день': (0, 0),
        'Признаки_ОРВИ': (0, 0),
        'Температура_тела': (36.6, 36.8),
        'Сатурация': (95, 99),
        'BLD': (0, 0),
        'KET': (0, 0),
        'LEU': (0, 0),
        'GLU': (0, 0),
        'NIT': (0, 0),
        'URO': (0, 1),
        'BIL': (0, 0),
        'VC': (0, 0),
        'PRO': (0, 1),
        'PH': (5, 7),
        'SG': (1.010, 1.035),
        'Артериальное_давление_верхнее': (110, 130),
        'Артериальное_давление_нижнее': (70, 90),
        'Пульс': (70, 90),
        'Глюкоза': (4.0, 5.5)
    },
    3: {  # 3 триместр
        'Боль_внизу_живота': (0, 0),
        'Тошнота': (0, 0),
        'Количество_рвотных_позывов_за_день': (0, 0),
        'Признаки_ОРВИ': (0, 0),
        'Температура_тела': (36.6, 36.8),
        'Сатурация': (95, 99),
        'BLD': (0, 0),
        'KET': (0, 0),
        'LEU': (0, 0),
        'GLU': (0, 1),
        'NIT': (0, 0),
        'URO': (0, 1),
        'BIL': (0, 0),
        'VC': (0, 0),
        'PRO': (0, 1),
        'PH': (5, 7),
        'SG': (1.010, 1.035),
        'Артериальное_давление_верхнее': (110, 130),
        'Артериальное_давление_нижнее': (70, 90),
        'Пульс': (70, 100),
        'Глюкоза': (4.0, 5.5)
    }
}

# ============================================
# ФУНКЦИЯ АНАЛИЗА ОТКЛОНЕНИЙ
# ============================================
def analyze_deviations(features: List[float], trimester: int) -> List[Dict[str, Any]]:
    """
    Анализирует отклонения показателей от нормы для данного триместра
    Возвращает список нарушений
    """
    norms = NORMAL_RANGES.get(trimester, NORMAL_RANGES[2])
    deviations = []
    
    for i, feature_name in enumerate(FEATURE_NAMES):
        value = features[i]
        min_val, max_val = norms.get(feature_name, (0, 999))
        
        # Для бинарных показателей
        if feature_name in ['Боль_внизу_живота', 'Тошнота', 'Признаки_ОРВИ', 
                           'BLD', 'KET', 'LEU', 'GLU', 'NIT', 'BIL', 'VC']:
            if value == 1 and max_val == 0:
                deviations.append({
                    'feature': feature_name,
                    'value': value,
                    'normal_range': 'отсутствует',
                    'severity': 'critical' if feature_name in ['Боль_внизу_живота', 'BLD'] else 'warning',
                    'message': f'⚠️ {feature_name}: обнаружено (должно отсутствовать)'
                })
            elif value > 1 and max_val == 0:
                deviations.append({
                    'feature': feature_name,
                    'value': value,
                    'normal_range': 'отсутствует',
                    'severity': 'critical',
                    'message': f'🚨 {feature_name}: значение {value} (должно отсутствовать)'
                })
            elif value > max_val and max_val > 0:
                deviations.append({
                    'feature': feature_name,
                    'value': value,
                    'normal_range': f'0-{max_val}',
                    'severity': 'warning',
                    'message': f'⚠️ {feature_name}: {value} (норма: 0-{max_val})'
                })
        else:
            # Для непрерывных показателей
            if value < min_val:
                deviations.append({
                    'feature': feature_name,
                    'value': value,
                    'normal_range': f'{min_val}-{max_val}',
                    'severity': 'warning',
                    'message': f'📉 {feature_name}: {value} (ниже нормы: {min_val}-{max_val})'
                })
            elif value > max_val:
                severity = 'critical' if feature_name in ['Артериальное_давление_верхнее', 'Глюкоза'] else 'warning'
                deviations.append({
                    'feature': feature_name,
                    'value': value,
                    'normal_range': f'{min_val}-{max_val}',
                    'severity': severity,
                    'message': f'📈 {feature_name}: {value} (выше нормы: {min_val}-{max_val})'
                })
    
    return deviations

# ============================================
# МОДЕЛИ ДАННЫХ
# ============================================
class PredictionRequest(BaseModel):
    features: List[float]
    trimester: int  # 1, 2 или 3

class PredictionResponse(BaseModel):
    trimester: int
    prediction: int  # 0=Норма, 1=Предупреждение, 2=Патология
    prediction_text: str
    probabilities: Dict[str, float]
    deviations: List[Dict[str, Any]]  # ← СПИСОК ОТКЛОНЕНИЙ

# ============================================
# ФУНКЦИЯ ДЛЯ ОПРЕДЕЛЕНИЯ ТРИМЕСТРА ПО НЕДЕЛЕ
# ============================================
def get_trimester_by_week(week: int) -> int:
    if week < 14:
        return 1
    elif week <= 26:
        return 2
    else:
        return 3

# ============================================
# REST ЭНДПОИНТЫ
# ============================================
@app.get("/health")
async def health_check():
    loaded = {t: m is not None for t, m in models.items()}
    return {"status": "healthy", "models_loaded": loaded}

@app.post("/predict", response_model=PredictionResponse)
async def predict(request: PredictionRequest):
    trimester = request.trimester
    
    # Проверяем, что модель для этого триместра загружена
    model = models.get(trimester)
    if model is None:
        raise HTTPException(status_code=404, detail=f"No model for trimester {trimester}")
    
    if len(request.features) != 21:
        raise HTTPException(status_code=400, detail=f"Expected 21 features, got {len(request.features)}")
    
    # Преобразование в numpy array и предсказание
    features_array = np.array(request.features).reshape(1, -1)
    
    # Предсказание класса
    prediction = int(model.predict(features_array)[0])
    
    # Вероятности
    try:
        proba = model.predict_proba(features_array)[0]
        probabilities = {
            "normal": float(proba[0]),
            "alert": float(proba[1]) if len(proba) > 1 else 0.0,
            "pathology": float(proba[2]) if len(proba) > 2 else 0.0
        }
    except:
        probabilities = {"normal": 0.0, "alert": 0.0, "pathology": 0.0}
    
    # Анализ отклонений
    deviations = analyze_deviations(request.features, trimester)
    
    # Текстовое описание
    prediction_texts = {
        0: "Норма",
        1: "Предупреждение", 
        2: "Патология"
    }
    
    return PredictionResponse(
        trimester=trimester,
        prediction=prediction,
        prediction_text=prediction_texts.get(prediction, "Неизвестно"),
        probabilities=probabilities,
        deviations=deviations
    )

# ============================================
# ЭНДПОИНТ С УЧЁТОМ НЕДЕЛИ (УДОБНЕЕ)
# ============================================
class PredictionByWeekRequest(BaseModel):
    features: List[float]
    week: int  # неделя беременности (1-42)

@app.post("/predict_by_week", response_model=PredictionResponse)
async def predict_by_week(request: PredictionByWeekRequest):
    """Предсказание с автоматическим определением триместра по неделе"""
    week = request.week
    trimester = get_trimester_by_week(week)
    
    model = models.get(trimester)
    if model is None:
        raise HTTPException(status_code=404, detail=f"No model for week {week} (trimester {trimester})")
    
    if len(request.features) != 21:
        raise HTTPException(status_code=400, detail=f"Expected 21 features, got {len(request.features)}")
    
    features_array = np.array(request.features).reshape(1, -1)
    prediction = int(model.predict(features_array)[0])
    
    try:
        proba = model.predict_proba(features_array)[0]
        probabilities = {
            "normal": float(proba[0]),
            "alert": float(proba[1]) if len(proba) > 1 else 0.0,
            "pathology": float(proba[2]) if len(proba) > 2 else 0.0
        }
    except:
        probabilities = {"normal": 0.0, "alert": 0.0, "pathology": 0.0}
    
    deviations = analyze_deviations(request.features, trimester)
    
    prediction_texts = {0: "Норма", 1: "Предупреждение", 2: "Патология"}
    
    return PredictionResponse(
        trimester=trimester,
        prediction=prediction,
        prediction_text=prediction_texts.get(prediction, "Неизвестно"),
        probabilities=probabilities,
        deviations=deviations
    )

# ============================================
# KAFKA CONSUMER
# ============================================
def make_prediction_with_deviations(features: List[float], trimester: int) -> dict:
    """Выполняет предсказание модели с анализом отклонений"""
    model = models.get(trimester)
    if model is None:
        return {"error": f"No model for trimester {trimester}"}
    
    if len(features) != 21:
        return {"error": f"Expected 21 features, got {len(features)}"}
    
    features_array = np.array(features).reshape(1, -1)
    prediction = int(model.predict(features_array)[0])
    
    try:
        proba = model.predict_proba(features_array)[0]
        probabilities = {
            "normal": float(proba[0]),
            "alert": float(proba[1]) if len(proba) > 1 else 0.0,
            "pathology": float(proba[2]) if len(proba) > 2 else 0.0
        }
    except:
        probabilities = {"normal": 0.0, "alert": 0.0, "pathology": 0.0}
    
    deviations = analyze_deviations(features, trimester)
    
    prediction_texts = {0: "Норма", 1: "Предупреждение", 2: "Патология"}
    
    return {
        "trimester": trimester,
        "prediction": prediction,
        "prediction_text": prediction_texts.get(prediction, "Неизвестно"),
        "probabilities": probabilities,
        "deviations": deviations
    }

async def consume_kafka_messages():
    """Консьюмер Kafka для асинхронной обработки запросов"""
    print(f"Starting Kafka consumer on {KAFKA_BOOTSTRAP_SERVERS}...")
    
    while True:
        try:
            consumer = KafkaConsumer(
                PREDICTION_REQUEST_TOPIC,
                bootstrap_servers=KAFKA_BOOTSTRAP_SERVERS,
                value_deserializer=lambda m: json.loads(m.decode('utf-8')),
                group_id='model-service-group',
                auto_offset_reset='latest',
                enable_auto_commit=True
            )
            
            print(f"Connected to Kafka topic: {PREDICTION_REQUEST_TOPIC}")
            
            for message in consumer:
                try:
                    request_data = message.value

                    request_id = (
                        request_data.get("request_id")
                        or request_data.get("RequestId")
                        or "unknown"
                    )
                    features = (
                        request_data.get("features")
                        or request_data.get("Features")
                        or []
                    )
                    trimester = (
                        request_data.get("trimester")
                        or request_data.get("Trimester")
                        or 1
                    )

                    # Идентификаторы, необходимые backend-у для сохранения в БД и уведомления пациента
                    user_id = (
                        request_data.get("user_id")
                        or request_data.get("UserId")
                    )
                    daily_survey_id = (
                        request_data.get("daily_survey_id")
                        or request_data.get("DailySurveyId")
                    )

                    print(f"Processing request {request_id} for trimester {trimester}")

                    result = make_prediction_with_deviations(features, trimester)
                    result["RequestId"] = request_id
                    # Прокидываем user_id / daily_survey_id дальше в response-topic
                    if user_id is not None:
                        result["user_id"] = user_id
                    if daily_survey_id is not None:
                        result["daily_survey_id"] = daily_survey_id
                    result["created_at_utc"] = None
                    
                    # created_at_utc (в формате ISO8601, чтобы C# корректно десериализовал DateTime?)
                    # Здесь используем UTC ISO format
                    import datetime
                    result["created_at_utc"] = datetime.datetime.utcnow().isoformat()

                    prod = get_kafka_producer()
                    if prod:
                        prod.send(PREDICTION_RESPONSE_TOPIC, value=result)
                        prod.flush()
                        print(f"Sent prediction result for request {request_id}")
                    
                except Exception as e:
                    print(f"Error processing message: {e}")
                    
        except Exception as e:
            print(f"Kafka consumer error: {e}")
            await asyncio.sleep(5)

@app.on_event("startup")
async def startup_event():
    """Запуск Kafka консьюмера при старте приложения"""
    asyncio.create_task(consume_kafka_messages())
    print("🚀 Service started with 3 models and deviation analysis")

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)