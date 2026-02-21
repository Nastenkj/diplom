using PregnancyAppBackend.Entities.Security;

namespace PregnancyAppBackend.Services.TokenService;

public interface ITokenService
{
    string CreateToken(User user);
}