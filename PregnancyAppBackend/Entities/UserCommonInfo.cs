using PregnancyAppBackend.Entities.Security;

namespace PregnancyAppBackend.Entities;

public class UserCommonInfo : Entity
{
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? TrustedPersonFullName { get; set; }
    public string? TrustedPersonPhoneNumber { get; set; }
    public string? TrustedPersonEmail { get; set; }
    public string? InsuranceNumber { get; set; }

    public DateOnly BirthDate { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}