export interface CreateCommunicationLinkDto {
  patientId: string;
  customLink?: string;
  meetingScheduledAtUtc: Date;
}
