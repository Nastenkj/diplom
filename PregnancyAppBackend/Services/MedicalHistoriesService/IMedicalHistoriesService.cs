using PregnancyAppBackend.Dtos.Web.MedicalHistory;
using PregnancyAppBackend.Persistance;

namespace PregnancyAppBackend.Services.MedicalHistoriesService;

public interface IMedicalHistoriesService
{
    Task<MedicalHistoryDto> AddMedicalHistoryAsync(MedicalHistoryDto medicalHistoryDto);
    
    /// <summary>
    /// Returns null if medical history has not been populated yet.
    /// </summary>
    Task<MedicalHistoryDto?> GetMedicalHistoryAsync(Guid userId, IDatabaseContext? databaseContext = null);
}