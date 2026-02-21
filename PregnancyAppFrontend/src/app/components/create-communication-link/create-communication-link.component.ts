import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {AsyncPipe, DatePipe, JsonPipe, NgIf} from '@angular/common';
import {TuiButton, TuiDialogContext, TuiError} from '@taiga-ui/core';
import {TuiFieldErrorPipe} from '@taiga-ui/kit';
import {TuiInputDateTimeModule, TuiInputModule, TuiUnfinishedValidator} from '@taiga-ui/legacy';
import {TuiDay, TuiTime} from '@taiga-ui/cdk';
import {CommunicationLinkService} from '../../shared-services/communication-link/communication-link.service';
import {CommunicationLinkDto} from '../../dtos/communication-link/communication-link-dto';
import {TableUserDto} from '../../dtos/patients/table-user-dto';
import {injectContext} from '@taiga-ui/polymorpheus';
import {formatLocalDateTime} from "../../shared-services/date/local-date-formatter";

@Component({
  selector: 'app-create-communication-link',
  standalone: true,
  imports: [
    NgIf,
    AsyncPipe,
    JsonPipe,
    DatePipe,
    ReactiveFormsModule,
    TuiFieldErrorPipe,
    TuiInputModule,
    TuiInputDateTimeModule,
    TuiUnfinishedValidator,
    TuiButton,
    TuiError
  ],
  templateUrl: './create-communication-link.component.html',
  styleUrls: ['./create-communication-link.component.scss']
})
export class CreateCommunicationLinkComponent implements OnInit {
  // Access dialog context using injectContext
  readonly context = injectContext<TuiDialogContext<CommunicationLinkDto, TableUserDto>>();

  linkForm!: FormGroup;
  isCreatingLink = false;
  createdLink: CommunicationLinkDto | null = null;
  name = ''; // Added for template binding

  constructor(
    private fb: FormBuilder,
    private communicationService: CommunicationLinkService
  ) {}

  ngOnInit(): void {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const tomorrowDay = new TuiDay(
      tomorrow.getFullYear(),
      tomorrow.getMonth(),
      tomorrow.getDate()
    );

    this.linkForm = this.fb.group({
      customLink: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(500)]],
      meetingDateTime: [
        [tomorrowDay, new TuiTime(10, 0)],
        [Validators.required, this.completeDateTimeValidator]
      ]
    });
  }

  // Get patient data from dialog context
  get patient(): TableUserDto {
    return this.context.data;
  }

  // Check if form has value for submit button
  get hasValue(): boolean {
    return this.linkForm && this.linkForm.valid;
  }

  createLink(event: Event): void {
    event.stopPropagation();

    if (this.linkForm.invalid) {
      this.linkForm.markAllAsTouched();
      return;
    }

    this.isCreatingLink = true;

    const formValue = this.linkForm.value;
    const [day, time] = formValue.meetingDateTime;

    const localDate = new Date(
      day.year,
      day.month,
      day.day,
      time.hours,
      time.minutes
    );

    const utcDate = new Date(
      localDate.getTime() + localDate.getTimezoneOffset() * 60000
    );

    // console.log('Local date:', localDate);
    // console.log('UTC date:', utcDate);

    this.communicationService.createCommunicationLink({
      patientId: this.patient.id,
      customLink: formValue.customLink,
      meetingScheduledAtUtc: localDate
    }).subscribe({
      next: (response: CommunicationLinkDto) => {
        this.isCreatingLink = false;
        this.createdLink = response;
      },
      error: (err) => {
        console.error('Error creating communication link', err);
        this.isCreatingLink = false;
      }
    });
  }

  // Submit dialog with result
  submit(): void {
    if (this.createdLink) {
      this.context.completeWith(this.createdLink);
    }
  }

  // Close dialog without result
  cancel(): void {
    this.context.$implicit.complete();
  }

  getFormControl(name: string): FormControl {
    return this.linkForm.get(name) as FormControl;
  }

  private completeDateTimeValidator(control: FormControl): {[key: string]: boolean} | null {
    const value = control.value;
    if (!value || !Array.isArray(value) || value.length !== 2) {
      return { incompleteDateTime: true };
    }

    const [day, time] = value;
    return day && time ? null : { incompleteDateTime: true };
  }

  protected readonly formatLocalDateTime = formatLocalDateTime;
}
