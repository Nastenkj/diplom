namespace PregnancyAppBackend.Dtos;

public class PatientDoctorCommunicationLinkDto
{
    public Guid UserId { get; set; }
    public Guid DoctorId { get; set; }

    public string CommunicationLink { get; set; }
}