namespace PregnancyAppBackend.Services.Excel;

public interface IExcelExportService
{
    Task<byte[]> ExportDailySurveyToExcelAsync(Guid surveyId);
    Task<byte[]> ExportWeeklySurveyToExcelAsync(Guid surveyId);
    Task<byte[]> ExportAllDailySurveysToExcelAsync(Guid userId);
    Task<byte[]> ExportAllWeeklySurveysToExcelAsync(Guid userId);
    Task<byte[]> ExportMedicalHistoryToExcelAsync(Guid userId);
    Task<byte[]> ExportObservationParametersStatisticsAsync(Guid userId);
}