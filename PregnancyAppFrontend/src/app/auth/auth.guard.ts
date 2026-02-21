import {CanActivateFn, Router} from "@angular/router";
import {inject} from "@angular/core";
import {AuthService} from "./auth.service";
import {AuthorizationService} from "../shared-services/auth/authorization.service";

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const authorizationService = inject(AuthorizationService);
  const router = inject(Router);

  if (!authService.currentUserSubject.value || authService.isTokenExpired()) {
    authService.logout();
    return router.createUrlTree(['/login']);
  }

  const featureKey = route.data['featureKey'] as string;

  if (featureKey && !authorizationService.hasPermissionForFeature(featureKey)) {
    return router.createUrlTree(['/unauthorized']);
  }

  return true;
};
