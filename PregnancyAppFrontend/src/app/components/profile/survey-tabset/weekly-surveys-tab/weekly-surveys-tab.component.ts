import {AsyncPipe, DatePipe, NgForOf, NgIf} from '@angular/common';
import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';
import {WeeklySurveyDto} from "../../../../dtos/weekly-survey/weekly-survey-dto";
import {WeeklySurveysService} from "../../../../shared-services/surveys/weekly-surveys.service";
import {TuiTable} from '@taiga-ui/addon-table';
import {TuiLet} from '@taiga-ui/cdk';
import {BehaviorSubject, Observable, Subject, switchMap, takeUntil, tap} from 'rxjs';
import {TuiButton, TuiLoader} from "@taiga-ui/core";
import {WeeklySurveyComponent} from "../../../weekly-survey-card/weekly-survey/weekly-survey.component";
import {TuiTablePagination, TuiTablePaginationEvent} from '@taiga-ui/addon-table';
import {TuiButtonLoading} from "@taiga-ui/kit";
import {ExcelExportService} from "../../../../shared-services/surveys/excel-export.service";

@Component({
  selector: 'app-weekly-surveys-tab',
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
    WeeklySurveyComponent,
    TuiTablePagination,
    TuiButtonLoading
  ],
  templateUrl: './weekly-surveys-tab.component.html',
  styleUrl: './weekly-surveys-tab.component.scss',
})
export class WeeklySurveysTabComponent implements OnChanges, OnInit, OnDestroy {
  @Input() patientId: string | null = null;

  readonly columns = ['date', 'action'];

  private readonly refresh$ = new BehaviorSubject<void>(undefined);
  readonly loading$ = new BehaviorSubject<boolean>(false);

  openWeeklySurvey$ = new Subject<void>();

  selectedSurvey: WeeklySurveyDto | null = null;

  private readonly destroy$ = new Subject<void>();

  page = 0;
  size = 10;
  total = 0;

  allSurveys: WeeklySurveyDto[] = [];

  paginatedData: WeeklySurveyDto[] = [];

  readonly data$: Observable<WeeklySurveyDto[]> = this.refresh$.pipe(
    switchMap(() => {
      this.loading$.next(true);

      return this.weeklySurveyService.getWeeklySurveysForUser(this.patientId).pipe(
        tap((data) => {
          this.loading$.next(false);
          this.allSurveys = data;
          this.total = data.length;
          this.updatePaginatedData();
        })
      );
    })
  );

  constructor(private weeklySurveyService: WeeklySurveysService, private excelExportService: ExcelExportService) {}

  ngOnInit(): void {
    this.data$.pipe(takeUntil(this.destroy$)).subscribe();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['patientId'] && this.patientId) {
      this.refresh$.next();
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
    this.weeklySurveyService.getWeeklySurveyById(surveyId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (survey) => {
          this.selectedSurvey = survey;
          // console.log(this.selectedSurvey);

          setTimeout(() => {
            this.openWeeklySurvey$.next();
          }, 0);
        },
        error: (error) => {
          console.error('Error fetching survey:', error);
        }
      });
  }

  downloadSurveyResult(surveyId: string): void {
    this.excelExportService.downloadWeeklySurvey(surveyId).subscribe({
      next: (data) => {
        const url = window.URL.createObjectURL(data);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'ЕженедельныйОпрос.xlsx';
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
      this.excelExportService.downloadWeeklySurveys(this.patientId).subscribe({
        next: (data) => {
          const url = window.URL.createObjectURL(data);
          const link = document.createElement('a');
          link.href = url;
          link.download = 'ЕженедельныйОпрос.xlsx';
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

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.openWeeklySurvey$.complete();
  }
}
