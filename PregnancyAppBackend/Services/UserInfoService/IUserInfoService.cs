using PregnancyAppBackend.Dtos.Authentication;

namespace PregnancyAppBackend.Services.UserInfoService;

public interface IUserInfoService
{
    UserInfo GetUserInfoFromToken();
    Task<bool> CheckUserIsAdminAsync();
    Task<bool> CheckUserIsDoctorAsync();
    Task EnsureUserHasAccessToPatientInfoAsync();
    Task<bool> CheckUserIsPatientAsync();
    Task EnsureUserIsDoctorAsync();
    Task EnsureUserIsAdminAsync();
    
    Task EnsureUserIsDoctorOrAdminAsync();

}