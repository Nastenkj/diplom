import {Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output, TemplateRef, ViewChild} from '@angular/core'
import { TuiButton, TuiDataList, TuiDialogService, TuiTitle } from "@taiga-ui/core"
import { TuiDataListWrapper, TuiProgress, TuiRadioList } from "@taiga-ui/kit"
import { TuiAppBar, TuiHeader } from "@taiga-ui/layout"
import { NgForOf, NgIf } from "@angular/common"
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators
} from "@angular/forms"
import { CovidStatus, covidStatusStringify } from '../../../enums/medical-history/covid-status'
import { HereditaryDisease, hereditaryDiseaseStringify } from '../../../enums/medical-history/hereditary-disease'
import { BirthType, birthTypeStringify } from "../../../enums/medical-history/birth-type"
import { Thermometer, thermometerStringify } from "../../../enums/medical-history/thermometer"
import { BloodGroup, bloodGroupStringify } from "../../../enums/medical-history/blood-group"
import { RhesusFactor, rhesusFactorStringify } from "../../../enums/medical-history/rhesus-factor"
import { TuiInputModule, TuiMultiSelectModule, TuiSelectModule, TuiTextareaModule } from "@taiga-ui/legacy"
import { getEnumNumericValues } from "../../../utils/enums/get-enum-numeric-values"
import { MaskitoDirective } from '@maskito/angular'
import {numberMask} from "../../../masks/number-mask";
import {bloodPressureMask} from "../../../masks/blood-pressure-mask";
import {bloodPressureValidator} from "../../../validators/blood-pressure-validator";
import {Observable, Subject, takeUntil} from "rxjs";
import {MedicalHistoriesService} from "../../../shared-services/surveys/medical-history-surveys.service";
import {MedicalHistoryDto} from "../../../dtos/medical-history/medical-history-dto";
import {AlertsService} from "../../../shared-services/alerts/alerts.service";

@Component({
  selector: 'app-medical-history',
  standalone: true,
  imports: [
    TuiAppBar,
    TuiButton,
    TuiHeader,
    TuiProgress,
    TuiTitle,
    NgIf,
    FormsModule,
    ReactiveFormsModule,
    TuiInputModule,
    TuiSelectModule,
    NgForOf,
    TuiRadioList,
    TuiMultiSelectModule,
    TuiDataList,
    TuiDataListWrapper,
    TuiTextareaModule,
    MaskitoDirective
  ],
  templateUrl: './medical-history.component.html',
  styleUrls: ['./medical-history.component.scss']
})
export class MedicalHistoryComponent implements OnInit, OnDestroy {
  @Input() openEvent!: Observable<void>;

  @Input() isFormReadonly: boolean = false;
  @Input() prepopulatedMedicalHistory: MedicalHistoryDto | null = null;

  @Output() onObserverComplete: EventEmitter<void> = new EventEmitter<void>();

  @ViewChild('template', { static: true })
  public templateRef!: TemplateRef<any>;

  form!: FormGroup;
  private readonly dialogs = inject(TuiDialogService);
  protected step = 0;
  protected pregnancyRange = Array.from({ length: 100 }, (_, i) => i);
  protected abortionRange = Array.from({ length: 100 }, (_, i) => i);
  protected prematureBirthRange = Array.from({ length: 100 }, (_, i) => i);
  protected miscarriageRange = Array.from({ length: 100 }, (_, i) => i);
  protected allergicReactionsRange = [false, true].reverse();
  protected isSmokingRange = [false, true].reverse();
  protected isConsumingAlcoholRange = [false, true].reverse();

  private readonly destroy$ = new Subject<void>();

  pregnancySelectorStringify = (value: number) => (value === 0 ? 'Не было' : `${value}`);
  abortionSelectorStringify = (value: number) => (value === 0 ? 'Не было' : `${value}`);
  prematureBirthSelectorStringify = (value: number) => (value === 0 ? 'Не было' : `${value}`);
  miscarriageSelectorStringify = (value: number) => (value === 0 ? 'Не было' : `${value}`);
  allergicReactionsRadioStringify = (value: number) => (!value ? 'Нет' : `Да`);
  isSmokingRadioStringify = (value: number) => (!value ? 'Нет' : `Да`);
  isConsumingAlcoholStringify = (value: number) => (!value ? 'Нет' : `Да`);

  constructor(private medicalHistoriesService: MedicalHistoriesService,
              private alertsService: AlertsService) {
  }

  protected open(template: TemplateRef<any>): void {
    this.step = 0
    this.dialogs
      .open(template, {
        label: '',
        size: 'fullscreen',
        closeable: false,
        dismissible: false,
      })
      .subscribe()
  }

  ngOnInit() {
    this.openEvent
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.open(this.templateRef);
      });

    this.form = new FormGroup({
      weight: new FormControl(null, [Validators.required, Validators.pattern('^[0-9]*$')]),
      height: new FormControl(null, [Validators.required, Validators.pattern('^[0-9]*$')]),
      bloodGroup: new FormControl(null, [Validators.required]),
      rhesusFactor: new FormControl(null, [Validators.required]),
      bloodPressure: new FormControl(null, [Validators.required, bloodPressureValidator()]),
      thermometer: new FormControl(null, [Validators.required]),
      pregnancyAmount: new FormControl(null, [Validators.required]),
      abortionAmount: new FormControl(null),
      miscarriageAmount: new FormControl(null),
      prematureBirthAmount: new FormControl(null),
      previousBirthType: new FormControl(null, [Validators.required]),
      gynecologicalDiseases: new FormControl(null),
      somaticDiseases: new FormControl(null),
      undergoneOperations: new FormControl(null),
      allergicReactionsRadio: new FormControl(null),
      allergicReactions: new FormControl(null),
      hereditaryDiseases: new FormControl([], [Validators.required]),
      isSmoking: new FormControl(null, [Validators.required]),
      isConsumingAlcohol: new FormControl(null, [Validators.required]),
      enduredCovid: new FormControl(null, [Validators.required]),
    })

    if (this.isFormReadonly) {
      this.form.disable();
    }

    this.populateFormWithExistingData();

    this.form
      .get('pregnancyAmount')
      ?.valueChanges.subscribe(value => {
      if (value >= 1) {
        this.form.get('abortionAmount')?.addValidators(Validators.required)
        this.form.get('miscarriageAmount')?.addValidators(Validators.required)
        this.form.get('prematureBirthAmount')?.addValidators(Validators.required)
      } else {
        this.form.get('abortionAmount')?.clearValidators()
        this.form.get('miscarriageAmount')?.clearValidators()
        this.form.get('prematureBirthAmount')?.clearValidators()
      }
      this.form.get('abortionAmount')?.updateValueAndValidity()
      this.form.get('miscarriageAmount')?.updateValueAndValidity()
      this.form.get('prematureBirthAmount')?.updateValueAndValidity()
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onClose($event: Event, observer: { complete: () => void }) {
    $event.preventDefault();
    $event.stopPropagation();
    observer.complete();
  }

  onSubmit($event: Event, observer: { complete: () => void }) {
    $event.preventDefault();
    $event.stopPropagation();



    if (this.form.valid && !this.form.disabled) {
      const formValues = this.form.getRawValue();

      const medicalHistoryDto: MedicalHistoryDto = {
        weight: Number(formValues.weight),
        height: Number(formValues.height),
        bloodGroup: formValues.bloodGroup,
        rhesusFactor: formValues.rhesusFactor,
        bloodPressure: formValues.bloodPressure,
        thermometer: formValues.thermometer,
        pregnancyAmount: formValues.pregnancyAmount,
        previousBirthType: formValues.previousBirthType,
        hereditaryDiseases: formValues.hereditaryDiseases,
        isSmoking: formValues.isSmoking === true,
        isConsumingAlcohol: formValues.isConsumingAlcohol === true,
        enduredCovid: formValues.enduredCovid
      };

      // Add optional fields only if they exist
      if (formValues.abortionAmount !== null) {
        medicalHistoryDto.abortionAmount = formValues.abortionAmount;
      }

      if (formValues.miscarriageAmount !== null) {
        medicalHistoryDto.miscarriageAmount = formValues.miscarriageAmount;
      }

      if (formValues.prematureBirthAmount !== null) {
        medicalHistoryDto.prematureBirthAmount = formValues.prematureBirthAmount;
      }

      if (formValues.gynecologicalDiseases) {
        medicalHistoryDto.gynecologicalDiseases = formValues.gynecologicalDiseases;
      }

      if (formValues.somaticDiseases) {
        medicalHistoryDto.somaticDiseases = formValues.somaticDiseases;
      }

      if (formValues.undergoneOperations) {
        medicalHistoryDto.undergoneOperations = formValues.undergoneOperations;
      }

      if (formValues.allergicReactionsRadio === true && formValues.allergicReactions) {
        medicalHistoryDto.allergicReactions = formValues.allergicReactions;
      }

      this.medicalHistoriesService.postMedicalHistory(medicalHistoryDto)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response) => {
            this.alertsService.alertPositive('Анамнез сохранён.');
            this.onObserverComplete.next();
            observer.complete();
          },
          error: (error) => {
            console.error('Error saving medical history', error);
            this.alertsService.alertPositive('Произошла ошибка при сохранении анамнеза.');
          }
        });
    } else {
      console.error('Form is invalid', this.form);
    }
  }

  get allergicReactionsPositiveSelected() {
    return this.form.get('allergicReactionsRadio')?.value
  }

  get isStep0Valid() {
    if (this.isFormReadonly) {
      return true;
    }

    return (
      this.form.get('weight')?.valid &&
      this.form.get('height')?.valid &&
      this.form.get('bloodGroup')?.valid &&
      this.form.get('rhesusFactor')?.valid &&
      this.form.get('bloodPressure')?.valid &&
      this.form.get('thermometer')?.valid
    )
  }

  get isStep1Valid() {
    if (this.isFormReadonly) {
      return true;
    }

    if ((this.form.get('pregnancyAmount')?.value ?? 0) >= 1) {
      return (
        this.form.get('pregnancyAmount')?.valid &&
        this.form.get('abortionAmount')?.valid &&
        this.form.get('miscarriageAmount')?.valid &&
        this.form.get('prematureBirthAmount')?.valid &&
        this.form.get('previousBirthType')?.valid
      )
    } else {
      return (
        this.form.get('pregnancyAmount')?.valid &&
        this.form.get('previousBirthType')?.valid
      )
    }
  }

  get isStep2Valid() {
    return (
      this.form.get('somaticDiseases')?.valid &&
      this.form.get('undergoneOperations')?.valid &&
      this.form.get('allergicReactionsRadio')?.valid &&
      (this.form.get('allergicReactionsRadio')?.value === false
        ? true
        : this.form.get('allergicReactions')?.valid) &&
      this.form.get('hereditaryDiseases')?.valid &&
      this.form.get('isSmoking')?.valid &&
      this.form.get('isConsumingAlcohol')?.valid &&
      this.form.get('enduredCovid')?.valid
    )
  }

  private populateFormWithExistingData(): void {
    if (this.prepopulatedMedicalHistory) {
      this.form.patchValue({
        weight: this.prepopulatedMedicalHistory.weight,
        height: this.prepopulatedMedicalHistory.height,
        bloodGroup: this.prepopulatedMedicalHistory.bloodGroup,
        rhesusFactor: this.prepopulatedMedicalHistory.rhesusFactor,
        bloodPressure: this.prepopulatedMedicalHistory.bloodPressure,
        thermometer: this.prepopulatedMedicalHistory.thermometer,
        pregnancyAmount: this.prepopulatedMedicalHistory.pregnancyAmount,
        previousBirthType: this.prepopulatedMedicalHistory.previousBirthType,
        hereditaryDiseases: this.prepopulatedMedicalHistory.hereditaryDiseases,
        isSmoking: this.prepopulatedMedicalHistory.isSmoking,
        isConsumingAlcohol: this.prepopulatedMedicalHistory.isConsumingAlcohol,
        enduredCovid: this.prepopulatedMedicalHistory.enduredCovid
      });

      if (this.prepopulatedMedicalHistory.abortionAmount !== undefined) {
        const value = this.prepopulatedMedicalHistory.abortionAmount === null ? 0 : this.prepopulatedMedicalHistory.abortionAmount;
        this.form.get('abortionAmount')?.setValue(value);
      }

      if (this.prepopulatedMedicalHistory.miscarriageAmount !== undefined) {
        const value = this.prepopulatedMedicalHistory.miscarriageAmount === null ? 0 : this.prepopulatedMedicalHistory.miscarriageAmount;
        this.form.get('miscarriageAmount')?.setValue(value);
      }

      if (this.prepopulatedMedicalHistory.prematureBirthAmount !== undefined) {
        const value = this.prepopulatedMedicalHistory.prematureBirthAmount === null ? 0 : this.prepopulatedMedicalHistory.prematureBirthAmount;
        this.form.get('prematureBirthAmount')?.setValue(value);
      }

      if (this.prepopulatedMedicalHistory.gynecologicalDiseases) {
        this.form.get('gynecologicalDiseases')?.setValue(this.prepopulatedMedicalHistory.gynecologicalDiseases);
      }

      if (this.prepopulatedMedicalHistory.somaticDiseases) {
        this.form.get('somaticDiseases')?.setValue(this.prepopulatedMedicalHistory.somaticDiseases);
      }

      if (this.prepopulatedMedicalHistory.undergoneOperations) {
        this.form.get('undergoneOperations')?.setValue(this.prepopulatedMedicalHistory.undergoneOperations);
      }

      if (this.prepopulatedMedicalHistory.allergicReactions) {
        this.form.get('allergicReactionsRadio')?.setValue(true);
        this.form.get('allergicReactions')?.setValue(this.prepopulatedMedicalHistory.allergicReactions);
      } else {
        this.form.get('allergicReactionsRadio')?.setValue(false);
      }
    }
  }

  bloodGroupValues = getEnumNumericValues(BloodGroup)
  protected readonly bloodGroupStringify = bloodGroupStringify
  readonly rhesusFactorStringify = rhesusFactorStringify
  protected readonly getEnumNumericValues = getEnumNumericValues
  protected readonly RhesusFactor = RhesusFactor
  protected readonly Thermometer = Thermometer
  protected readonly thermometerStringify = thermometerStringify
  protected readonly BirthType = BirthType
  protected readonly birthTypeStringify = birthTypeStringify
  protected readonly HereditaryDisease = HereditaryDisease
  protected readonly hereditaryDiseaseStringify = hereditaryDiseaseStringify
  protected readonly CovidStatus = CovidStatus
  protected readonly covidStatusStringify = covidStatusStringify
  protected readonly numberMask = numberMask;
  protected readonly bloodPressureMask = bloodPressureMask;
}
