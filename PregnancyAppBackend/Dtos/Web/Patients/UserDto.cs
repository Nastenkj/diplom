namespace PregnancyAppBackend.Dtos.Web.Patients;

public class UserDto : TableUserDto
{
    public string TrustedPersonFullName { get; set; }
    public string TrustedPersonPhoneNumber { get; set; }
    public string TrustedPersonEmail { get; set; }
    public string InsuranceNumber { get; set; }

    public DateOnly BirthDate { get; set; }
}