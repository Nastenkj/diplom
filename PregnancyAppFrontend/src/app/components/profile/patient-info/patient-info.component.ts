import {Component, Input, OnInit} from '@angular/core';
import {PatientsService} from "../../../shared-services/patients/patients.service";
import {ActivatedRoute, RouterLink} from "@angular/router";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {UserDto} from "../../../dtos/patients/user-dto";
import {finalize} from "rxjs";
import {NgIf} from "@angular/common";
import {TuiInputDateModule, TuiInputModule} from "@taiga-ui/legacy";
import {TuiButton, TuiError, TuiTitle} from "@taiga-ui/core";
import {HasPermissionDirective} from "../../../directives/has-permission.directive";
import {SurveyTabsetComponent} from "../survey-tabset/survey-tabset.component";
// Добавленный импорт
import {TuiDay} from '@taiga-ui/cdk';
import {AlertsService} from "../../../shared-services/alerts/alerts.service";

@Component({
  selector: 'app-patient',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    TuiInputModule,
    NgIf,
    TuiError,
    TuiInputDateModule,
    TuiButton,
    HasPermissionDirective,
    RouterLink,
    SurveyTabsetComponent,
    TuiTitle
  ],
  templateUrl: './patient-info.component.html',
  styleUrl: './patient-info.component.scss'
})
export class PatientInfoComponent implements OnInit {
  form!: FormGroup;

  // Used to determine whether we see page as a doctor or as an admin
  @Input() patientId: string | null = null;
  @Input() disableEditAbility: boolean = true;

  isSubmitting = false;

  constructor(
    private patientService: PatientsService,
    private notificationService: AlertsService
  ) {}

  ngOnInit(): void {
    this.form = new FormGroup({
      id: new FormControl(null, []), // userId
      fullName: new FormControl(null, [Validators.required]),
      birthDate: new FormControl(null, [Validators.required]),
      phoneNumber: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      trustedPersonFullName: new FormControl(null, [Validators.required]),
      trustedPersonPhoneNumber: new FormControl(null, [Validators.required]),
      trustedPersonEmail: new FormControl(null, [Validators.required, Validators.email]),
      insuranceNumber: new FormControl(null, [Validators.required]),
    });

    this.loadPatientData();

    if (this.disableEditAbility) {
      this.form.disable();
    }
  }

  loadPatientData(): void {
    if (this.patientId) {
      this.patientService.getPatient(this.patientId).subscribe({
        next: (patient: UserDto) => {
          this.updateFormFromBackend(patient);
        },
        error: (error) => {
          console.error('Failed to load patient data', error);
        }
      });
    } else {
      this.patientService.getPatient().subscribe({
        next: (patient: UserDto) => {
          this.updateFormFromBackend(patient);
        },
        error: (error) => {
          console.error('Failed to load patient data', error);
        }
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.form.value;

    let birthDate = formValue.birthDate;
    // Преобразуем TuiDay в строку даты для отправки на сервер
    if (birthDate instanceof TuiDay) {
      birthDate = `${birthDate.year}-${String(birthDate.month + 1).padStart(2, '0')}-${String(birthDate.day).padStart(2, '0')}`;
    } else if (birthDate instanceof Date) {
      birthDate = birthDate.toISOString().split('T')[0];
    }

    const updatedPatient: UserDto = {
      id: this.patientId as string,
      ...formValue,
      birthDate
    };

    this.patientService.editPatient(updatedPatient)
      .pipe(finalize(() => {
        this.isSubmitting = false
      }))
      .subscribe({
        next: (patient) => {
          this.updateFormFromBackend(patient);
          this.notificationService.alertPositive("Информация успешно обновлена.");
        },
        error: (error) => {
          this.loadPatientData();
          console.error('Failed to update patient information', error);
          this.notificationService.alertPositive("Произошла ошибка при обновлении информации. Попробуйте позже.");
        }
      });
  }

  onCancel(): void {
    // console.log('cancel');
  }

  updateFormFromBackend(patient: UserDto): void {
    let tuiDate = null;

    if (patient.birthDate) {
      const [year, month, day] = patient.birthDate.split('-').map(Number);
      // @ts-ignore
      tuiDate = new TuiDay(year, month - 1, day);
    }

    const formattedPatient = {
      ...patient,
      birthDate: tuiDate
    };

    this.form.patchValue(formattedPatient);
  }
}
