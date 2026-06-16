import { Injectable } from '@angular/core';
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { HealthPredictionResponseDto } from "../../dtos/health-prediction/health-prediction-dto";

@Injectable({
  providedIn: 'root'
})
export class HealthPredictionService {
  private readonly baseUrl = `${environment.baseUrl}/web/healthpredictions`;

  constructor(private http: HttpClient) {}

  /**
   * Получить прогноз на основе последнего обследования
   */
  getPredictionFromLatestSurvey(): Observable<HealthPredictionResponseDto> {
    return this.http.get<HealthPredictionResponseDto>(`${this.baseUrl}/from-latest-survey`);
  }

  /**
   * Получить прогноз по ID обследования
   */
  getPredictionFromSurvey(surveyId: string): Observable<HealthPredictionResponseDto> {
    return this.http.get<HealthPredictionResponseDto>(`${this.baseUrl}/from-survey/${surveyId}`);
  }

  /**
   * Проверить доступность сервиса модели
   */
  checkHealth(): Observable<any> {
    return this.http.get(`${this.baseUrl}/health`);
  }
}

