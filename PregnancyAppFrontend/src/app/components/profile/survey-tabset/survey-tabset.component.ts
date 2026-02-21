import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {TuiInputNumber, TuiTabs, TuiTabsWithMore} from "@taiga-ui/kit";
import {TuiItem} from "@taiga-ui/cdk";
import {FormsModule} from "@angular/forms";
import {TuiTextfield} from "@taiga-ui/core";
import {NgIf} from "@angular/common";
import {MedicalHistoryTabComponent} from "./medical-history-tab/medical-history-tab.component";
import {WeeklySurveyCardComponent} from "../../weekly-survey-card/weekly-survey-card.component";
import {DailySurveyCardComponent} from "../../daily-survey-card/daily-survey-card.component";
import {DailySurveysTabComponent} from "./daily-surveys-tab/daily-surveys-tab.component";
import {WeeklySurveysTabComponent} from "./weekly-surveys-tab/weekly-surveys-tab.component";

@Component({
  selector: 'app-survey-tabset',
  standalone: true,
  imports: [
    TuiTabsWithMore,
    TuiItem,
    TuiTabs,
    FormsModule,
    TuiInputNumber,
    TuiTextfield,
    NgIf,
    MedicalHistoryTabComponent,
    WeeklySurveyCardComponent,
    DailySurveyCardComponent,
    DailySurveysTabComponent,
    WeeklySurveysTabComponent,
  ],
  templateUrl: './survey-tabset.component.html',
  styleUrl: './survey-tabset.component.scss'
})
export class SurveyTabsetComponent implements OnInit, OnDestroy {
  @Input() patientId: string | null | undefined = undefined;

  protected activeItemIndex = 0;

  ngOnDestroy(): void {

  }
  ngOnInit(): void {
  }

  changeActiveItemIndex(index: number): void {
    this.activeItemIndex = index;
  }

}
