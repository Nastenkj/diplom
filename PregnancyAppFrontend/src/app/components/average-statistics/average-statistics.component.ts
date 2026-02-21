import { Component, Input } from '@angular/core';
import { StatisticsService } from "../../shared-services/statistics/statistics.service";
import { TuiButton } from "@taiga-ui/core";
import { AverageStatisticsDialogComponent } from "./average-statistics-dialog/average-statistics-dialog.component";
import { Subject } from "rxjs";
import {ObservationParameterWithNormDto} from "../../dtos/statistics/observation-parameter-with-norm";
import {AlertsService} from "../../shared-services/alerts/alerts.service";
import {AppDoesntHavePermissionDirective} from "../../directives/app-doesnt-have-permission.directive";

@Component({
  selector: 'app-average-statistics',
  standalone: true,
  imports: [
    TuiButton,
    AverageStatisticsDialogComponent,
    AppDoesntHavePermissionDirective
  ],
  templateUrl: './average-statistics.component.html',
  styleUrl: './average-statistics.component.scss'
})
export class AverageStatisticsComponent {
  @Input() patientId!: string;
  openDialog$ = new Subject<void>();
  statisticsData: ObservationParameterWithNormDto[] = [];

  constructor(private readonly statisticsService: StatisticsService, private readonly notificationService: AlertsService) {
  }

  openStatisticsDialog() {
    this.statisticsService.getObservationParametersStatistics(this.patientId).subscribe((res) => {
      if (res.length > 0) {
        this.statisticsData = res;
        this.openDialog$.next();
      }
      else {
        this.notificationService.alertNegative("Результаты опросников пока не обработаны алгоритмом или вы не заполнили один из запросов. Попробуйте позже.")
      }
    });
  }
}
