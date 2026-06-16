import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DailyHealthPredictionResultDto } from '../../dtos/daily-health-prediction/daily-health-prediction-result-dto';

@Injectable({
  providedIn: 'root'
})
export class DailyHealthPredictionsService {
  private readonly baseUrl = `${environment.baseUrl}/web/DailyHealthPredictions`;

  constructor(private readonly http: HttpClient) {}

  getResultsByDateRange(fromUtc: Date, toUtc: Date): Observable<DailyHealthPredictionResultDto[]> {
    const params = new HttpParams()
      .set('fromUtc', fromUtc.toISOString())
      .set('toUtc', toUtc.toISOString());

    return this.http.get<DailyHealthPredictionResultDto[]>(
      `${this.baseUrl}/results`,
      { params }
    );
  }

  getResultByDailySurveyId(dailySurveyId: string): Observable<DailyHealthPredictionResultDto | null> {
    const params = new HttpParams().set('dailySurveyId', dailySurveyId);

    return this.http.get<DailyHealthPredictionResultDto | null>(
      `${this.baseUrl}/by-daily-survey`,
      { params }
    );
  }
}
