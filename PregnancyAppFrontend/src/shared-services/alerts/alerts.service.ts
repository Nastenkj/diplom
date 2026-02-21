import {inject, Injectable} from '@angular/core';
import {TuiAlertService} from "@taiga-ui/core";

@Injectable({
  providedIn: 'root'
})
export class AlertsService {
  private readonly alerts = inject(TuiAlertService);

  alertPositive(message: string) {
    this.alerts
      .open(message, {appearance: 'positive', label: 'Действие выполнено успешно!'})
      .subscribe();
  }

  alertNegative(message: string) {
    this.alerts
      .open(message, {appearance: 'negative', label: 'Что-то пошло не так'})
      .subscribe();
  }
}
