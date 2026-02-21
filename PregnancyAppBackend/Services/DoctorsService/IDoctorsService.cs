using PregnancyAppBackend.Dtos.Authentication;
using PregnancyAppBackend.Dtos.Web.Patients;
using PregnancyAppBackend.Entities.Security;

namespace PregnancyAppBackend.Services.DoctorsService;

public interface IDoctorsService
{
    Task<User> CreateDoctorAsync(UserRegistrationRequestDto request);
    Task<TableUsersDto> GetDoctorsAsync(TableUserRequestDto request);
    Task<UserDto> GetDoctorAsync(Guid? request);
    Task<UserDto> EditDoctorInfoAsync(UserDto userEditDto);
}