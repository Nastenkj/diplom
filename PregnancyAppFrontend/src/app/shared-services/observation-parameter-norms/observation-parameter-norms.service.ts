import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {Observable} from "rxjs";
import {ObservationParameterNormDto} from "../../dtos/observation-parameter-norms/observation-parameter-norm-dto";
import {StatisticFields} from "../../dtos/internal/statistic-fields-enum";

@Injectable({
  providedIn: 'root'
})
export class ObservationParameterNormsService {
  private readonly baseUrl = `${environment.baseUrl}/web/ObservationParameters`;
  private readonly headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  });

  constructor(private http: HttpClient) { }

  getObservationParameterNorms(userId: string | null = null): Observable<ObservationParameterNormDto[]> {
    const url = `${this.baseUrl}?userId=${userId}`;

    return this.http.get<ObservationParameterNormDto[]>(url, {
      headers: this.headers
    });
  }

  updateObservationParameterNormBounds(
    lowerBound: number,
    upperBound: number,
    parameterName: StatisticFields,
    userId: string | null = null,
  ): Observable<ObservationParameterNormDto> {
    const url = `${this.baseUrl}?userId=${userId}&upperBound=${upperBound}&parameterName=${parameterName}&lowerBound=${lowerBound}`;

    return this.http.post<ObservationParameterNormDto>(url, null, {
      headers: this.headers
    });
  }
}
