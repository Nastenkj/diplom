import {Component, OnDestroy, OnInit} from '@angular/core';
import {NavigationEnd, Router, RouterModule} from '@angular/router';
import {CommonModule} from "@angular/common";
import {TuiButton, TuiIcon, TuiLink, TuiPopup, TuiRoot, TuiTitle} from "@taiga-ui/core";
import {TuiTabsHorizontal, TuiTabs, TuiDrawer, TuiBadge} from "@taiga-ui/kit";
import {TuiHeader, TuiNavigation} from "@taiga-ui/layout";
import {HasPermissionDirective} from "./directives/has-permission.directive";
import {AuthService} from "./auth/auth.service";
import {filter, Subject, takeUntil} from "rxjs";
import {AppDoesntHavePermissionDirective} from "./directives/app-doesnt-have-permission.directive";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [TuiRoot, CommonModule,
    RouterModule, TuiRoot, TuiTabsHorizontal, TuiTabs, TuiDrawer, TuiPopup, TuiButton, TuiNavigation, TuiLink, TuiBadge, TuiTitle, TuiHeader, HasPermissionDirective, TuiIcon, AppDoesntHavePermissionDirective],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  activeTabUrl = '/';
  showPatientTabs = false;
  showDoctorTabs = false;
  isAdmin = false;
  isAuthenticated = false;
  private destroy$ = new Subject<void>();

  constructor(public authService: AuthService, private router: Router) {
  }

  ngOnInit(): void {
    this.router.events
      .pipe(
        takeUntil(this.destroy$),
        filter(event => event instanceof NavigationEnd)
      )
      .subscribe((event: NavigationEnd) => {
        if (this.isAdmin && event.url === '/') {
          this.router.navigate(['/patients']);
        } else {
          this.activeTabUrl = this.router.url;
        }
      });

    this.authService.currentUserSubject
      .pipe(takeUntil(this.destroy$))
      .subscribe(val => {
        if (val) {
          this.isAuthenticated = true;
          this.showPatientTabs = val.roles === 'Пациент';
          this.showDoctorTabs = val.roles === 'Врач';
          this.isAdmin = val.roles === 'Администратор';
        } else {
          this.isAuthenticated = false;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  logout() {
    this.authService.logout();
  }

  isActiveTab(url: string): boolean {
    return this.activeTabUrl.startsWith(url);
  }
}
