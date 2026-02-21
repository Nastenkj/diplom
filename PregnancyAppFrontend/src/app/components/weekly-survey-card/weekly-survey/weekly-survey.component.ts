import {Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output, TemplateRef, ViewChild} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {TuiButton, TuiDataListComponent, TuiDialogService, TuiOption, TuiTitle} from "@taiga-ui/core";
import {NgForOf, NgIf} from "@angular/common";
import {
  TuiInputModule,
  TuiMultiSelectModule,
  TuiSelectModule,
  TuiTextareaModule
} from "@taiga-ui/legacy";
import {TuiProgressBar, TuiRadioList} from "@taiga-ui/kit";
import {
  TuiAppBarBack,
  TuiAppBarComponent,
  TuiAppBarDirective,
  TuiAppBarSizeDirective,
  TuiHeader
} from "@taiga-ui/layout";
import {MaskitoDirective} from "@maskito/angular";
import {numberMask} from "../../../masks/number-mask";
import {weightAddedMask} from "../../../masks/weight-added-mask";
import {getEnumNumericValues} from "../../../utils/enums/get-enum-numeric-values";
import {BloodPressure, bloodPressureStringify} from "../../../enums/weekly-survey/blood-pressure";
import {WaterConsumed, waterConsumedStringify} from "../../../enums/weekly-survey/water-consumed";
import {Stool, stoolStringify} from "../../../enums/weekly-survey/stool";
import {Urination, urinationStringify} from "../../../enums/weekly-survey/urination";
import {Observable, Subject, takeUntil} from "rxjs";
import {WeeklySurveyDto} from "../../../dtos/weekly-survey/weekly-survey-dto";
import {WeeklySurveysService} from "../../../shared-services/surveys/weekly-surveys.service";
import {pregnancyWeekMask} from "../../../masks/pregnancy-week-mask";
import {AlertsService} from "../../../shared-services/alerts/alerts.service";

@Component({
  selector: 'app-weekly-survey',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgIf,
    NgForOf,
    TuiInputModule,
    TuiRadioList,
    TuiTitle,
    TuiAppBarBack,
    TuiAppBarComponent,
    TuiAppBarDirective,
    TuiAppBarSizeDirective,
    TuiButton,
    TuiDataListComponent,
    TuiHeader,
    TuiMultiSelectModule,
    TuiOption,
    TuiProgressBar,
    TuiSelectModule,
    TuiTextareaModule,
    MaskitoDirective
  ],
  templateUrl: './weekly-survey.component.html',
  styleUrls: ['./weekly-survey.component.scss']
})
export class WeeklySurveyComponent implements OnInit, OnDestroy {
  @Output() surveySubmitted = new EventEmitter<void>();
  @Input() openEvent!: Observable<void>;
  @Input() isFormReadonly: boolean = false;
  @Input() prepopulatedWeeklySurvey: WeeklySurveyDto | null = null;
  @ViewChild('template', { static: true })
  public templateRef!: TemplateRef<any>;

  form!: FormGroup
  private readonly dialogs = inject(TuiDialogService)
  protected step = 0
  protected ORVIRange = [false, true].reverse()
  protected unordinaryTempRange = [false, true].reverse()
  protected gynecologicalSymptomsRange = [false, true].reverse()
  protected unordinaryUrineRange = [false, true].reverse()
  protected swellingRange = [false, true].reverse()
  private readonly alertsService = inject(AlertsService);


  private readonly destroy$ = new Subject<void>();
  private readonly weeklySurveyService = inject(WeeklySurveysService);

  ORVIStringify = (value: number) => (!value ? 'Нет' : `Да`)
  unordinaryTempStringify = (value: number) => (!value ? 'Нет' : `Да`)
  unordinaryUrineStringify = (value: number) => (!value ? 'Нет' : `Да`)
  swellingStringify = (value: number) => (!value ? 'Нет' : `Да`)

  protected open(template: TemplateRef<any>): void {
    this.step = 0
    this.dialogs.open(template, {
      label: '',
      size: 'fullscreen',
      closeable: false,
      dismissible: false,
    }).subscribe()
  }

  onSubmit($event: Event, observer: { complete: () => void }) {
    $event.preventDefault()
    $event.stopPropagation()

      const weeklySurveyDto: WeeklySurveyDto = {
        ...this.form.value,
        hasOrvi: this.form.value.ORVIRadio,
        orviSymptoms: this.form.value.ORVI,
        hasUnordinaryTemp: this.form.value.unordinaryTempRadio,
        unordinaryTempOccurrences: this.form.value.unordinaryTemp,
        hasGynecologicalSymptoms: this.form.value.gynecologicalSymptomsRadio,
        gynecologicalSymptoms: this.form.value.gynecologicalSymptoms,
        hasUnordinaryUrine: this.form.value.unordinaryUrine,
        hasSwelling: this.form.value.swellingRadio,
        swellingDescription: this.form.value.swelling
      };

      this.weeklySurveyService.postWeeklySurvey(weeklySurveyDto)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response: WeeklySurveyDto) => {
            // console.log(response);
            this.surveySubmitted.next();
            this.alertsService.alertPositive('Результат еженедельного опроса сохранён.');
            observer.complete()
          },
          error: (error: any) => {
            this.alertsService.alertPositive('Ошибка сохранения результата еженедельного опроса.');
            console.error('Error submitting weekly survey:', error);
          }
        });
  }

  onClose($event: Event, observer: { complete: () => void }) {
    $event.preventDefault();
    $event.stopPropagation();
    observer.complete();
  }

  ngOnInit() {
    this.openEvent
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.open(this.templateRef);
        this.populateFormWithExistingData();
        // console.log(this.prepopulatedWeeklySurvey);
      });

    this.form = new FormGroup({
      ORVIRadio: new FormControl(null, [Validators.required]),
      ORVI: new FormControl(null, [Validators.required]),
      unordinaryTempRadio: new FormControl(null, [Validators.required]),
      unordinaryTemp: new FormControl(null, [Validators.required]),
      unordinaryBloodPressure: new FormControl(null, [Validators.required]),
      gynecologicalSymptomsRadio: new FormControl(null, [Validators.required]),
      gynecologicalSymptoms: new FormControl(null, [Validators.required]),
      unordinaryUrine: new FormControl(null, [Validators.required]),
      swellingRadio: new FormControl(null, [Validators.required]),
      swelling: new FormControl(null, [Validators.required]),
      waterConsumed: new FormControl(null, [Validators.required]),
      stool: new FormControl(null, [Validators.required]),
      urination: new FormControl(null, [Validators.required]),
      weightAdded: new FormControl(null, [Validators.required]),
      pregnancyWeek: new FormControl(null, [Validators.required]),
    });

    if (this.isFormReadonly) {
      this.form.disable();
    }
  }

  populateFormWithExistingData() {
    if (this.prepopulatedWeeklySurvey) {
      this.form.patchValue({
        ORVIRadio: this.prepopulatedWeeklySurvey.hasOrvi,
        ORVI: this.prepopulatedWeeklySurvey.orviSymptoms,
        unordinaryTempRadio: this.prepopulatedWeeklySurvey.hasUnordinaryTemp,
        unordinaryTemp: this.prepopulatedWeeklySurvey.unordinaryTempOccurrences,
        unordinaryBloodPressure: this.prepopulatedWeeklySurvey.unordinaryBloodPressure,
        gynecologicalSymptomsRadio: this.prepopulatedWeeklySurvey.hasGynecologicalSymptoms,
        gynecologicalSymptoms: this.prepopulatedWeeklySurvey.gynecologicalSymptoms,
        unordinaryUrine: this.prepopulatedWeeklySurvey.hasUnordinaryUrine,
        swellingRadio: this.prepopulatedWeeklySurvey.hasSwelling,
        swelling: this.prepopulatedWeeklySurvey.swellingDescription,
        waterConsumed: this.prepopulatedWeeklySurvey.waterConsumed,
        stool: this.prepopulatedWeeklySurvey.stool,
        urination: this.prepopulatedWeeklySurvey.urination,
        weightAdded: this.prepopulatedWeeklySurvey.weightAdded,
        pregnancyWeek: this.prepopulatedWeeklySurvey.pregnancyWeek,
      });
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get allergicReactionsPositiveSelected() {
    return this.form.get('ORVIRadio')?.value
  }

  get swellingPositiveSelected() {
    return this.form.get('swellingRadio')?.value
  }

  get unordinaryTempPositiveSelected() {
    return this.form.get('unordinaryTempRadio')?.value
  }

  get gynecologicalSymptomsPositiveSelected() {
    return this.form.get('gynecologicalSymptomsRadio')?.value
  }

  get isStep0Valid() {
    const orviRadioValue = this.form.get('ORVIRadio')?.value
    const unordinaryTempRadioValue = this.form.get('unordinaryTempRadio')?.value
    const gynecologicalSymptomsRadioValue = this.form.get('gynecologicalSymptomsRadio')?.value
    const swellingRadioValue = this.form.get('swellingRadio')?.value

    const orviValid = orviRadioValue ? this.form.get('ORVI')?.valid : true
    const unordinaryTempValid = unordinaryTempRadioValue ? this.form.get('unordinaryTemp')?.valid : true
    const gynecologicalSymptomsValid = gynecologicalSymptomsRadioValue ? this.form.get('gynecologicalSymptoms')?.valid : true
    const swellingValid = swellingRadioValue ? this.form.get('swelling')?.valid : true

    return this.isFormReadonly || (
      this.form.get('ORVIRadio')?.valid &&
      orviValid &&
      this.form.get('unordinaryTempRadio')?.valid &&
      unordinaryTempValid &&
      this.form.get('unordinaryBloodPressure')?.valid &&
      this.form.get('gynecologicalSymptomsRadio')?.valid &&
      gynecologicalSymptomsValid &&
      this.form.get('unordinaryUrine')?.valid &&
      this.form.get('swellingRadio')?.valid &&
      swellingValid
    )
  }

  get isStep1Valid() {
    return this.isFormReadonly || (
      this.form.get('waterConsumed')?.valid &&
      this.form.get('stool')?.valid &&
      this.form.get('urination')?.valid &&
      this.form.get('weightAdded')?.valid &&
      this.form.get('pregnancyWeek')?.valid
    )
  }


  protected readonly numberMask = numberMask
  protected readonly weightAddedMask = weightAddedMask
  protected readonly pregnancyWeekMask = pregnancyWeekMask
  protected readonly getEnumNumericValues = getEnumNumericValues
  protected readonly BloodPressure = BloodPressure
  protected readonly bloodPressureStringify = bloodPressureStringify
  protected readonly WaterConsumed = WaterConsumed
  protected readonly waterConsumedStringify = waterConsumedStringify
  protected readonly Stool = Stool
  protected readonly stoolStringify = stoolStringify
  protected readonly Urination = Urination
  protected readonly urinationStringify = urinationStringify
}
