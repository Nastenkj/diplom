using PregnancyAppBackend.Entities.Security;

namespace PregnancyAppBackend.Services.UserService;

public interface IUserService
{
    Task EnsureUniqueUserAsync(string email, string phoneNumber);
    Task<User> CreateUserAsync(string email, string password);
}