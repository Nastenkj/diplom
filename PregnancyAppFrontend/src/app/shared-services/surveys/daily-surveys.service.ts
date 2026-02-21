import { Injectable } from '@angular/core';
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { DailySurveyDto } from "../../dtos/daily-survey/daily-survey-dto";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class DailySurveyService {
  private readonly baseUrl = `${environment.baseUrl}/web/DailySurveys`;
  constructor(private http: HttpClient) {}

  postDailySurvey(data: DailySurveyDto): Observable<DailySurveyDto> {
    return this.http.post<DailySurveyDto>(this.baseUrl, data);
  }

  getLatestDailySurveyDate(): Observable<Date | null> {
    return this.http.get<Date | null>(`${this.baseUrl}/latest-date`);
  }

  getDailySurveysForUser(userId: string | null = null): Observable<DailySurveyDto[]> {
    return this.http.get<DailySurveyDto[]>(`${this.baseUrl}?userId=${userId}`);
  }

  getDailySurveyById(surveyId: string): Observable<DailySurveyDto> {
    return this.http.get<DailySurveyDto>(`${this.baseUrl}/${surveyId}`);
  }
}
