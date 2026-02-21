import { Injectable } from '@angular/core';
import {BehaviorSubject} from "rxjs";
import {UserClaims} from './entities/UserClaims';
import {jwtDecode} from 'jwt-decode';
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  public currentUserSubject = new BehaviorSubject<any>(null);

  constructor(private router: Router) {
    this.initializeAuthState();
  }

  private initializeAuthState(): void {
    const token = localStorage.getItem('access_token');
    if (token && !this.isTokenExpired()) {
      this.setUser(token);
    }
  }

  login(token: string): void {
    localStorage.setItem('access_token', token);
    // console.log(token);

    this.setUser(token);
    this.router.navigateByUrl('/').then(() => {
      window.location.reload();
    });
  }

  logout(): void {
    localStorage.removeItem('access_token');
    this.currentUserSubject.next(null);
    // this.router.navigate(['/login']);
    this.router.navigateByUrl('/login').then(() => {
      window.location.reload();
    });
  }

  private setUser(token: string): void {
    const decoded = jwtDecode<UserClaims>(token);
    this.currentUserSubject.next(decoded);
  }

  hasPermission(requiredClaim: string): boolean {
    const user = this.currentUserSubject.value;
    return Boolean(user && user[requiredClaim as keyof typeof user]);
  }

  getUserId() {
    return this.currentUserSubject.value.sub;
  }

  isTokenExpired(): boolean {
    const token = localStorage.getItem('access_token');
    if (!token) return true;
    const decoded = jwtDecode<UserClaims>(token);
    return Date.now() >= decoded.exp * 1000;
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem('access_token');
    return !!(token && !this.isTokenExpired());
  }
}
