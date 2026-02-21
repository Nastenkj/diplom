import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {LoginService} from "../../shared-services/auth/login.service";
import {AlertsService} from "../../shared-services/alerts/alerts.service";
import {AuthService} from "../../auth/auth.service";
import {BaseRegistrationRequestDto} from "../../dtos/auth/registration-request";
import {TuiAppearance, TuiButton, TuiLoader, TuiTitle} from "@taiga-ui/core";
import {TuiCardLarge} from "@taiga-ui/layout";
import {
  TuiInputDateModule,
  TuiInputModule,
  TuiInputPasswordModule,
  TuiInputPhoneModule,
  TuiPrimitiveTextfieldModule
} from "@taiga-ui/legacy";
import {RouterLink} from "@angular/router";

@Component({
  selector: 'app-register-doctor',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    TuiAppearance,
    TuiButton,
    TuiCardLarge,
    TuiInputDateModule,
    TuiInputModule,
    TuiInputPasswordModule,
    TuiInputPhoneModule,
    TuiPrimitiveTextfieldModule,
    TuiTitle,
    RouterLink,
    TuiLoader
  ],
  templateUrl: './register-doctor.component.html',
  styleUrl: './register-doctor.component.scss'
})
export class RegisterDoctorComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;

  constructor(
    private loginService: LoginService,
    private alertsService: AlertsService,
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.form = new FormGroup({
      fullName: new FormControl(null, [Validators.required]),
      birthDate: new FormControl(null, [Validators.required]),
      phoneNumber: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required, Validators.minLength(1), Validators.maxLength(32)]),
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.isLoading = true;
      const formValue = this.form.getRawValue();

      // Format birthDate properly for sending to the backend
      let birthDateStr = '';
      if (formValue.birthDate instanceof Date) {
        birthDateStr = this.formatDate(formValue.birthDate);
      } else if (typeof formValue.birthDate === 'string') {
        // If it's already a string, make sure it's in YYYY-MM-DD format
        const date = new Date(formValue.birthDate);
        birthDateStr = this.formatDate(date);
      } else {
        birthDateStr = formValue.birthDate;
      }

      const registrationData: BaseRegistrationRequestDto = {
        ...formValue,
        birthDate: birthDateStr
      };

      this.loginService.registerDoctor(registrationData).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.alertsService.alertPositive('Доктор создан.');
        },
        error: (error) => {
          this.isLoading = false;
        }
      });
    } else {
      console.error('Form is invalid');
      this.form.markAllAsTouched();
    }
  }

  private formatDate(date: Date): string {
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
  }
}
