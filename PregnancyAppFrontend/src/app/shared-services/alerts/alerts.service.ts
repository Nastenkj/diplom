 import {inject, Injectable} from '@angular/core';
import {TuiAlertService} from "@taiga-ui/core";

@Injectable({
  providedIn: 'root'
})
export class AlertsService {
  private readonly alerts = inject(TuiAlertService);

  alertPositive(message: string) {
    this.alerts
      .open(message, {
        appearance: 'positive',
        label: 'Действие выполнено успешно!',
        // Важно: уведомления для ML-обработки должны не исчезать сами.
        // Закрытие — только по крестику
        closeable: true,
      })
      .subscribe();
  }



  alertNegative(message: string) {
    this.alerts
      .open(message, {
        appearance: 'negative',
        label: 'Что-то пошло не так',
        // duration: 0 — параметр может отсутствовать в текущих типах TuiAlertOptions
        // dismissible: true,
        closeable: true,
      })
      .subscribe();
  }
}

