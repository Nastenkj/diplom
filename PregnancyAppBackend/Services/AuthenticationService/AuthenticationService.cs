using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Entities.Security;
using PregnancyAppBackend.Persistance;
using System.Security.Authentication;

namespace PregnancyAppBackend.Services.AuthenticationService;

public class AuthenticationService : IAuthenticationService
{
    private readonly IDatabaseContext _databaseContext;

    public AuthenticationService(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
    }

    public async Task<User> AuthenticateAsync(string email, string password)
    {
        email = email.ToLower().Trim();

        var passwordHash = HashService.HashService.SHA512(password);

        var user = await _databaseContext.Users
                                         .AsNoTracking()
                                         .Include(u => u.Roles)
                                         .ThenInclude(r => r.ApiClaims)
                                         .SingleOrDefaultAsync(u => u.Email == email &&
                                                                    u.PasswordHash == passwordHash);

        if (user == null || user.IsDeleted)
        {
            var errorMessage = "Неверный логин или пароль.";

            throw new AuthenticationException(errorMessage);
        }

        return user;
    }
}