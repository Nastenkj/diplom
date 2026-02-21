namespace PregnancyAppBackend.Dtos.Authentication;

public class UserRegistrationRequestDto
{
    public string FullName { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}