import {ChangeDetectorRef, Component, OnDestroy, OnInit, PipeTransform} from '@angular/core';
import { SignalRService } from "../../shared-services/hubs/signal-r.service";
import {AsyncPipe, DatePipe, NgClass, NgForOf, NgIf} from "@angular/common";
import { FormsModule } from "@angular/forms";
import {CommunicationLinkDto} from "../../dtos/communication-link/communication-link-dto";
import {last, Subject, takeUntil} from "rxjs";
import {CommunicationLinkService} from "../../shared-services/communication-link/communication-link.service";
import {TuiBadge} from "@taiga-ui/kit";
import {TuiCardMedium} from "@taiga-ui/layout";
import {TuiFormatDatePipe, TuiIcon, TuiSurface, TuiTitle} from "@taiga-ui/core";
import {AuthService} from "../../auth/auth.service";
import {formatLocalDateTime} from "../../shared-services/date/local-date-formatter";
import {AlertsService} from "../../shared-services/alerts/alerts.service";

@Component({
  selector: 'app-communication-link',
  standalone: true,
  imports: [
    NgForOf,
    NgIf,
    FormsModule,
    NgClass,
    AsyncPipe,
    TuiBadge,
    TuiCardMedium,
    TuiFormatDatePipe,
    TuiIcon,
    TuiSurface,
    TuiTitle,
    DatePipe
  ],
  templateUrl: './communication-link.component.html',
  styleUrls: ['./communication-link.component.scss', '../../../styles.scss']
})
export class CommunicationLinkComponent implements OnInit, OnDestroy {
  communicationLinks: CommunicationLinkDto[] = [];
  isLoading = true;
  error: string | null = null;

  userIsDoctor: boolean | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private communicationService: CommunicationLinkService,
    private signalRService: SignalRService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef,
    private notificationService: AlertsService
  ) { }

  ngOnInit(): void {
    this.loadCommunicationLinks();
    this.initializeSignalR();

    this.authService.currentUserSubject
      .pipe(takeUntil(this.destroy$))
      .subscribe(val => {
        if (val) {
          this.userIsDoctor = val.roles === 'Врач';
        } else {
          this.userIsDoctor = null;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeSignalR(): void {
    this.signalRService.startConnection()
      .catch(err => console.error('Failed to establish SignalR connection', err));

    this.signalRService.messages$
      .pipe(takeUntil(this.destroy$))
      .subscribe(message => {
        if (message) {
          try {
            const parsedMessage = JSON.parse(message.message);
            if (parsedMessage.type === 'communication_link') {
              const existingLinkIndex = this.communicationLinks.findIndex(
                link => link.userId.toString() === parsedMessage.patientId
              );

              if (existingLinkIndex >= 0) {
                this.communicationLinks[existingLinkIndex].communicationLink = parsedMessage.link;
                this.communicationLinks[existingLinkIndex].meetingScheduledAtUtc = parsedMessage.meetingScheduledAtUtc;
                this.communicationLinks[existingLinkIndex].createdAtUtc = parsedMessage.createdAtUtc;
              } else {
                this.communicationLinks.push({
                  createdAtUtc: parsedMessage.createdAtUtc,
                  meetingScheduledAtUtc: parsedMessage.meetingScheduledAtUtc,
                  userId: parsedMessage.patientId,
                  userName: parsedMessage.userName,
                  doctorId: parsedMessage.doctorId,
                  doctorName: parsedMessage.doctorName,
                  communicationLink: parsedMessage.link
                });
              }

              // console.log(this.communicationLinks);
              this.cdr.detectChanges();
              this.isLoading = false;
            }
          } catch (e) {
            console.error('Failed to parse message', e);
          }
        }
      });
  }

  private loadCommunicationLinks(): void {
    this.isLoading = true;
    this.communicationService.getCommunicationLinks()
      .subscribe({
        next: (links: CommunicationLinkDto[]) => {
          this.communicationLinks = links;
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Error loading communication links', err);
          this.error = 'Failed to load links';
          this.isLoading = false;
        }
      });
  }

  protected readonly last = last;
  protected readonly formatLocalDateTime = formatLocalDateTime;
}
