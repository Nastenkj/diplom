import { Routes } from '@angular/router';
import {LoginComponent} from "./components/login/login.component";
import {RegisterComponent} from "./components/register/register.component";
import {DemoComponent} from "./components/demo/demo.component";
import {UnauthorizedComponent} from "./components/unauthorized/unauthorized.component";
import {noAuthGuard} from "./auth/no-auth.guard";
import {authGuard} from "./auth/auth.guard";
import {PatientListComponent} from "./components/patient-list/patient-list.component";
import {PatientProfileComponent} from "./components/profile/patient-profile.component";
import {DoctorProfileComponent} from "./components/doctor-profile/doctor-profile.component";
import {DoctorsListComponent} from "./components/doctors-list/doctors-list.component";
import {ObservationParametersComponent} from "./components/observation-parameters/observation-parameters.component";
import {RegisterDoctorComponent} from "./components/register-doctor/register-doctor.component";

// Todo: lazy loading
export const routes: Routes = [
  { path:'', component: DemoComponent, canActivate: [authGuard] },
  { path:'login', component: LoginComponent, canActivate: [noAuthGuard] },
  { path:'register', component: RegisterComponent, canActivate: [noAuthGuard] },
  {
    path: 'patients',
    component: PatientListComponent,
    canActivate: [authGuard],
    data: { featureKey: 'doctor.patients' }
  },
  {
    path: 'patients/:id', // userId
    component: PatientProfileComponent,
    canActivate: [authGuard],
    data: { featureKey: 'doctor.patient_details' }
  },
  {
    path: 'patient-profile',
    component: PatientProfileComponent,
    canActivate: [authGuard],
    data: { featureKey: 'patient.edit' }
  },
  {
    path: 'doctors',
    component: DoctorsListComponent,
    canActivate: [authGuard],
    data: { featureKey: 'admin.doctors' }
  },
  {
    path: 'doctor-profile',
    component: DoctorProfileComponent,
    canActivate: [authGuard],
    data: { featureKey: 'doctor.edit' }
  },
  {
    path: 'doctors/:id', // userId
    component: DoctorProfileComponent,
    canActivate: [authGuard],
    data: { featureKey: 'admin.doctor_details' }
  },
  {
    path: 'observation-parameters',
    component: ObservationParametersComponent,
    canActivate: [authGuard],
    data: { featureKey: 'doctor.edit' }
  },
  {
    path: 'observation-parameters/:id', // userId
    component: ObservationParametersComponent,
    canActivate: [authGuard],
    data: { featureKey: 'doctor.edit' }
  },
  {
    path: 'register-doctor',
    component: RegisterDoctorComponent,
    canActivate: [authGuard],
    data: { featureKey: 'admin.doctor_details' }
  },
  { path:'unauthorized', component: UnauthorizedComponent, canActivate: [authGuard] },
];
