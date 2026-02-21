import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import {AuthService} from "../../auth/auth.service";
import {AlertsService} from "../alerts/alerts.service";

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection | null = null;
  private messageSubject = new BehaviorSubject<any>(null);
  private connectionEstablished = new BehaviorSubject<boolean>(false);
  private userId: string;

  public messages$ = this.messageSubject.asObservable();
  // public connectionEstablished$ = this.connectionEstablished.asObservable();

  constructor(authService: AuthService, private readonly alertService: AlertsService) {
    this.userId = authService.getUserId() || '';
    authService.currentUserSubject.subscribe(currentUserSubject => {
      this.userId = authService.getUserId() || '';
    });
  }

  public startConnection = (): Promise<void> => {
    return new Promise((resolve, reject) => {
      const url = `${environment.baseUrl}/notificationHub?userId=${this.userId}`;

      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(url, {
          skipNegotiation: true,
          transport: signalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect()
        .build();

      this.hubConnection
        .start()
        .then(() => {
          // console.log('SignalR соединение установлено с userId:', this.userId);
          this.connectionEstablished.next(true);
          this.addReceiveMessageListener();
          resolve();
        })
        .catch(err => {
          console.error('Ошибка при установке SignalR соединения:', err);
          reject(err);
        });
    });
  }

  private addReceiveMessageListener = () => {
    if (this.hubConnection) {
      this.hubConnection.on('ReceiveMessage', (userId, message) => {
        // console.log('Получено сообщение через SignalR:', { userId, message });
        this.messageSubject.next({ user: userId, message });
      });
    }
  }

  // public setUserId(userId: string): void {
  //   this.userId = userId;
  //   localStorage.setItem('userId', userId);
  //
  //   if (this.connectionEstablished.value) {
  //     this.hubConnection?.stop().then(() => {
  //       this.startConnection();
  //     });
  //   }
  // }

  // public sendMessage = async (userId: string, message: string): Promise<void> => {
  //   if (!this.connectionEstablished.value) {
  //     console.log('Соединение не установлено, ожидание...');
  //     await this.waitForConnection();
  //   }
  //
  //   if (this.hubConnection &&
  //     this.hubConnection.state === signalR.HubConnectionState.Connected) {
  //     return this.hubConnection.invoke('SendMessage', userId, message);
  //   } else {
  //     throw new Error('Невозможно отправить сообщение - соединение не в состоянии Connected');
  //   }
  // }

  // private waitForConnection(): Promise<void> {
  //   return new Promise((resolve) => {
  //     const subscription = this.connectionEstablished$.subscribe(isConnected => {
  //       if (isConnected) {
  //         subscription.unsubscribe();
  //         resolve();
  //       }
  //     });
  //
  //     setTimeout(() => {
  //       subscription.unsubscribe();
  //       resolve();
  //     }, 5000);
  //   });
  // }
}
