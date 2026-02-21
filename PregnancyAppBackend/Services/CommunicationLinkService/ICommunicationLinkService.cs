using PregnancyAppBackend.Dtos.Web.CommunicationLinks;

namespace PregnancyAppBackend.Services.CommunicationLinkService;

public interface ICommunicationLinkService
{
    Task<PatientDoctorCommunicationLinkDto> CreateCommunicationLinkAsync(CreateCommunicationLinkDto createDto);
    Task<List<PatientDoctorCommunicationLinkDto>> GetUserCommunicationLinksAsync();
    Task ClearDeadCommunicationLinksAsync(TimeSpan expireTime);
}