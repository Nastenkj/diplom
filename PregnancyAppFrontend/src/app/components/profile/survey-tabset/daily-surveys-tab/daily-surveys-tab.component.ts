import {AsyncPipe, DatePipe, NgForOf, NgIf} from '@angular/common';
import {Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, TemplateRef, ViewChild} from '@angular/core';
import {DailySurveyDto} from "../../../../dtos/daily-survey/daily-survey-dto";
import {DailySurveyService} from "../../../../shared-services/surveys/daily-surveys.service";
import {TuiTable} from '@taiga-ui/addon-table';
import {TuiLet} from '@taiga-ui/cdk';
import {BehaviorSubject, Observable, Subject, switchMap, takeUntil, tap} from 'rxjs';
import {TuiButton, TuiLoader, TuiDialogService} from "@taiga-ui/core";
import {inject} from "@angular/core";
import {DailySurveyComponent} from "../../../daily-survey-card/daily-survey/daily-survey.component";
import {TuiTablePagination, TuiTablePaginationEvent} from '@taiga-ui/addon-table';
import {ExcelExportService} from "../../../../shared-services/surveys/excel-export.service";
import {DailyHealthPredictionsService} from "../../../../shared-services/surveys/daily-health-predictions.service";
import {DailyHealthPredictionResultDto} from "../../../../dtos/daily-health-prediction/daily-health-prediction-result-dto";

@Component({
  selector: 'app-daily-surveys-tab',
  standalone: true,
  imports: [
    NgIf,
    NgForOf,
    AsyncPipe,
    DatePipe,
    TuiTable,
    TuiLet,
    TuiButton,
    TuiLoader,
    DailySurveyComponent,
    DailySurveyComponent,
    TuiTablePagination
  ],
  templateUrl: './daily-surveys-tab.component.html',
  styleUrl: './daily-surveys-tab.component.scss',
})
export class DailySurveysTabComponent implements OnChanges, OnInit, OnDestroy {
  @Input() patientId: string | null = null;

  readonly columns = ['date', 'action'];

  private readonly refresh$ = new BehaviorSubject<void>(undefined);
  readonly loading$ = new BehaviorSubject<boolean>(false);

  openDailySurvey$ = new Subject<void>();

  selectedSurvey: DailySurveyDto | null = null;

  private readonly destroy$ = new Subject<void>();

  // ML результаты
  selectedMlResult: DailyHealthPredictionResultDto | null = null;
  mlLoading = false;
  mlError: string | null = null;

  // ML диалог
  mlDialogOpen = false;

  @ViewChild('mlDialogTemplate', {static: true})
  private readonly mlDialogTemplate!: TemplateRef<any>;

  private readonly dialogs = inject(TuiDialogService);

  // Добавляем состояние пагинации
  page = 0; // Страница (начиная с 0)
  size = 10; // Количество элементов на странице
  total = 0; // Общее количество элементов

  // Сохраняем все данные для локальной пагинации
  allSurveys: DailySurveyDto[] = [];

  // Данные для текущей страницы
  paginatedData: DailySurveyDto[] = [];

  readonly data$: Observable<DailySurveyDto[]> = this.refresh$.pipe(
    switchMap(() => {
      this.loading$.next(true);

      return this.dailySurveyService.getDailySurveysForUser(this.patientId).pipe(
        tap((data) => {
          this.loading$.next(false);
          this.allSurveys = data;
          this.total = data.length;
          this.updatePaginatedData();
        })
      );
    })
  );

  constructor(
    private dailySurveyService: DailySurveyService,
    private excelExportService: ExcelExportService,
    private dailyHealthPredictionsService: DailyHealthPredictionsService
  ) {}

  ngOnInit(): void {
    this.data$.pipe(takeUntil(this.destroy$)).subscribe();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['patientId'] && this.patientId) {
      this.refresh$.next();
      // сбрасываем выбранные результаты ML при смене пациента
      this.selectedMlResult = null;
      this.mlError = null;
    }
  }

  updatePaginatedData(): void {
    const start = this.page * this.size;
    const end = start + this.size;
    this.paginatedData = this.allSurveys.slice(start, end);
  }

  onPagination({page, size}: TuiTablePaginationEvent): void {
    this.page = page;
    this.size = size;
    this.updatePaginatedData();
  }

  viewSurveyResult(surveyId: string): void {
    this.selectedSurvey = null;
    this.dailySurveyService.getDailySurveyById(surveyId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (survey) => {
          this.selectedSurvey = survey;
          // console.log(this.selectedSurvey);

          setTimeout(() => {
            this.openDailySurvey$.next();
          }, 0);
        },
        error: (error) => {
          console.error('Error fetching survey:', error);
        }
      });
  }

  downloadSurveyResult(surveyId: string): void {
    this.excelExportService.downloadDailySurvey(surveyId).subscribe({
      next: (data) => {
        const url = window.URL.createObjectURL(data);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'ЕжедневныйОпрос.xlsx';
        link.click();

        setTimeout(() => {
          window.URL.revokeObjectURL(url);
          link.remove();
        }, 100);
      },
      error: (error) => {
        console.error('Error downloading survey', error);
      }
    });
  }

  downloadSurveyResults(): void {
    if (this.patientId) {
      this.excelExportService.downloadDailySurveys(this.patientId).subscribe({
        next: (data) => {
          const url = window.URL.createObjectURL(data);
          const link = document.createElement('a');
          link.href = url;
          link.download = 'ЕжедневныеОпросы.xlsx';
          link.click();

          setTimeout(() => {
            window.URL.revokeObjectURL(url);
            link.remove();
          }, 100);
        },
        error: (error) => {
          console.error('Error downloading survey', error);
        }
      });
    }
  }

  viewSurveyMlResults(dailySurvey: DailySurveyDto): void {
    this.selectedMlResult = null;
    this.mlError = null;
    this.mlDialogOpen = false;

    // Ищем результат по dailySurveyId, а не по дате
    if (!dailySurvey.id) {
      this.mlError = 'Не удалось определить id ежедневного опроса.';
      this.mlDialogOpen = false;
      // показываем диалог даже при ошибке
      this.dialogs.open(this.mlDialogTemplate, {
        label: '',
        size: 'm',
        closeable: true,
        dismissible: true,
      }).subscribe();
      return;
    }

    this.mlLoading = true;

    this.dailyHealthPredictionsService.getResultByDailySurveyId(dailySurvey.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.selectedMlResult = result ?? null;
          this.mlLoading = false;

          this.dialogs.open(this.mlDialogTemplate, {
            label: '',
            size: 'm',
            closeable: true,
            dismissible: true,
          }).subscribe();
        },
        error: (err) => {
          console.error('Error fetching ML results:', err);
          this.mlError = 'Ошибка загрузки результатов ML. Попробуйте позже.';
          this.mlLoading = false;

          this.dialogs.open(this.mlDialogTemplate, {
            label: '',
            size: 'm',
            closeable: true,
            dismissible: true,
          }).subscribe();
        }
      });
  }

  onCloseMlDialog(): void {
    this.mlDialogOpen = false;
    this.selectedMlResult = null;
  }

  getPredictionLabel(prediction: number): string {
    return prediction === 0 ? 'Норма' : prediction === 1 ? 'Предупреждение' : 'Патология';
  }

  getDoctorActionsText(prediction: number): string | null {
    if (prediction === 1) return 'Нужна консультация врача.';
    if (prediction === 2) return 'Немедленная госпитализация.';
    return null;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.openDailySurvey$.complete();
  }
}
