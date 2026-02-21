import {Component, inject, OnInit} from '@angular/core';
import {Subject} from "rxjs";
import {DailySurveyComponent} from "./daily-survey/daily-survey.component";
import {TuiButton, TuiFormatDatePipe, TuiIcon, TuiSurface, TuiTitle} from "@taiga-ui/core";
import {TuiBadge} from "@taiga-ui/kit";
import {TuiCardMedium} from "@taiga-ui/layout";
import {AsyncPipe, NgIf} from "@angular/common";
import {DailySurveyService} from "../../shared-services/surveys/daily-surveys.service";
import {calculateSurveyTime} from "../../utils/date/calculate-survey-time";
import {isNowSameDayOrLater} from "../../utils/date/is-now-same-day-or-later";

@Component({
  selector: 'app-daily-survey-card',
  standalone: true,
  imports: [
    DailySurveyComponent,
    TuiButton,
    TuiBadge,
    TuiCardMedium,
    TuiIcon,
    TuiSurface,
    TuiTitle,
    AsyncPipe,
    TuiFormatDatePipe,
    NgIf
  ],
  templateUrl: './daily-survey-card.component.html',
  styleUrls: ['./daily-survey-card.component.scss', '../../../styles.scss']
})
export class DailySurveyCardComponent implements OnInit {
  openDailySurvey$ = new Subject<void>();
  whenNeedToTakeSurvey: Date = null!;
  private readonly dailySurveyService = inject(DailySurveyService);

  ngOnInit() {
    this.refreshWhenNeedToTakeSurvey();
  }

  onSurveySubmit() {
    this.refreshWhenNeedToTakeSurvey();
  }

  private refreshWhenNeedToTakeSurvey(): void {
    this.dailySurveyService.getLatestDailySurveyDate().subscribe((dateUtc: Date | null) => {
      this.whenNeedToTakeSurvey = calculateSurveyTime(dateUtc, 24);
    });
  }

  get isDisabled(): boolean {
    if (this.whenNeedToTakeSurvey === null) return true;
    return !isNowSameDayOrLater(this.whenNeedToTakeSurvey);
  }
}
