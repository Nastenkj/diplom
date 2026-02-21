import {Directive, Input, OnDestroy, OnInit, TemplateRef, ViewContainerRef} from '@angular/core';
import {AuthorizationService} from "../shared-services/auth/authorization.service";
import {Subscription} from "rxjs";
import {AuthService} from "../auth/auth.service";

@Directive({
  selector: '[appHasPermission]',
  standalone: true
})
export class HasPermissionDirective implements OnInit, OnDestroy {

  @Input('appHasPermission') featureKey!: string;

  private subscription?: Subscription;

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authorizationService: AuthorizationService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.updateView();
    this.subscription = this.authService.currentUserSubject.subscribe(() => {
      this.updateView();
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  private updateView(): void {
    this.viewContainer.clear();
    if (this.authorizationService.hasPermissionForFeature(this.featureKey)) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }
}
