import {Component, OnInit} from '@angular/core';
import {PatientInfoComponent} from "./patient-info/patient-info.component";
import {SurveyTabsetComponent} from "./survey-tabset/survey-tabset.component";
import {TuiTab, TuiTabs, TuiTabsWithMore} from "@taiga-ui/kit";
import {NgIf} from "@angular/common";
import {ActivatedRoute} from "@angular/router";
import {AuthorizationService} from "../../shared-services/auth/authorization.service";
import {StatisticComponent} from "../statistic/statistic.component";
import {HasPermissionDirective} from "../../directives/has-permission.directive";

@Component({
  selector: 'app-patient-profile',
  standalone: true,
  imports: [
    PatientInfoComponent,
    SurveyTabsetComponent,
    TuiTab,
    TuiTabsWithMore,
    TuiTabs,
    NgIf,
    StatisticComponent,
    HasPermissionDirective
  ],
  templateUrl: './patient-profile.component.html',
  styleUrl: './patient-profile.component.scss'
})
export class PatientProfileComponent implements OnInit {
  protected activeItemIndex: number = 0;

  patientId: string | null = null;
  disableEditAbility: boolean | null = null;

  constructor(private route: ActivatedRoute,
              private authorizationService: AuthorizationService,) {
  }

  ngOnInit() {
    // if doctor - don't allow to edit
    this.route.params.subscribe({
      next: params => {
        if (params['id']) {
          // Came from table (admin or doctor)
          this.patientId = params['id'];
          console.log(this.patientId);
          this.disableEditAbility = !this.authorizationService.hasPermissionForFeature('patient.edit');
        }
      }
    });

    this.disableEditAbility = !this.authorizationService.hasPermissionForFeature('patient.edit');
  }

  get allowShowTabs() {
    return this.disableEditAbility !== null;
  }
}
