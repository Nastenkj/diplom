import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {CreateCommunicationLinkDto} from "../../dtos/communication-link/create-communication-link-dto";
import {CommunicationLinkDto} from "../../dtos/communication-link/communication-link-dto";

@Injectable({
  providedIn: 'root'
})
export class CommunicationLinkService {
  private apiUrl = `${environment.baseUrl}/web/communicationLinks`;

  constructor(private http: HttpClient) { }

  createCommunicationLink(dto: CreateCommunicationLinkDto): Observable<CommunicationLinkDto> {
    return this.http.post<CommunicationLinkDto>(this.apiUrl, dto);
  }

  getCommunicationLinks(): Observable<CommunicationLinkDto[]> {
    return this.http.get<CommunicationLinkDto[]>(this.apiUrl);
  }
}
