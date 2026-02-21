import { Injectable } from '@angular/core';
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { MedicalHistoryDto } from "../../dtos/medical-history/medical-history-dto";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class MedicalHistoriesService {
  private readonly baseUrl = `${environment.baseUrl}/web/medicalhistories`;
  constructor(private http: HttpClient) {}

  postMedicalHistory(data: MedicalHistoryDto): Observable<MedicalHistoryDto> {
    return this.http.post<MedicalHistoryDto>(this.baseUrl, data);
  }

  getMedicalHistory(userId: string | null = null): Observable<MedicalHistoryDto | null> {
    let url = `${this.baseUrl}`;
    if (userId !== null) {
        url += `?userId=${userId}`;
    }
    return this.http.get<MedicalHistoryDto | null>(url);
  }
}
