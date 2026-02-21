using PregnancyAppBackend.Dtos.Authentication;
using PregnancyAppBackend.Dtos.Web.Patients;
using PregnancyAppBackend.Entities.Security;

namespace PregnancyAppBackend.Services.PatientsService;

public interface IPatientsService
{
    Task<User> CreatePatientAsync(PatientRegistrationRequestDto request);
    Task<TableUsersDto> GetPatientsAsync(TableUserRequestDto request);
    Task<UserDto> GetPatientAsync(Guid? request);
    Task<UserDto> EditPatientInfoAsync(UserDto userEditDto);
}