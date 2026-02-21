namespace PregnancyAppBackend.Dtos.Web.Common;

public class UserCommonInfoDto
{
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string TrustedPersonFullName { get; set; } = null!;
    public string TrustedPersonPhoneNumber { get; set; } = null!;
    public string TrustedPersonEmail { get; set; } = null!;
    public string InsuranceNumber { get; set; } = null!;

    public DateTime BirthDate { get; set; }
}