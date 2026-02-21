import { Injectable } from '@angular/core';
import {AuthService} from "../../auth/auth.service";
import {ClaimsConfigService} from "./claims-config.service";

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {

  constructor(
    private authService: AuthService,
    private claimsConfig: ClaimsConfigService
  ) {}

  hasPermissionForFeature(featureKey: string): boolean {
    const requiredClaims = this.claimsConfig.getRequiredClaims(featureKey);
    return this.checkPermissions(requiredClaims);
  }

  checkPermissions(requiredClaims: string[]): boolean {
    if (!requiredClaims || requiredClaims.length === 0) {
      return true;
    }
    return requiredClaims.every(claim => this.authService.hasPermission(claim));
  }
}
