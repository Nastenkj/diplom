namespace PregnancyAppBackend.Dtos.Authentication;

public class PatientRegistrationRequestDto : UserRegistrationRequestDto
{
    public string TrustedPersonFullName { get; set; } = null!;
    public string TrustedPersonPhoneNumber { get; set; } = null!;
    public string TrustedPersonEmail { get; set; } = null!;
    public string InsuranceNumber { get; set; } = null!;
}