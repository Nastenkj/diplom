import { Injectable } from '@angular/core';
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class ExcelExportService {
  private readonly baseUrl = `${environment.baseUrl}/api/excel`;

  constructor(private http: HttpClient) {}

  downloadDailySurvey(surveyId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/daily-survey/${surveyId}`, {
      responseType: 'blob'
    });
  }

  downloadDailySurveys(userId?: string): Observable<Blob> {
    const url = userId
      ? `${this.baseUrl}/daily-surveys?userId=${userId}`
      : `${this.baseUrl}/daily-surveys`;

    return this.http.get(url, {
      responseType: 'blob'
    });
  }

  downloadWeeklySurvey(surveyId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/weekly-survey/${surveyId}`, {
      responseType: 'blob'
    });
  }

  downloadWeeklySurveys(userId?: string): Observable<Blob> {
    const url = userId
      ? `${this.baseUrl}/weekly-surveys?userId=${userId}`
      : `${this.baseUrl}/weekly-surveys`;

    return this.http.get(url, {
      responseType: 'blob'
    });
  }

  downloadMedicalHistory(userId?: string): Observable<Blob> {
    const url = userId
      ? `${this.baseUrl}/medical-history?userId=${userId}`
      : `${this.baseUrl}/medical-history`;

    return this.http.get(url, {
      responseType: 'blob'
    });
  }

  downloadObservationParameters(userId?: string): Observable<Blob> {
    const url = userId
      ? `${this.baseUrl}/observation-parameters?userId=${userId}`
      : `${this.baseUrl}/observation-parameters`;

    return this.http.get(url, {
      responseType: 'blob'
    });
  }

  saveFile(data: Blob, filename: string): void {
    const url = window.URL.createObjectURL(data);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();

    setTimeout(() => {
      window.URL.revokeObjectURL(url);
      link.remove();
    }, 100);
  }
}
