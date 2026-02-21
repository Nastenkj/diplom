using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Converters;
using PregnancyAppBackend.Dtos.Web.MedicalHistory;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.UserInfoService;

namespace PregnancyAppBackend.Services.MedicalHistoriesService;

public class MedicalHistoriesService : IMedicalHistoriesService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<MedicalHistoriesService> _logger;

    public MedicalHistoriesService(IDatabaseContext databaseContext, IUserInfoService userInfoService, ILogger<MedicalHistoriesService> logger)
    {
        _databaseContext = databaseContext;
        _userInfoService = userInfoService;
        _logger = logger;
    }

    public async Task<MedicalHistoryDto> AddMedicalHistoryAsync(MedicalHistoryDto medicalHistoryDto)
    {
        var userId = _userInfoService.GetUserInfoFromToken().UserId;

        _logger.LogInformation("Adding medical history for userId={userId}, dto={@medicalHistoryDto}", userId, medicalHistoryDto);
        
        var medicalHistoryAlreadyExists = await _databaseContext.MedicalHistories.AnyAsync(mh => mh.UserId == userId);

        if (medicalHistoryAlreadyExists)
        {
            throw new ApiException($"Medical history for user with id={userId} already exists.", "Анамнез уже заполнен.");
        }
        
        var entity = await _databaseContext.MedicalHistories.AddAsync(medicalHistoryDto.ConvertToEntity());
        var medicalHistoryEntity = entity.Entity;
        
        // TODO move to configuration
        medicalHistoryEntity.CreationDateUtc = DateTime.UtcNow;
        medicalHistoryEntity.UserId = userId;

        await _databaseContext.SaveChangesAsync();

        return medicalHistoryEntity.ConvertToDto();
    }

    public async Task<MedicalHistoryDto?> GetMedicalHistoryAsync(Guid userId, IDatabaseContext? databaseContext = null)
    {
        databaseContext ??= _databaseContext;
        
        var medicalHistory = await databaseContext.MedicalHistories.SingleOrDefaultAsync(mh => mh.UserId == userId);

        return medicalHistory?.ConvertToDto();
    }
}