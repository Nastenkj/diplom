import {Component, inject, Input} from '@angular/core';
import {TuiButton} from '@taiga-ui/core';
import {TuiDialogContext, TuiDialogService, TuiHostedDialog} from '@taiga-ui/core';

@Component({
  selector: 'app-ml-completed-dialog',
  standalone: true,
  imports: [TuiButton],
  template: `
    <div style="padding: 16px; max-width: 520px;">
      <div style="font-size: 18px; font-weight: 700; margin-bottom: 8px;">Обработка ML завершена</div>
      <div style="line-height: 1.4; color: rgba(0,0,0,.8);">
        Результат можно посмотреть в профиле в разделе информация об опросниках — ежедневные опросы.
      </div>
      <div style="display:flex; justify-content:flex-end; margin-top: 16px;">
        <button tuiButton type="button" appearance="primary" (click)="close()">Ок</button>
      </div>
    </div>
  `,
})
export class MlCompletedDialogComponent {
  private readonly dialogs = inject(TuiDialogService);
  // TuiDialogService для hosted dialog контекста; если не используется в вашей сборке — можно убрать.

  close(): void {
    // Закрываем текущий диалог.
    this.dialogs.complete();
  }
}

