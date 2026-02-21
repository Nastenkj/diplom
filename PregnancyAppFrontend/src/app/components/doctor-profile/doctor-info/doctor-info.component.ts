import {Component, Input, OnInit} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {UserDto} from "../../../dtos/patients/user-dto";
import {finalize} from "rxjs";
import {DoctorsService} from "../../../shared-services/doctors/doctors.service";
import {HasPermissionDirective} from "../../../directives/has-permission.directive";
import {TuiButton, TuiError, TuiTitle} from "@taiga-ui/core";
import {TuiInputDateModule, TuiInputModule} from "@taiga-ui/legacy";
import {RouterLink} from "@angular/router";
import {AlertsService} from "../../../shared-services/alerts/alerts.service";

@Component({
  selector: 'app-doctor-info',
  standalone: true,
    imports: [
        HasPermissionDirective,
        ReactiveFormsModule,
        TuiButton,
        TuiError,
        TuiInputDateModule,
        TuiInputModule,
        RouterLink,
        TuiTitle
    ],
  templateUrl: './doctor-info.component.html',
  styleUrl: './doctor-info.component.scss'
})
export class DoctorInfoComponent implements OnInit {
  form!: FormGroup;

  @Input() doctorId: string | null = null;

  isSubmitting = false;

  constructor(
    private doctorService: DoctorsService,
    private notificationService: AlertsService
  ) {}

  ngOnInit(): void {
    this.form = new FormGroup({
      id: new FormControl(null, []), // userId
      fullName: new FormControl(null, [Validators.required]),
      birthDate: new FormControl(null, [Validators.required]),
      phoneNumber: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required, Validators.email]),
    });

    this.loadDoctorData();
  }

  loadDoctorData(): void {
    if (this.doctorId) {
      this.doctorService.getDoctor(this.doctorId).subscribe({
        next: (patient: UserDto) => {
          this.updateFormFromBackend(patient);
        },
        error: (error) => {
          console.error('Failed to load doctor data', error);
        }
      });
    } else {
      this.doctorService.getDoctor().subscribe({
        next: (patient: UserDto) => {
          this.updateFormFromBackend(patient);
        },
        error: (error) => {
          console.error('Failed to load doctor data', error);
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
    if (birthDate instanceof Date) {
      birthDate = birthDate.toISOString().split('T')[0];
    }

    const updatedPatient: UserDto = {
      id: this.doctorId as string,
      ...formValue,
      birthDate
    };

    // console.log(updatedPatient);
    this.doctorService.editDoctor(updatedPatient)
      .pipe(finalize(() => {
        this.isSubmitting = false
      }))
      .subscribe({
        next: (patient) => {
          this.updateFormFromBackend(patient);
          this.notificationService.alertPositive("Информация успешно обновлена.");
        },
        error: (error) => {
          this.loadDoctorData();
          console.error('Failed to update doctor information', error);
          this.notificationService.alertPositive("Произошла ошибка при обновлении информации. Попробуйте позже.");
        }
      });
  }

  updateFormFromBackend(patient: UserDto): void {
    const formattedPatient = {
      ...patient,
      birthDate: patient.birthDate ? new Date(patient.birthDate) : null
    };
    this.form.patchValue(formattedPatient);
  }
}
