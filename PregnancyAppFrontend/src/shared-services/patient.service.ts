import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface PatientState {
  stage: string;
  state: string;
  risks: string[];
  lastUpdate: string;
}

@Injectable({ providedIn: 'root' })
export class PatientService {
  private apiUrl = 'https://pregnancy-compass.duckdns.org/api';  // Или локальный URL

  constructor(private http: HttpClient) {}

  getPatientState(id: number): Observable<PatientState> {
    return this.http.get<PatientState>(`${this.apiUrl}/patients/${id}/state`);
  }
}