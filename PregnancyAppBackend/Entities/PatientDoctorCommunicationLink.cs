using PregnancyAppBackend.Entities.Security;

namespace PregnancyAppBackend.Entities;

public class PatientDoctorCommunicationLink
{
    public Guid UserId { get; set; }
    public string CommunicationLink { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime MeetingScheduledAtUtc { get; set; }
    public Guid DoctorId { get; set; }
    public User Doctor { get; set; } = null!;
    public User User { get; set; } = null!;

}