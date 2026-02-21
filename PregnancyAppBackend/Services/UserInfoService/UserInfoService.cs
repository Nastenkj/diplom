using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Dtos.Authentication;
using PregnancyAppBackend.Entities.Security;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Persistance;
using System.Net;
using System.Security.Claims;

namespace PregnancyAppBackend.Services.UserInfoService;

public class UserInfoService : IUserInfoService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserInfoService(IHttpContextAccessor httpContextAccessor,
                           IDatabaseContext databaseContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _databaseContext = databaseContext;
    }

    public UserInfo GetUserInfoFromToken()
    {
        var userIdentity = _httpContextAccessor.HttpContext?.User;

        if (userIdentity is null || !userIdentity.Identity!.IsAuthenticated)
        {
            throw new ApiException("Пользователь не авторизован", "User is not authorized.", HttpStatusCode.Unauthorized);
        }

        var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
        {
            throw new ApiException($"{nameof(userId)} is empty in token", HttpStatusCode.Unauthorized);
        }

        return new UserInfo
        {
            UserId = userIdGuid
        };
    }

    public async Task<bool> CheckUserIsAdminAsync()
    {
        var userId = GetUserInfoFromToken().UserId;

        return await _databaseContext.Users.AnyAsync(u => u.Id == userId && u.Roles.Any(r => r.Id.ToString() == Role.AdministratorId));
    }

    public async Task<bool> CheckUserIsDoctorAsync()
    {
        var userId = GetUserInfoFromToken().UserId;

        return await _databaseContext.Users.AnyAsync(u => u.Id == userId && u.Roles.Any(r => r.Id.ToString() == Role.DoctorId));
    }
    
    public async Task EnsureUserIsDoctorAsync()
    {
        if (!await CheckUserIsDoctorAsync())
        {
            throw new ApiException("No access to doctor.", 
                                   "Пользователь не найден. Попробуйте позже.");   
        }
    }
    
    public async Task EnsureUserIsAdminAsync()
    {
        if (!await CheckUserIsAdminAsync())
        {
            throw new ApiException("No access to admin.", 
                                   "Пользователь не найден. Попробуйте позже.");   
        }
    }

    public async Task EnsureUserIsDoctorOrAdminAsync()
    {
        await EnsureUserIsAdminAsync();
        await EnsureUserIsDoctorAsync();
    }

    public async Task EnsureUserHasAccessToPatientInfoAsync()
    {
        var userIsDoctor = await CheckUserIsDoctorAsync();
        var userIsAdmin = await CheckUserIsAdminAsync();

        if (!userIsAdmin && !userIsDoctor)
        {
            throw new ApiException("No access to user.", 
                                   "Пациент не найден. Попробуйте позже.");   
        }
    }

    public async Task<bool> CheckUserIsPatientAsync()
    {
        var userId = GetUserInfoFromToken().UserId;

        var user = await _databaseContext.Users.SingleAsync(u => u.Id == userId);

        return await _databaseContext.Users.AnyAsync(u => u.Id == userId && u.Roles.Any(r => r.Id.ToString() == Role.PatientId));
    }
}