import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {UserRequestDto} from "../../dtos/patients/user-request-dto";
import {TableUserDto, TableUsersDto} from "../../dtos/patients/table-user-dto";
import {Observable} from "rxjs";
import {UserDto} from "../../dtos/patients/user-dto";

@Injectable({
  providedIn: 'root'
})
export class PatientsService {
  private readonly baseUrl = `${environment.baseUrl}/web/Patients`;
  private readonly headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  });

  constructor(private http: HttpClient) { }

  getPatients(request: UserRequestDto): Observable<TableUsersDto> {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber.toString())
      .set('pageSize', request.pageSize.toString())
      .set('email', request.email?.toLowerCase() ?? '')
      .set('phoneNumber', request.phoneNumber ?? '')
      .set('name', request.name?.toLowerCase() ?? '')

    return this.http.get<TableUsersDto>(this.baseUrl, { params, headers: this.headers });
  }

  getPatient(id?: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.baseUrl}/${id}`, { headers: this.headers });
  }

  editPatient(patient: UserDto): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.baseUrl}/${patient.id}`, patient, {
      headers: this.headers
    });
  }
}
