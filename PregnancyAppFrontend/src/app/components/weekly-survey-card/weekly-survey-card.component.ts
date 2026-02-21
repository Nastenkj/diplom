import {Component, inject, OnInit} from '@angular/core';
import {WeeklySurveyComponent} from "./weekly-survey/weekly-survey.component";
import {Subject} from "rxjs";
import {TuiButton, TuiFormatDatePipe, TuiIcon, TuiSurface, TuiTitle} from "@taiga-ui/core";
import {TuiBadge} from "@taiga-ui/kit";
import {TuiCardMedium} from "@taiga-ui/layout";
import {AsyncPipe} from "@angular/common";
import {calculateSurveyTime} from "../../utils/date/calculate-survey-time";
import {WeeklySurveysService} from "../../shared-services/surveys/weekly-surveys.service";
import {isNowSameDayOrLater} from "../../utils/date/is-now-same-day-or-later";

@Component({
  selector: 'app-weekly-survey-card',
  standalone: true,
  imports: [
    WeeklySurveyComponent,
    TuiButton,
    TuiBadge,
    TuiCardMedium,
    TuiIcon,
    TuiSurface,
    TuiTitle,
    TuiFormatDatePipe,
    AsyncPipe
  ],
  templateUrl: './weekly-survey-card.component.html',
  styleUrls: ['./weekly-survey-card.component.scss', '../../../styles.scss']
})
export class WeeklySurveyCardComponent implements OnInit {
  openWeeklySurvey$ = new Subject<void>();
  whenNeedToTakeSurvey: Date = null!;
  private readonly weeklySurveyService = inject(WeeklySurveysService);

  ngOnInit() {
    this.refreshWhenNeedToTakeSurvey();
  }

  onSurveySubmit() {
    this.refreshWhenNeedToTakeSurvey();
  }

  private refreshWhenNeedToTakeSurvey(): void {
    this.weeklySurveyService.getLatestWeeklySurveyDate().subscribe((dateUtc: Date | null) => {
      this.whenNeedToTakeSurvey = calculateSurveyTime(dateUtc, 24 * 7);
    });
  }

    get isDisabled(): boolean {
      if (this.whenNeedToTakeSurvey === null) return true;
      return !isNowSameDayOrLater(this.whenNeedToTakeSurvey);
  }
}
