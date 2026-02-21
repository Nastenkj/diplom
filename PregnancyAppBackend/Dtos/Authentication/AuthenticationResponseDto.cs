namespace PregnancyAppBackend.Dtos.Authentication;

public class AuthenticationResponseDto
{
    public string JwtToken { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; }
}