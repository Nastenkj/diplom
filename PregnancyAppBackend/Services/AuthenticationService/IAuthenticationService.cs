using PregnancyAppBackend.Entities.Security;

namespace PregnancyAppBackend.Services.AuthenticationService;

public interface IAuthenticationService
{
    Task<User> AuthenticateAsync(string email, string password);
}