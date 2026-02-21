import { Injectable } from '@angular/core';
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { WeeklySurveyDto } from "../../dtos/weekly-survey/weekly-survey-dto";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class WeeklySurveysService {
  private readonly baseUrl = `${environment.baseUrl}/web/WeeklySurveys`;
  constructor(private http: HttpClient) {}

  postWeeklySurvey(data: WeeklySurveyDto): Observable<WeeklySurveyDto> {
    return this.http.post<WeeklySurveyDto>(this.baseUrl, data);
  }

  getLatestWeeklySurveyDate(): Observable<Date | null> {
    return this.http.get<Date | null>(`${this.baseUrl}/latest-date`);
  }

  getWeeklySurveysForUser(userId: string | null = null): Observable<WeeklySurveyDto[]> {
    return this.http.get<WeeklySurveyDto[]>(`${this.baseUrl}?userId=${userId}`);
  }

  getWeeklySurveyById(surveyId: string): Observable<WeeklySurveyDto> {
    return this.http.get<WeeklySurveyDto>(`${this.baseUrl}/${surveyId}`);
  }
}
