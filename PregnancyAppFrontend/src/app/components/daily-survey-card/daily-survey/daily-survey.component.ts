import {Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output, TemplateRef, ViewChild} from '@angular/core';
import {NgForOf, NgIf} from "@angular/common";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {
  TuiAppBarBack,
  TuiAppBarComponent,
  TuiAppBarDirective,
  TuiAppBarSizeDirective,
  TuiHeader
} from "@taiga-ui/layout";
import {TuiButton, TuiDataList, TuiDialogService, TuiTitle} from "@taiga-ui/core";
import {TuiInputDateRangeModule, TuiInputModule, TuiSelectModule, TuiTextareaModule} from "@taiga-ui/legacy";
import {TuiProgressBar, TuiRadioList} from "@taiga-ui/kit";
import {bloodPressureValidator} from "../../../validators/blood-pressure-validator";
import {MaskitoDirective} from "@maskito/angular";
import {decimalMask, numberMask} from "../../../masks/number-mask";
import {bodyTemperatureMask} from "../../../masks/body-temperature-mask";
import {bloodPressureMask} from "../../../masks/blood-pressure-mask";
import {Observable, Subject, takeUntil} from "rxjs";
import {DailySurveyDto} from "../../../dtos/daily-survey/daily-survey-dto";
import {DailySurveyService} from "../../../shared-services/surveys/daily-surveys.service";
import {mapTemperature, temperatureStringify} from "../../../enums/daily-survey/temperature";
import {AlertsService} from "../../../shared-services/alerts/alerts.service";


@Component({
  selector: 'app-daily-survey',
  standalone: true,
  imports: [
    NgIf,
    ReactiveFormsModule,
    TuiAppBarBack,
    TuiAppBarComponent,
    TuiAppBarDirective,
    TuiAppBarSizeDirective,
    TuiButton,
    TuiHeader,
    TuiInputDateRangeModule,
    TuiInputModule,
    TuiProgressBar,
    TuiRadioList,
    TuiTitle,
    TuiDataList,
    TuiSelectModule,
    MaskitoDirective,
    NgForOf,
    TuiTextareaModule
  ],
  templateUrl: './daily-survey.component.html',
  styleUrl: './daily-survey.component.scss'
})
export class DailySurveyComponent implements OnInit, OnDestroy {
  @Output() surveySubmitted = new EventEmitter<void>();
  @Input() openEvent!: Observable<void>;
  @Input() isFormReadonly: boolean = false;
  @Input() prepopulatedDailySurvey: DailySurveyDto | null = null;
  @ViewChild('template', { static: true })
  public templateRef!: TemplateRef<any>;
  form!: FormGroup;
  private readonly dialogs = inject(TuiDialogService);
  private readonly dailySurveysService = inject(DailySurveyService);
  private readonly alertsService = inject(AlertsService);
  private readonly destroy$ = new Subject<void>();
  protected step = 0;

  protected urgeToPukeRange = Array.from({ length: 100 }, (_, i) => i);
  protected abdomenHurtsRange = [false, true].reverse();
  protected bloodDischargeRange = [false, true].reverse();
  protected nauseaRange = [false, true].reverse();

  abdomenHurtsStringify = (value: number) => (!value ? 'Нет' : `Да`)
  bloodDischargeStringify = (value: number) => (!value ? 'Нет' : `Да`)
  nauseaStringify = (value: number) => (!value ? 'Нет' : `Да`)
  urgeToPukeStringify = (value: number) => (value === 0 ? 'Не было' : `${value}`)

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

    if (this.form.valid) {
      const tempValue = parseFloat(this.form.value.temperature);
      const dailySurvey: DailySurveyDto = {
        ...this.form.value,
        temperature: mapTemperature(tempValue),
        systolicPressure: parseInt(this.form.value.bloodPressure.split('/')[0]),
        diastolicPressure: parseInt(this.form.value.bloodPressure.split('/')[1])
      };

      this.dailySurveysService.postDailySurvey(dailySurvey)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response: DailySurveyDto) => {
            // console.log(response);
            this.surveySubmitted.next();
            this.alertsService.alertPositive('Результат ежедневного опроса сохранён.');
            observer.complete()
          },
          error: (error: any) => {
            this.alertsService.alertPositive('Ошибка сохранения результата ежедневного опроса.');
            console.error('Error submitting daily survey:', error);
          }
        });
    } else {
      console.error('Form invalid.');
    }
  }

  ngOnInit() {
    this.openEvent
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.open(this.templateRef);
        this.populateFormWithExistingData();
        // console.log(this.prepopulatedDailySurvey);
      });

    this.form = new FormGroup({
      abdomenHurts: new FormControl(null, [Validators.required]),
      bloodDischarge: new FormControl(null, [Validators.required]),
      nausea: new FormControl(null, [Validators.required]),
      urgeToPuke: new FormControl(null, [Validators.required]),
      temperature: new FormControl(null, [Validators.required]),
      bloodPressure: new FormControl(null, [Validators.required, bloodPressureValidator()]),
      heartRate: new FormControl(null, [Validators.required]),
      glucoseLevel: new FormControl(null),
      hemoglobinLevel: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      saturation: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),

      uro: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      bld: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      bil: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      ket: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      leu: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      glu: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      pro: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      ph: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      nit: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      sg: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      vc: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      pt: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      aptt: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),
      inr: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),

      oxygenLevel: new FormControl(null, [Validators.pattern('^[0-9]*(\\.[0-9]+)?$')]),

      additionalInformation: new FormControl(null),
    });

    if (this.isFormReadonly){
      this.form.disable();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get isStep0Valid() {
    return this.isFormReadonly || (
      this.form.get('abdomenHurts')?.valid &&
      this.form.get('bloodDischarge')?.valid &&
      this.form.get('nausea')?.valid &&
      this.form.get('urgeToPuke')?.valid &&
      this.form.get('temperature')?.valid &&
      this.form.get('bloodPressure')?.valid &&
      this.form.get('heartRate')?.valid &&
      this.form.get('glucoseLevel')?.valid &&
      this.form.get('hemoglobinLevel')?.valid &&
      this.form.get('saturation')?.valid
    );
  }

  get isStep1Valid() {
    return true;
  }

  onClose($event: Event, observer: { complete: () => void }) {
    $event.preventDefault();
    $event.stopPropagation();
    observer.complete();
  }

  populateFormWithExistingData() {
    if (this.prepopulatedDailySurvey) {
      this.form.patchValue({
        abdomenHurts: this.prepopulatedDailySurvey.abdomenHurts,
        bloodDischarge: this.prepopulatedDailySurvey.bloodDischarge,
        nausea: this.prepopulatedDailySurvey.nausea,
        urgeToPuke: this.prepopulatedDailySurvey.urgeToPuke,
        temperature: this.isFormReadonly
          ? temperatureStringify(this.prepopulatedDailySurvey.temperature)
          : this.prepopulatedDailySurvey.temperature,
        bloodPressure: `${this.prepopulatedDailySurvey.systolicPressure}/${this.prepopulatedDailySurvey.diastolicPressure}`,
        heartRate: this.prepopulatedDailySurvey.heartRate,
        glucoseLevel: this.prepopulatedDailySurvey.glucoseLevel,
        hemoglobinLevel: this.prepopulatedDailySurvey.hemoglobinLevel,
        saturation: this.prepopulatedDailySurvey.saturation
      });

      if (this.prepopulatedDailySurvey.uro !== undefined) {
        this.form.get('uro')?.setValue(this.prepopulatedDailySurvey.uro);
      }

      if (this.prepopulatedDailySurvey.bld !== undefined) {
        this.form.get('bld')?.setValue(this.prepopulatedDailySurvey.bld);
      }

      if (this.prepopulatedDailySurvey.bil !== undefined) {
        this.form.get('bil')?.setValue(this.prepopulatedDailySurvey.bil);
      }

      if (this.prepopulatedDailySurvey.ket !== undefined) {
        this.form.get('ket')?.setValue(this.prepopulatedDailySurvey.ket);
      }

      if (this.prepopulatedDailySurvey.leu !== undefined) {
        this.form.get('leu')?.setValue(this.prepopulatedDailySurvey.leu);
      }

      if (this.prepopulatedDailySurvey.glu !== undefined) {
        this.form.get('glu')?.setValue(this.prepopulatedDailySurvey.glu);
      }

      if (this.prepopulatedDailySurvey.pro !== undefined) {
        this.form.get('pro')?.setValue(this.prepopulatedDailySurvey.pro);
      }

      if (this.prepopulatedDailySurvey.ph !== undefined) {
        this.form.get('ph')?.setValue(this.prepopulatedDailySurvey.ph);
      }

      if (this.prepopulatedDailySurvey.nit !== undefined) {
        this.form.get('nit')?.setValue(this.prepopulatedDailySurvey.nit);
      }

      if (this.prepopulatedDailySurvey.sg !== undefined) {
        this.form.get('sg')?.setValue(this.prepopulatedDailySurvey.sg);
      }

      if (this.prepopulatedDailySurvey.vc !== undefined) {
        this.form.get('vc')?.setValue(this.prepopulatedDailySurvey.vc);
      }

      if (this.prepopulatedDailySurvey.pt !== undefined) {
        this.form.get('pt')?.setValue(this.prepopulatedDailySurvey.pt);
      }

      if (this.prepopulatedDailySurvey.aptt !== undefined) {
        this.form.get('aptt')?.setValue(this.prepopulatedDailySurvey.aptt);
      }

      if (this.prepopulatedDailySurvey.inr !== undefined) {
        this.form.get('inr')?.setValue(this.prepopulatedDailySurvey.inr);
      }

      if (this.prepopulatedDailySurvey.oxygenLevel !== undefined) {
        this.form.get('oxygenLevel')?.setValue(this.prepopulatedDailySurvey.oxygenLevel);
      }

      if (this.prepopulatedDailySurvey.additionalInformation) {
        this.form.get('additionalInformation')?.setValue(this.prepopulatedDailySurvey.additionalInformation);
      }
    }
  }

  protected readonly numberMask = numberMask;
  protected readonly decimalMask = decimalMask;
  protected readonly bodyTemperatureMask = bodyTemperatureMask;
  protected readonly bloodPressureMask = bloodPressureMask;
  protected readonly temperatureStringify = temperatureStringify;
}
