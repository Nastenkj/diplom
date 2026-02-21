import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {NgIf} from "@angular/common";
import {PatientInfoComponent} from "../profile/patient-info/patient-info.component";
import {StatisticComponent} from "../statistic/statistic.component";
import {SurveyTabsetComponent} from "../profile/survey-tabset/survey-tabset.component";
import {TuiTab, TuiTabsWithMore} from "@taiga-ui/kit";
import {DoctorInfoComponent} from "./doctor-info/doctor-info.component";
import {TuiItem} from "@taiga-ui/cdk";
import {ObservationParametersComponent} from "../observation-parameters/observation-parameters.component";
import {HasPermissionDirective} from "../../directives/has-permission.directive";

@Component({
  selector: 'app-doctor-profile',
  standalone: true,
  imports: [
    NgIf,
    PatientInfoComponent,
    StatisticComponent,
    SurveyTabsetComponent,
    TuiTab,
    TuiTabsWithMore,
    DoctorInfoComponent,
    TuiItem,
    ObservationParametersComponent,
    HasPermissionDirective
  ],
  templateUrl: './doctor-profile.component.html',
  styleUrl: './doctor-profile.component.scss'
})
export class DoctorProfileComponent {
  protected activeItemIndex: number = 0;

  doctorId: string | null = null;
  allowShowTabs: boolean | null = null;

  constructor(private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.route.params.subscribe({
      next: params => {
        if (params['id']) {
          // Came from table (admin)
          this.doctorId = params['id'];
        }
      }
    });

    this.allowShowTabs = true;
  }
}
