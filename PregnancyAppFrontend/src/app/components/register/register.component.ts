import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import {
  TuiInputDateModule,
  TuiInputModule,
  TuiInputPasswordModule, TuiInputPhoneModule, TuiInputYearModule,
} from "@taiga-ui/legacy";
import {TuiAppearance, TuiButton, TuiLoader, TuiTitle} from "@taiga-ui/core";
import { TuiCardLarge } from "@taiga-ui/layout";
import { CommonModule } from '@angular/common';
import {Router, RouterLink} from "@angular/router";
import { AlertsService } from "../../shared-services/alerts/alerts.service";
import {LoginService} from "../../shared-services/auth/login.service";
import {AuthService} from "../../auth/auth.service";
import {RegistrationRequestDto} from "../../dtos/auth/registration-request";

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    TuiInputDateModule,
    ReactiveFormsModule,
    TuiInputModule,
    TuiTitle,
    TuiAppearance,
    TuiCardLarge,
    TuiInputPasswordModule,
    TuiButton,
    TuiInputYearModule,
    TuiInputPhoneModule,
    RouterLink,
    TuiLoader,
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;

  constructor(
    private loginService: LoginService,
    private router: Router,
    private alertsService: AlertsService,
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.form = new FormGroup({
      fullName: new FormControl(null, [Validators.required]),
      birthDate: new FormControl(null, [Validators.required]),
      phoneNumber: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      trustedPersonFullName: new FormControl(null, [Validators.required]),
      trustedPersonPhoneNumber: new FormControl(null, [Validators.required]),
      trustedPersonEmail: new FormControl(null, [Validators.required, Validators.email]),
      insuranceNumber: new FormControl(null, [Validators.required]),
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
        // make sure it's in YYYY-MM-DD format
        const date = new Date(formValue.birthDate);
        birthDateStr = this.formatDate(date);
      } else {
        birthDateStr = formValue.birthDate;
      }

      const registrationData: RegistrationRequestDto = {
        ...formValue,
        birthDate: birthDateStr
      };

      this.loginService.register(registrationData).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.alertsService.alertPositive('Регистрация совершена. Вы можете войти в свой аккаунт.');
          this.router.navigateByUrl('/login').then(() => {
          });
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
