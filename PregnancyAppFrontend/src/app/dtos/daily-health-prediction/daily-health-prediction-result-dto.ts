export interface PredictionProbabilitiesDto {
  normal: number;
  alert: number;
  pathology: number;
}

export interface DailyHealthPredictionDeviationDto {
  feature: string;
  value: number;
  normalRange: string;
  severity: string;
  message: string;
}

export interface DailyHealthPredictionResultDto {
  dailySurveyId: string;
  creationDateUtc: string; // ISO
  trimester: number;
  prediction: number; // 0=Норма,1=Предупреждение,2=Патология
  predictionText: string;
  probabilities: PredictionProbabilitiesDto;
  deviations: DailyHealthPredictionDeviationDto[];
}
