import { HttpInterceptorFn } from '@angular/common/http';
import {catchError} from "rxjs/operators";
import {throwError} from "rxjs";
import {StringUtils} from "../utils/string/string-utils";
import {EnvironmentInjector, inject, runInInjectionContext} from "@angular/core";
import {AlertsService} from "../shared-services/alerts/alerts.service";
import {AuthService} from "../auth/auth.service";

export const errorInterceptor: HttpInterceptorFn = (req, next, injector = inject(EnvironmentInjector)) => {
  return next(req).pipe(
    catchError((httpError) => {
      if (httpError) {
        return runInInjectionContext(injector, () => processError(httpError));
      }
      return throwError(() => httpError);
    }),
  );
};

const processError = (httpError: any) => {
  const notificationsService = inject(AlertsService);
  const errorJson = httpError.error || {};

  const isUserFriendlyMessage = !StringUtils.isNullOrWhiteSpace(errorJson.userFriendlyMessage);
  let userFriendlyMessage = '';
  switch (httpError.status) {
    case 401:
      handleUnauthorizedError();
      break;
    case 404:
      userFriendlyMessage = isUserFriendlyMessage
        ? errorJson.userFriendlyMessage
        : 'Данные отсутствуют.';
      notificationsService.alertNegative(userFriendlyMessage);
      break;
    case 500:
      userFriendlyMessage = isUserFriendlyMessage
        ? errorJson.userFriendlyMessage
        : 'Внутренняя ошибка сервера.';
      notificationsService.alertNegative(userFriendlyMessage);
      break;
    default:
      userFriendlyMessage = isUserFriendlyMessage
        ? errorJson.userFriendlyMessage
        : 'Произошла ошибка сервера. Пожалуйста, попробуйте позже.';
      notificationsService.alertNegative(userFriendlyMessage);
      break;
  }
  return throwError(() => httpError);
};

const handleUnauthorizedError = () => {
  const notificationsService = inject(AlertsService);
  const authService = inject(AuthService);

  notificationsService.alertNegative("Токен устарел, требуется повторная авторизация.");
  authService.logout();
}
