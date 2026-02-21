import {Component, Input, OnInit} from '@angular/core';
import {CommonModule, NgForOf, NgIf} from '@angular/common';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {
  ObservationParameterNormsService
} from "../../shared-services/observation-parameter-norms/observation-parameter-norms.service";
import {ObservationParameterNormDto} from "../../dtos/observation-parameter-norms/observation-parameter-norm-dto";
import {StatisticFields} from "../../dtos/internal/statistic-fields-enum";
import {TuiHeader} from "@taiga-ui/layout";
import {TuiButton, TuiLoader, TuiTitle} from "@taiga-ui/core";
import {TuiInputNumberModule, TuiSelectModule} from "@taiga-ui/legacy";
import {displayNames} from "../../dtos/internal/parameter-display-names";
import {AlertsService} from "../../shared-services/alerts/alerts.service";

@Component({
  selector: 'app-observation-parameters',
  standalone: true,
  imports: [
    TuiHeader,
    TuiTitle,
    NgIf,
    TuiLoader,
    ReactiveFormsModule,
    TuiSelectModule,
    NgForOf,
    TuiInputNumberModule,
    TuiButton

  ],
  templateUrl: './observation-parameters.component.html',
  styleUrl: './observation-parameters.component.scss'
})
export class ObservationParametersComponent implements OnInit {
  @Input() doctorId: string | null = null;

  form: FormGroup;
  parameters: ObservationParameterNormDto[] = [];
  parameterNames: StatisticFields[] = [];
  selectedParameter: ObservationParameterNormDto | null = null;
  loading = false;
  submitting = false;

  constructor(private readonly observationParametersService: ObservationParameterNormsService, private notificationService: AlertsService) {
    this.form = new FormGroup({
      parameterName: new FormControl(null, [Validators.required]),
      lowerBound: new FormControl(null, [Validators.required]),
      upperBound: new FormControl(null, [Validators.required])
    });
  }

  private loadData() {
    this.loading = true;
    if (this.doctorId) {
      this.observationParametersService.getObservationParameterNorms(this.doctorId).subscribe({
        next: (parameters: ObservationParameterNormDto[]) => {
          this.parameters = parameters;
          this.extractParameterNames();
          this.loading = false;
        },
        error: (error) => {
          console.error('Failed to load parameter data', error);
          this.loading = false;
        }
      });
    } else {
      this.observationParametersService.getObservationParameterNorms().subscribe({
        next: (parameters: ObservationParameterNormDto[]) => {
          this.parameters = parameters;
          this.extractParameterNames();
          this.loading = false;
        },
        error: (error) => {
          console.error('Failed to load parameter data', error);
          this.loading = false;
        }
      });
    }
  }

  private extractParameterNames() {
    this.parameterNames = this.parameters.map(p => p.parameterName);
  }

  ngOnInit() {
    this.loadData();

    // Subscribe to parameter name changes
    this.form.get('parameterName')?.valueChanges.subscribe(parameterName => {
      if (parameterName) {
        this.onParameterSelect(parameterName);
      }
    });
  }

  onParameterSelect(parameterName: StatisticFields) {
    this.selectedParameter = this.parameters.find(p => p.parameterName === parameterName) || null;
    if (this.selectedParameter) {
      this.form.patchValue({
        lowerBound: this.selectedParameter.lowerBound,
        upperBound: this.selectedParameter.upperBound
      });
    }
  }

  onSubmit() {
    if (this.form.valid && this.form.value.parameterName) {
      this.submitting = true;
      const userId = this.doctorId || '';
      this.observationParametersService.updateObservationParameterNormBounds(
        this.form.value.lowerBound,
        this.form.value.upperBound,
        this.form.value.parameterName,
        userId
      ).subscribe({
        next: (updated: ObservationParameterNormDto) => {
          // Update the parameter in the list
          const index = this.parameters.findIndex(p => p.parameterName === updated.parameterName);
          if (index !== -1) {
            this.parameters[index] = updated;
          } else {
            this.parameters.push(updated);
            this.extractParameterNames();
          }
          this.selectedParameter = updated;
          this.submitting = false;
          this.notificationService.alertPositive("Параметр успешно обновлен.");
        },
        error: (error) => {
          console.error('Failed to update parameter', error);
          this.submitting = false;
          this.notificationService.alertPositive("Произошла ошибка при обновлении параметра. Попробуйте позже.");
        }
      });
    }
  }

  // Stringify functions for the UI
  parameterNameStringify = (parameterName: StatisticFields): string => {
    if (!parameterName) return '';
    return displayNames[parameterName] || '';
  }
}
