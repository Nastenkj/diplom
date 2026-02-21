namespace PregnancyAppBackend.Dtos.Web.CommunicationLinks;

public class PatientDoctorCommunicationLinkDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; }
    public string CommunicationLink { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    public DateTime MeetingScheduledAtUtc { get; set; }
}