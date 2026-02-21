import { NG_EVENT_PLUGINS } from "@taiga-ui/event-plugins";
import { provideAnimations } from "@angular/platform-browser/animations";
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import { appConfig } from './app/app.config';
import {errorInterceptor} from "./app/interceptors/error.interceptor";
import {authInterceptor} from "./app/interceptors/auth.interceptor";


bootstrapApplication(AppComponent, {
  providers: [
    provideAnimations(),
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
    ...appConfig.providers,
    NG_EVENT_PLUGINS
  ]
}).catch((err) => console.error(err));
