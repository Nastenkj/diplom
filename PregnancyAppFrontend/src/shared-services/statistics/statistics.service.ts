import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpParams} from "@angular/common/http";
import {StatisticsDatePlotResultDto} from "../../dtos/statistics/statistics-date-plot-result-dto";
import {Observable} from "rxjs";
import {StatisticFields} from "../../dtos/internal/statistic-fields-enum";
import {ObservationParameterWithNormDto} from "../../dtos/statistics/observation-parameter-with-norm";

@Injectable({
  providedIn: 'root'
})
export class StatisticsService {
  private readonly baseUrl = `${environment.baseUrl}/web/statistics`;
  constructor(private http: HttpClient) {}

  getStatisticByDate(
    dbFieldName: StatisticFields,
    patientId: string,
    startDateUtc?: Date,
    endDateUtc?: Date,
  ): Observable<StatisticsDatePlotResultDto> {
    let params = new HttpParams().set('dbFieldName', dbFieldName);

    if (startDateUtc) {
      params = params.set('startDateUtc', startDateUtc.toISOString());
    }

    if (endDateUtc) {
      params = params.set('endDateUtc', endDateUtc.toISOString());
    }

    params = params.set('patientId', patientId);

    return this.http.get<StatisticsDatePlotResultDto>(this.baseUrl, { params });
  }

  getObservationParametersStatistics(
    patientId: string,
  ): Observable<Array<ObservationParameterWithNormDto>> {
    let params = new HttpParams().set('patientId', patientId);

    return this.http.get<Array<ObservationParameterWithNormDto>>(`${this.baseUrl}/with-average-values`, { params });
  }
}
