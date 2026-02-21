import {CanActivateFn, Router} from '@angular/router';
import {AuthService} from "./auth.service";
import {inject} from "@angular/core";

export const noAuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if (authService.currentUserSubject.value && !authService.isTokenExpired()) {
    return router.createUrlTree(['']);
  }
  return true;
};
