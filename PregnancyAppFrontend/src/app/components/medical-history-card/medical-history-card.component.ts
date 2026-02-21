import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {MedicalHistoryComponent} from "./medical-history/medical-history.component";
import {Subject} from "rxjs";
import {TuiButton, TuiFormatDatePipe, TuiIcon, TuiSurface, TuiTitle} from "@taiga-ui/core";
import {TuiCardMedium} from "@taiga-ui/layout";
import {TuiBadge} from "@taiga-ui/kit";
import {AsyncPipe, NgIf} from "@angular/common";
import {MedicalHistoriesService} from "../../shared-services/surveys/medical-history-surveys.service";
import {AlertsService} from "../../shared-services/alerts/alerts.service";

@Component({
  selector: 'app-medical-history-card',
  standalone: true,
  imports: [
    MedicalHistoryComponent,
    TuiButton,
    TuiCardMedium,
    TuiSurface,
    TuiTitle,
    TuiBadge,
    TuiIcon,
    AsyncPipe,
    TuiFormatDatePipe,
    NgIf
  ],
  templateUrl: './medical-history-card.component.html',
  styleUrls: ['./medical-history-card.component.scss', '../../../styles.scss']
})
export class MedicalHistoryCardComponent implements OnInit{
  openMedicalHistory$ = new Subject<void>();
  now = Date.now();
  medicalHistoryPopulated: boolean = true;
  @Output() medicalHistoryPopulatedEvent = new EventEmitter<boolean>();

  constructor(private medicalHistoriesService: MedicalHistoriesService,
              private alertsService: AlertsService) {
  }

  ngOnInit(): void {
        this.medicalHistoriesService.getMedicalHistory().subscribe((result) => {
          this.medicalHistoryPopulated = result !== null;
          this.medicalHistoryPopulatedEvent.next(this.medicalHistoryPopulated);
          // TODO error handle
        });
  }

  onObserverComplete() {
    this.medicalHistoriesService.getMedicalHistory().subscribe((result) => {
      this.medicalHistoryPopulated = result !== null;
      this.medicalHistoryPopulatedEvent.next(this.medicalHistoryPopulated);
      // TODO error handle
    });
  }
}
