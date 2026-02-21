import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {UserRequestDto} from "../../dtos/patients/user-request-dto";
import {Observable} from "rxjs";
import {TableUserDto, TableUsersDto} from "../../dtos/patients/table-user-dto";
import {UserDto} from "../../dtos/patients/user-dto";

@Injectable({
  providedIn: 'root'
})
export class DoctorsService {
  private readonly baseUrl = `${environment.baseUrl}/web/Doctors`;
  private readonly headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  });

  constructor(private http: HttpClient) { }

  getDoctors(request: UserRequestDto): Observable<TableUsersDto> {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber.toString())
      .set('pageSize', request.pageSize.toString())
      .set('email', request.email?.toLowerCase() ?? '')
      .set('phoneNumber', request.phoneNumber ?? '')
      .set('name', request.name?.toLowerCase() ?? '')

    return this.http.get<TableUsersDto>(this.baseUrl, { params, headers: this.headers });
  }

  getDoctor(id?: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.baseUrl}/${id}`, { headers: this.headers });
  }

  editDoctor(userDto: UserDto): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.baseUrl}/${userDto.id}`, userDto, {
      headers: this.headers
    });
  }
}
