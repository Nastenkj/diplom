import {Component, Input, OnInit} from '@angular/core';
import {MedicalHistoriesService} from "../../../../shared-services/surveys/medical-history-surveys.service";
import {MedicalHistoryDto} from "../../../../dtos/medical-history/medical-history-dto";
import {JsonPipe, NgIf} from "@angular/common";
import {MedicalHistoryComponent} from "../../../medical-history-card/medical-history/medical-history.component";
import {TuiButton} from "@taiga-ui/core";
import {Subject} from "rxjs";

@Component({
  selector: 'app-medical-history-tab',
  standalone: true,
  imports: [
    JsonPipe,
    MedicalHistoryComponent,
    TuiButton,
    NgIf
  ],
  templateUrl: './medical-history-tab.component.html',
  styleUrl: './medical-history-tab.component.scss'
})
export class MedicalHistoryTabComponent implements OnInit{
  @Input() patientId: string | null = null;
  openMedicalHistory$ = new Subject<void>();
  medicalHistory: MedicalHistoryDto | null | undefined = undefined;

  constructor(private readonly medicalHistoriesService: MedicalHistoriesService) {
  }

  ngOnInit() {
    this.loadMedicalHistory();
  }

  loadMedicalHistory(): void {
    this.medicalHistoriesService.getMedicalHistory(this.patientId).subscribe({
      next: (medicalHistory: MedicalHistoryDto | null) => {
        this.medicalHistory = medicalHistory;
      },
      error: (error) => {
        console.error('Failed to load medical history', error);
      }
    });
  }
}
