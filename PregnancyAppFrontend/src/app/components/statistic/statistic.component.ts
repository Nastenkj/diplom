import { Component, Input, OnInit, OnDestroy, inject } from '@angular/core';
import { AsyncPipe, NgForOf, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {LineChartComponent, NgxChartsModule} from '@swimlane/ngx-charts';
import { StatisticsService } from '../../shared-services/statistics/statistics.service';
import { Subscription } from 'rxjs';
import {
  TuiDayRange,
  TuiDay,
  TuiMonth,
  tuiPure,
  TuiStringHandler,
  TUI_IS_E2E,
} from '@taiga-ui/cdk';
import { TUI_MONTHS } from '@taiga-ui/core';
import { TuiInputDateRangeModule, TuiSelectModule } from "@taiga-ui/legacy";
import { map, Observable, of } from 'rxjs';
import {StatisticFields} from "../../dtos/internal/statistic-fields-enum";
import {displayNames} from "../../dtos/internal/parameter-display-names";
import {enumConfigs} from "../../dtos/internal/enum-configs";
import {AverageStatisticsComponent} from "../average-statistics/average-statistics.component";

@Component({
  selector: 'app-statistic',
  standalone: true,
  imports: [
    AsyncPipe,
    FormsModule,
    NgForOf,
    NgIf,
    NgxChartsModule,
    TuiSelectModule,
    TuiInputDateRangeModule,
    AverageStatisticsComponent,
  ],
  templateUrl: './statistic.component.html',
  styleUrl: './statistic.component.scss'
})
export class StatisticComponent implements OnInit, OnDestroy {
  @Input() patientId!: string;

  private readonly months$ = inject(TUI_MONTHS);
  private readonly isE2E = inject(TUI_IS_E2E);
  formatXAxisDate = (date: any): string => {
    if (!date || !(date instanceof Date)) return '';

    // Format date in Russian style: DD.MM.YYYY
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();

    return `${day}.${month}.${year}`;
  }

  private subscription: Subscription | null = null;

  yAxisTickFormatting: any;
  yScaleMin: number | undefined = undefined;
  yScaleMax: number | undefined = undefined;

  // Update to use StatisticFields instead of StatisticFields
  protected selectedField = StatisticFields.HeartRate;
  protected fieldOptions = Object.keys(StatisticFields)
    .map(key => StatisticFields[key as keyof typeof StatisticFields]);

  protected dateRange = new TuiDayRange(
    TuiDay.currentLocal().append({month: -12}),
    TuiDay.currentLocal(),
  );
  protected show = this.dateRange;
  protected readonly maxLength = {month: 6};

  protected minDate: TuiDay | null = null;
  protected maxDate: TuiDay | null = null;

  multi: any[] = [];

  legend: boolean = false;
  showLabels: boolean = true;
  animations: boolean = true;
  xAxis: boolean = true;
  yAxis: boolean = true;
  showYAxisLabel: boolean = true;
  showXAxisLabel: boolean = true;
  xAxisLabel: string = 'Дата';
  yAxisLabel: string = '';
  timeline: boolean = true;
  isBinaryData: boolean = false;
  isEnumData: boolean = false;
  tableEmpty: boolean = false;
  enumType: string | null = null;
  yAxisTicks!: number[];

  colorScheme: string = 'vivid';

  protected readonly xStringify$: Observable<TuiStringHandler<TuiDay>> =
    this.months$.pipe(
      map(
        (months) =>
          ({month, day}) =>
            `${months[month]}, ${day}`,
      ),
    );

  protected readonly yStringify: TuiStringHandler<number> = function(y){
    return `${y.toLocaleString('ru-RU', {maximumFractionDigits: 1})}`;
  }

  constructor(private readonly statisticsService: StatisticsService) {
    this.yAxisTickFormatting = this.formatYAxisTick.bind(this);
  }

  ngOnInit() {
    // console.log(LineChartComponent);
    this.loadStatistics();
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  protected loadStatistics(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }

    const startDate = this.dateRange.from.toUtcNativeDate();
    const endDate = this.dateRange.to.toUtcNativeDate();

    this.subscription = this.statisticsService.getStatisticByDate(
      this.selectedField,
      this.patientId,
      startDate,
      endDate
    ).subscribe(result => {
      const seriesData = result.plotPoints.map(point => {
        const date = new Date(point.fixationDateUtc);
        return {
          name: date,
          value: point.value
        };
      });

      seriesData.sort((a, b) => a.name.getTime() - b.name.getTime());

      // Reset flags
      this.isBinaryData = false;
      this.isEnumData = false;
      this.enumType = null;

      // Check field and data type
      this.checkDataType(seriesData);

      this.multi = [
        {
          name: this.getFieldDisplayName(this.selectedField),
          series: seriesData
        }
      ];

      this.yAxisLabel = this.getFieldDisplayName(this.selectedField);

      if (seriesData.length > 0) {
        const firstDate = seriesData[0].name;
        const lastDate = seriesData[seriesData.length - 1].name;

        this.dateRange = new TuiDayRange(new TuiDay(
          firstDate.getFullYear(),
          firstDate.getMonth(),
          firstDate.getDate()
        ), new TuiDay(
          lastDate.getFullYear(),
          lastDate.getMonth(),
          lastDate.getDate()
        ));
        this.tableEmpty = false;
      } else {
        this.tableEmpty = true;
      }
    });
  }

  protected onFieldChange(): void {
    this.dropDateRange();
    this.loadStatistics();

    this.statisticsService.getObservationParametersStatistics(this.patientId).subscribe(result => {
      console.log(result);
    })
  }

  protected onDateRangeChange(range: TuiDayRange): void {
    this.dateRange = range;
    this.show = range;
    this.loadStatistics();
  }

  onSelect(event: any): void {
    // console.log('Item clicked', JSON.parse(JSON.stringify(event)));
  }

  onActivate(event: any): void {
    // console.log('Activate', JSON.parse(JSON.stringify(event)));
  }

  onDeactivate(event: any): void {
    // console.log('Deactivate', JSON.parse(JSON.stringify(event)));
  }

  @tuiPure
  protected getWidth({from, to}: TuiDayRange): number {
    return TuiDay.lengthBetween(from, to);
  }

  @tuiPure
  protected labels({from, to}: TuiDayRange): Observable<readonly string[]> {
    const length = TuiDay.lengthBetween(from, to);

    if (length > 90) {
      return this.months$.pipe(
        map((months) => [
          ...Array.from(
            {length: TuiMonth.lengthBetween(from, to) + 1},
            (_, i) => months[from.append({month: i}).month] ?? '',
          ),
          '',
        ]),
      );
    }

    const range = Array.from({length}, (_, day) => from.append({day}));
    const mondays = this.onlyMondays(range);
    const days = range.map(String);

    if (length > 60) {
      return of([...this.even(mondays), '']);
    }

    if (length > 14) {
      return of([...mondays, '']);
    }

    if (length > 7) {
      return of([...this.even(days), '']);
    }

    return of([...days, '']);
  }

  protected readonly filter = (
    [day]: [TuiDay, number],
    {from, to}: TuiDayRange,
  ) => day.daySameOrAfter(from) && day.daySameOrBefore(to);

  protected readonly toNumbers = (
    days: Array<[TuiDay, number]>,
    {from}: TuiDayRange
  ): Array<[number, number]> => days.map(([day, value]) => [TuiDay.lengthBetween(from, day), value]);

  protected getFieldDisplayName(field: StatisticFields | string): string {
    const fieldEnum = typeof field === 'string' ? field as StatisticFields : field;

    return displayNames[fieldEnum] || String(fieldEnum);
  }

  private checkDataType(seriesData: any[]): void {
    // First check if it's an enum type based on the field name
    this.checkIfEnumField(seriesData);

    // Only check for binary data if it's not already identified as an enum
    if (!this.isEnumData && seriesData.length > 0) {
      this.isBinaryData = seriesData.every(item => item.value === 0 || item.value === 1);

      if (this.isBinaryData) {
        this.yAxisTicks = [0, 1];
      }
    }
  }

  private checkIfEnumField(seriesData: any[]): void {
    const config = enumConfigs.get(this.selectedField);

    if (config) {
      this.isEnumData = true;
      this.enumType = config.enumType;
      this.yAxisTicks = [...config.yAxisTicks];

      if (config.yScaleMin !== undefined) {
        this.yScaleMin = config.yScaleMin;
      }

      if (config.yScaleMax !== undefined) {
        this.yScaleMax = config.yScaleMax;
      }
    } else {
      this.isEnumData = false;
      this.enumType = null;
    }

    const set = new Set(seriesData.map(item => item.value));
    this.fixYAxisTicksIfSetSizeIsOne(set);
  }

  fixYAxisTicksIfSetSizeIsOne(set: Set<number>){
    if (set.size === 1) {
      this.yAxisTicks = [...set];
    }
  }

  protected formatYAxisTick(value: number): string {
    if (this.isBinaryData) {
      return value === 1 ? 'Да' : 'Нет';
    }

    if (this.isEnumData) {
      const config = enumConfigs.get(this.selectedField);
      if (config) {
        return config.stringifyFn(value);
      }
    }

    return value.toString();
  }

  private onlyMondays(range: readonly TuiDay[]): readonly string[] {
    return range.filter((day) => !day.dayOfWeek()).map(String);
  }

  private even<T>(array: readonly T[]): readonly T[] {
    return array.filter((_, i) => !(i % 2));
  }

  private dropDateRange() {
    this.dateRange = new TuiDayRange(
      TuiDay.currentLocal().append({month: -12}),
      TuiDay.currentLocal(),
    );
  }

  protected readonly String = String;
}
