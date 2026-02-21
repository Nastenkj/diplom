import {Component, OnDestroy, OnInit} from '@angular/core';
import {TableUserDto} from "../../dtos/patients/table-user-dto";
import {debounceTime, Subject, takeUntil} from "rxjs";
import {UserRequestDto} from "../../dtos/patients/user-request-dto";
import {TuiButton, TuiIcon, TuiLoader} from "@taiga-ui/core";
import {Router} from "@angular/router";
import {
  TuiTableCell,
  TuiTableDirective,
  TuiTablePagination,
  TuiTablePaginationEvent,
  TuiTableTbody, TuiTableTd, TuiTableTh, TuiTableThGroup, TuiTableTr
} from "@taiga-ui/addon-table";
import {DoctorsService} from "../../shared-services/doctors/doctors.service";
import {NgForOf, NgIf, SlicePipe} from "@angular/common";
import {TuiInputModule, TuiTextareaModule, TuiTextfieldControllerModule} from "@taiga-ui/legacy";
import {FormsModule} from "@angular/forms";
import {TuiAccordion} from "@taiga-ui/kit";

@Component({
  selector: 'app-doctors-list',
  standalone: true,
  imports: [
    NgForOf,
    NgIf,
    SlicePipe,
    TuiButton,
    TuiInputModule,
    TuiLoader,
    TuiTableCell,
    TuiTableDirective,
    TuiTablePagination,
    TuiTableTbody,
    TuiTableTd,
    TuiTableTh,
    TuiTableThGroup,
    TuiTableTr,
    TuiTextareaModule,
    FormsModule,
    TuiAccordion,
    TuiIcon,
    TuiTextfieldControllerModule,
  ],
  templateUrl: './doctors-list.component.html',
  styleUrl: './doctors-list.component.scss'
})
export class DoctorsListComponent implements OnInit, OnDestroy {
  doctors: TableUserDto[] = [];
  loading = false;
  error: string | null = null;

  columns = ['fullName', 'email', 'phoneNumber', 'birthDate', 'actions'];
  page = 0;
  size = 10;
  totalDoctors = 0;
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

  constructor(
    private doctorsService: DoctorsService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loadDoctors();

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

  loadDoctors(): void {
    this.loading = true;
    this.error = null;
    this.doctorsService.getDoctors(this.request).subscribe({
      next: (data) => {
        this.doctors = data.tableUsers;
        this.totalDoctors = data.total;
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

    this.loadDoctors();
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
    this.loadDoctors();
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString();
  }

  navigateToDoctor(id: string | number): void {
    this.router.navigate(['/doctors', id]);
  }
}
