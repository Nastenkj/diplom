import {Component} from '@angular/core';
import {MedicalHistoryComponent} from "../medical-history-card/medical-history/medical-history.component";
import {WeeklySurveyComponent} from "../weekly-survey-card/weekly-survey/weekly-survey.component";
import {DailySurveyComponent} from "../daily-survey-card/daily-survey/daily-survey.component";
import {TuiButton, TuiFormatDatePipe, TuiFormatDateService, TuiIcon, TuiSurface, TuiTitle} from "@taiga-ui/core";
import {LoginService} from "../../shared-services/auth/login.service";
import {TuiCardMedium, TuiNavigation} from "@taiga-ui/layout";
import {DefaultDateFormatterService} from "../../shared-services/date/default-date-formatter.service";
import {AsyncPipe, NgIf} from "@angular/common";
import {MedicalHistoryCardComponent} from "../medical-history-card/medical-history-card.component";
import {DailySurveyCardComponent} from "../daily-survey-card/daily-survey-card.component";
import {WeeklySurveyCardComponent} from "../weekly-survey-card/weekly-survey-card.component";
import {HasPermissionDirective} from "../../directives/has-permission.directive";
import {CommunicationLinkComponent} from "../communication-link/communication-link.component";
import {TuiBadge} from "@taiga-ui/kit";

@Component({
  selector: 'app-demo',
  standalone: true,
  imports: [
    MedicalHistoryComponent,
    WeeklySurveyComponent,
    DailySurveyComponent,
    TuiButton,
    TuiNavigation,
    TuiFormatDatePipe,
    AsyncPipe,
    MedicalHistoryCardComponent,
    DailySurveyCardComponent,
    WeeklySurveyCardComponent,
    HasPermissionDirective,
    CommunicationLinkComponent,
    NgIf,
    TuiBadge,
    TuiCardMedium,
    TuiIcon,
    TuiSurface,
    TuiTitle
  ],
  templateUrl: './demo.component.html',
  styleUrls: ['./demo.component.scss', '../../../styles.scss'],
  providers: [
    {
      provide: TuiFormatDateService,
      useClass: DefaultDateFormatterService,
    },
  ]
})
export class DemoComponent {
  protected readonly now = Date.now();

  showMedicalHistoryCard: boolean = true;

  constructor(private loginService: LoginService) {
  }

  onMedicalHistoryPopulatedReceived(isPopulated: boolean) {
    this.showMedicalHistoryCard = !isPopulated;
  }

}
