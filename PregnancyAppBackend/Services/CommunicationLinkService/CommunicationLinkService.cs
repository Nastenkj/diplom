using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Converters;
using PregnancyAppBackend.Dtos.Web.CommunicationLinks;
using PregnancyAppBackend.Entities;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Hubs;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.UserInfoService;

namespace PregnancyAppBackend.Services.CommunicationLinkService;

public class CommunicationLinkService : ICommunicationLinkService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly IUserInfoService _userInfoService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CommunicationLinkService> _logger;

    public CommunicationLinkService(IDatabaseContext databaseContext, 
                                    IUserInfoService userInfoService,
                                    INotificationService notificationService,
                                    ILogger<CommunicationLinkService> logger)
    {
        _databaseContext = databaseContext;
        _userInfoService = userInfoService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<PatientDoctorCommunicationLinkDto> CreateCommunicationLinkAsync(CreateCommunicationLinkDto createDto)
    {
        var doctorId = _userInfoService.GetUserInfoFromToken().UserId;
        
        _logger.LogInformation("Creating communication link. UserId={doctorId}. Dto={@dto}", doctorId, createDto);
        
        var patientId = createDto.PatientId;
        
        var existingLink = await _databaseContext.PatientDoctorCommunicationLinks
                                                 .FirstOrDefaultAsync(c => c.UserId == patientId && c.DoctorId == doctorId);
            
        PatientDoctorCommunicationLink link;
        
        if (existingLink != null)
        {
            existingLink.CommunicationLink = createDto.CustomLink;
            existingLink.MeetingScheduledAtUtc = createDto.MeetingScheduledAtUtc;
            
            _databaseContext.PatientDoctorCommunicationLinks.Update(existingLink);
            link = existingLink;
        }
        else
        {
            link = createDto.ConvertToEntity(doctorId);
            link.CommunicationLink = createDto.CustomLink;
            link.MeetingScheduledAtUtc = createDto.MeetingScheduledAtUtc;
            
            _databaseContext.PatientDoctorCommunicationLinks.Add(link);
        }
        
        await _databaseContext.SaveChangesAsync();

        var user = await _databaseContext.UserCommonInfos.SingleAsync(u => u.User.Id == link.UserId);
        var doctor = await _databaseContext.UserCommonInfos.SingleAsync(u => u.User.Id == link.DoctorId);
        
        var linkDto = link.ConvertToDto(user.FullName, doctor.FullName);
        
        await _notificationService.SendNotification(patientId.ToString(),
                                                    $"{{\"type\":\"communication_link\"," +
                                                    $"\"patientId\":\"{link.UserId}\"," +
                                                    $"\"userName\":\"{user.FullName}\"," +
                                                    $"\"doctorId\":\"{doctorId}\"," +
                                                    $"\"doctorName\":\"{doctor.FullName}\"," +
                                                    $"\"link\":\"{link.CommunicationLink}\"," +
                                                    $"\"createdAtUtc\":\"{link.CreatedAtUtc:o}\"," +
                                                    $"\"meetingScheduledAtUtc\":\"{link.MeetingScheduledAtUtc:o}\"" +
                                                    $"}}");
            
        return linkDto;
    }

    public async Task<List<PatientDoctorCommunicationLinkDto>> GetUserCommunicationLinksAsync()
    {
        var userGuid = _userInfoService.GetUserInfoFromToken().UserId;
        
        var links = await _databaseContext.PatientDoctorCommunicationLinks
                                          .Where(c => c.UserId == userGuid || c.DoctorId == userGuid)
                                          .ToListAsync();
            
        var result = new List<PatientDoctorCommunicationLinkDto>();
        
        foreach (var link in links)
        {
            var doctor = await _databaseContext.UserCommonInfos.SingleOrDefaultAsync(uci => uci.UserId == link.DoctorId);
            var patient = await _databaseContext.UserCommonInfos.SingleOrDefaultAsync(uci => uci.UserId == link.UserId);

            if (doctor is null)
            {
                throw new ApiException($"Doctor with userId={link.DoctorId} not found.", "Id доктора не найден. Попробуйте позже.");
            }
            
            if (patient is null)
            {
                throw new ApiException($"Patient with userId={link.UserId} not found.", "Id пациента не найден. Попробуйте позже.");
            }

            result.Add(link.ConvertToDto(patient.FullName, doctor.FullName));
        }
        
        return result;
    }
    
    public async Task ClearDeadCommunicationLinksAsync(TimeSpan expireTime)
    {
        var expirationThreshold = DateTime.UtcNow - expireTime;
    
        var expiredLinks = await _databaseContext.PatientDoctorCommunicationLinks
                                                 .Where(link => link.MeetingScheduledAtUtc < expirationThreshold)
                                                 .ToListAsync();
            
        _databaseContext.PatientDoctorCommunicationLinks.RemoveRange(expiredLinks);
        await _databaseContext.SaveChangesAsync();
        
        _logger.LogInformation("Cleared {Count} dead communication links.", expiredLinks.Count);
    }
}