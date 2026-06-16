export interface HealthPredictionRequestDto {
  features: number[];
  trimester: number;
  userId?: string;
  dailySurveyId?: string;
}

export interface HealthPredictionResponseDto {
  prediction: number;
  predictionText: string;
  probabilities: PredictionProbabilities;
  requestId: string;
  trimester: number;
}

export interface PredictionProbabilities {
  normal: number;
  alert: number;
  pathology: number;
}

