import {ApplicationConfig, importProvidersFrom} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {provideAnimations} from "@angular/platform-browser/animations";
import {TUI_LANGUAGE, TUI_RUSSIAN_LANGUAGE} from "@taiga-ui/i18n";
import {of} from "rxjs";
import {NG_EVENT_PLUGINS} from "@taiga-ui/event-plugins";



export const appConfig: ApplicationConfig = {
  providers: [provideAnimations(), NG_EVENT_PLUGINS, provideRouter(routes),  {
    provide: TUI_LANGUAGE,
    useValue: of(TUI_RUSSIAN_LANGUAGE),
  },]
};
