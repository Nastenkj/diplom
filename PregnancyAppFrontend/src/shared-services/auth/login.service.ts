import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {AuthenticationResponse} from "../../dtos/auth/authentication-response";
import {environment} from "../../environments/environment";
import {BaseRegistrationRequestDto, RegistrationRequestDto} from "../../dtos/auth/registration-request";

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  constructor(private http: HttpClient) {}

  login(data: {email: string, password: string}): Observable<AuthenticationResponse> {
    return this.http.post<AuthenticationResponse>(`${environment.baseUrl}/web/Authentication/login`, data);
  }

  register(data: RegistrationRequestDto): Observable<AuthenticationResponse> {
    return this.http.post<AuthenticationResponse>(`${environment.baseUrl}/web/Authentication/register`, data);
  }

  registerDoctor(data: BaseRegistrationRequestDto): Observable<AuthenticationResponse> {
    return this.http.post<AuthenticationResponse>(`${environment.baseUrl}/web/Authentication/register-doctor`, data);
  }

  check() {
    return this.http.get(`${environment.baseUrl}/web/Authentication/check`);
  }

  test() {
    return this.http.get(`${environment.baseUrl}/web/Authentication/users`);
  }

}
