import {inject, Injectable, PLATFORM_ID} from '@angular/core';
import {TuiFormatDateService} from "@taiga-ui/core";
import {isPlatformBrowser} from "@angular/common";
import {map, Observable, of, timer} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class DefaultDateFormatterService extends TuiFormatDateService{
  private readonly delay$ = isPlatformBrowser(inject(PLATFORM_ID))
    ? timer(0, 1000)
    : of(0);

  public override format(timestamp: number): Observable<string> {

    return this.delay$.pipe(map(() => {
      const date = new Date(timestamp);
      const day = String(date.getDate()).padStart(2, '0');

      // getMonth() returns 0-11, so add 1 for the correct month number
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const year = date.getFullYear();

      return `${day}.${month}.${year}`;
    }));
  }
}
