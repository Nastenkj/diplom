import {Component, Injector, OnDestroy, OnInit} from '@angular/core';
import {PatientsService} from "../../shared-services/patients/patients.service";
import {TableUserDto} from "../../dtos/patients/table-user-dto";
import {JsonPipe, NgForOf, NgIf, SlicePipe} from "@angular/common";
import {TuiTable, TuiTablePagination, TuiTablePaginationEvent} from "@taiga-ui/addon-table";
import {TuiButton, tuiDialog, TuiIcon, TuiLoader, TuiTextfieldDirective} from "@taiga-ui/core";
import {Router, RouterLink} from "@angular/router";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {TuiInputModule, TuiTextfieldControllerModule} from "@taiga-ui/legacy";
import {Subject, debounceTime, takeUntil} from "rxjs";
import {SignalRService} from "../../shared-services/hubs/signal-r.service";
import {CreateCommunicationLinkComponent} from "../create-communication-link/create-communication-link.component";
import {UserRequestDto} from "../../dtos/patients/user-request-dto";
import {HasPermissionDirective} from "../../directives/has-permission.directive";
import {TuiAccordion} from "@taiga-ui/kit";

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [
    TuiLoader,
    NgIf,
    TuiTable,
    NgForOf,
    TuiButton,
    TuiTablePagination,
    FormsModule,
    TuiInputModule,
    ReactiveFormsModule,
    HasPermissionDirective,
    TuiAccordion,
    TuiIcon,
    TuiTextfieldControllerModule
  ],
  templateUrl: './patient-list.component.html',
  styleUrl: './patient-list.component.scss'
})
export class PatientListComponent implements OnInit, OnDestroy {
  patients: TableUserDto[] = [];
  loading = false;
  error: string | null = null;

  columns = ['fullName', 'email', 'phoneNumber', 'birthDate', 'actions'];
  page = 0;
  size = 10;
  totalPatients = 0;
  readonly sizeOptions = [10, 20, 50, 100];

  nameFilter = '';
  emailFilter = '';
  phoneFilter = '';
  private destroy$ = new Subject<void>();

  private nameFilterChange$ = new Subject<string>();
  private emailFilterChange$ = new Subject<string>();
  private phoneFilterChange$ = new Subject<string>();

  request: UserRequestDto = {
    pageNumber: 1,
    pageSize: 10
  };

  private readonly dialog = tuiDialog(CreateCommunicationLinkComponent, {
    dismissible: true,
    label: 'Создать ссылку для встречи',
    size: 'l'
  });

  constructor(
    private patientsService: PatientsService,
    private router: Router,
    private signalRService: SignalRService
  ) {}

  ngOnInit(): void {
    this.initializeSignalR();
    this.loadPatients();

    this.nameFilterChange$.pipe(
      debounceTime(350),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.applyFilters();
    });

    this.emailFilterChange$.pipe(
      debounceTime(350),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.applyFilters();
    });

    this.phoneFilterChange$.pipe(
      debounceTime(350),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.applyFilters();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onNameFilterChange(value: string): void {
    this.nameFilter = value;
    this.nameFilterChange$.next(value);
  }

  onEmailFilterChange(value: string): void {
    this.emailFilter = value;
    this.emailFilterChange$.next(value);
  }

  onPhoneFilterChange(value: string): void {
    this.phoneFilter = value;
    this.phoneFilterChange$.next(value);
  }

  loadPatients(): void {
    this.loading = true;
    this.error = null;
    this.patientsService.getPatients(this.request).subscribe({
      next: (data) => {
        this.patients = data.tableUsers;
        this.totalPatients = data.total;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load patients';
        this.loading = false;
        console.error('Error loading patients:', err);
      }
    });
  }

  onPagination({page, size}: TuiTablePaginationEvent): void {
    this.page = page;
    this.size = size;

    this.request = {
      ...this.request,
      pageNumber: page + 1,
      pageSize: size
    };

    this.loadPatients();
  }

  applyFilters(): void {
    this.page = 0;
    this.request = {
      ...this.request,
      name: this.nameFilter || '',
      email: this.emailFilter || '',
      phoneNumber: this.phoneFilter || '',
      pageNumber: 1
    };

    this.loadPatients();
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString();
  }

  navigateToPatient(id: string | number): void {
    this.router.navigate(['/patients', id]);
  }

  initializeSignalR(): void {
    this.signalRService.startConnection()
      .then(() => {
        console.log('SignalR connection established');
        this.setupSignalRSubscriptions();
      })
      .catch(err => console.error('Failed to establish SignalR connection', err));
  }

  private setupSignalRSubscriptions(): void {
    this.signalRService.messages$
      .pipe(takeUntil(this.destroy$))
      .subscribe(message => {
        if (message) {
          try {
            const parsedMessage = JSON.parse(message.message);
            if (parsedMessage.type === 'communication_link') {
              this.handleCommunicationLinkMessage(parsedMessage);
            }
          } catch (e) {
            console.error('Failed to parse message', e);
          }
        }
      });
  }

  handleCommunicationLinkMessage(message: any): void {
  }

  openCommunicationLinkDialog(patient: TableUserDto, event: Event): void {
    event.stopPropagation();
    this.dialog(patient).subscribe({
      next: (result) => {
        if (result) {

        }
      },
      complete: () => {
        console.info('Dialog closed');
      }
    });
  }
}
