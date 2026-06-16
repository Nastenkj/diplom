FROM python:3.11-slim

WORKDIR /app

# Install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy model and application
COPY app.py .
COPY model_trimester_1.joblib /app/models/
COPY model_trimester_2.joblib /app/models/
COPY model_trimester_3.joblib /app/models/

# Expose port
EXPOSE 8000

# Run the application
CMD ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "8000"]
