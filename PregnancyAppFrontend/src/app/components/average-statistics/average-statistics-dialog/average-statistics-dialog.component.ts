import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { inject } from '@angular/core';
import {
  TuiDialogService,
  TuiButton,
  TuiDataListComponent,
  TuiOption,
  TuiTitle,
  TuiIcon,
  TuiSurface
} from "@taiga-ui/core";
import {
  TuiAppBarBack,
  TuiAppBarComponent,
  TuiAppBarDirective,
  TuiAppBarSizeDirective,
  TuiCardMedium,
  TuiHeader
} from "@taiga-ui/layout";
import { TuiProgressBar } from "@taiga-ui/kit";
import { NgForOf, NgIf } from "@angular/common";
import { Observable, Subject, takeUntil } from "rxjs";
import { ObservationParameterWithNormDto } from "../../../dtos/statistics/observation-parameter-with-norm";
import { StatisticFields } from "../../../dtos/internal/statistic-fields-enum";
import { displayNames } from "../../../dtos/internal/parameter-display-names";
import { HasPermissionDirective } from "../../../directives/has-permission.directive";
import { AlertsService } from "../../../shared-services/alerts/alerts.service";
import {ExcelExportService} from "../../../shared-services/surveys/excel-export.service";

@Component({
  selector: 'app-average-statistics-dialog',
  standalone: true,
  imports: [
    NgIf,
    NgForOf,
    TuiTitle,
    TuiAppBarBack,
    TuiAppBarComponent,
    TuiAppBarDirective,
    TuiAppBarSizeDirective,
    TuiButton,
    TuiDataListComponent,
    TuiHeader,
    TuiProgressBar,
    HasPermissionDirective,
    TuiIcon,
    TuiCardMedium,
    TuiSurface
  ],
  templateUrl: './average-statistics-dialog.component.html',
  styleUrls: ['./average-statistics-dialog.component.scss']
})
export class AverageStatisticsDialogComponent implements OnInit, OnDestroy {
  @Input() openEvent!: Observable<void>;
  @Input() statisticsData: ObservationParameterWithNormDto[] = [];
  @Input() patientId!: string;
  @Output() dialogClosed = new EventEmitter<void>();
  @ViewChild('template', { static: true })
  public templateRef!: TemplateRef<any>;

  private readonly dialogs = inject(TuiDialogService);
  private readonly excelExportService = inject(ExcelExportService);
  private readonly alertsService = inject(AlertsService);
  private readonly destroy$ = new Subject<void>();

  protected open(template: TemplateRef<any>): void {
    this.dialogs.open(template, {
      label: '',
      size: 'fullscreen',
      closeable: false,
      dismissible: false,
    }).subscribe();
  }

  onClose($event: Event, observer: { complete: () => void }) {
    $event.preventDefault();
    $event.stopPropagation();
    this.dialogClosed.emit();
    observer.complete();
  }

  downloadStatistics($event: Event) {
    $event.preventDefault();
    $event.stopPropagation();

    this.excelExportService.downloadObservationParameters(this.patientId).subscribe({
      next: (data: Blob) => {
        this.excelExportService.saveFile(data, 'ПараметрыНаблюдения.xlsx');
        this.alertsService.alertPositive('Файл успешно скачан');
      },
      error: (error: unknown) => {
        this.alertsService.alertNegative('Не удалось скачать параметры наблюдения');
      }
    });
  }

  ngOnInit() {
    this.openEvent
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.open(this.templateRef);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  isWithinNorm(parameter: ObservationParameterWithNormDto): boolean {
    if (parameter.lowerBound !== undefined && parameter.upperBound !== undefined) {
      return parameter.value >= parameter.lowerBound && parameter.value <= parameter.upperBound;
    }
    return true;
  }

  getParameterDisplayName(paramName: StatisticFields): string {
    return displayNames[paramName] || paramName;
  }
}
