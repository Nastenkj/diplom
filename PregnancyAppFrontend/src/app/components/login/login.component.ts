import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import {Router, RouterModule} from '@angular/router';
import {TuiAppearance, TuiButton, TuiTitle} from '@taiga-ui/core';
import {TuiCardLarge} from '@taiga-ui/layout';
import {TuiInputModule, TuiInputPasswordModule} from '@taiga-ui/legacy';
import {HttpErrorResponse} from "@angular/common/http";
import {AuthService} from "../../auth/auth.service";
import {LoginService} from "../../shared-services/auth/login.service";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    TuiInputModule,
    RouterModule,
    TuiTitle,
    TuiCardLarge,
    TuiInputPasswordModule,
    TuiButton,
    TuiAppearance
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  form!: FormGroup;

  constructor(private auth: AuthService, private router: Router, private loginService: LoginService) {
  }

  ngOnInit(): void {
    this.form = new FormGroup({
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required, Validators.minLength(1), Validators.maxLength(32)]),
    });
  }

  onSubmit() {
    if (this.form.invalid) return;
    const { email, password } = this.form.value;
    this.loginService.login({ email, password }).subscribe({
      next: r => {
        this.auth.login(r.jwtToken);
        this.router.navigate(['/']);
      },
      error: () => {}
    });
  }



}
