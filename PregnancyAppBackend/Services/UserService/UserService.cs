using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Entities.Security;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Persistance;

namespace PregnancyAppBackend.Services.UserService;

public class UserService : IUserService
{
    private readonly IDatabaseContext _databaseContext;

    public UserService(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task EnsureUniqueUserAsync(string email, string phoneNumber)
    {
        var exist = await _databaseContext.UserCommonInfos.AnyAsync(pci => pci.User.Email == email || pci.PhoneNumber == phoneNumber);

        if (exist)
        {
            throw new ApiException($"User with email={email}, phoneNumber={phoneNumber} already exists.", 
                                   "Пользователь с данной почтой или номером телефона уже существует.");
        }
    }

    public async Task<User> CreateUserAsync(string email, string password)
    {
        var user = new User
        {
            Email = email,
            PasswordHash = HashService.HashService.SHA512(password),
            IsDeleted = false
        };

        await _databaseContext.Users.AddAsync(user);
        await _databaseContext.SaveChangesAsync();

        return user;
    }
}