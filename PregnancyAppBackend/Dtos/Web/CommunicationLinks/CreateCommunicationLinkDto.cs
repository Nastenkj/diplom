namespace PregnancyAppBackend.Dtos.Web.CommunicationLinks;

public class CreateCommunicationLinkDto
{
    public Guid PatientId { get; set; }
    public string CustomLink { get; set; }
    public DateTime MeetingScheduledAtUtc { get; set; }
}