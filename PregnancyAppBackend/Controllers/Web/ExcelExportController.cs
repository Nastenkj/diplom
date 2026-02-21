using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Services.Excel;
using PregnancyAppBackend.Services.UserInfoService;

namespace PregnancyAppBackend.Controllers.Web;

[Route("api/excel")]
public class ExcelExportController : WebController
{
    private readonly IExcelExportService _excelExportService;
    private readonly IUserInfoService _userInfoService;

    public ExcelExportController(IExcelExportService excelExportService, IUserInfoService userInfoService)
    {
        _excelExportService = excelExportService;
        _userInfoService = userInfoService;
    }
    
    [HttpGet("daily-surveys")]
    [Authorize(Policy = Policies.GetDailySurveysForUser)]
    public async Task<IActionResult> GetDailySurveysExport(Guid? userId)
    {
        if (userId is not null)
        {
            await _userInfoService.EnsureUserHasAccessToPatientInfoAsync();
        }
        else
        {
            userId = _userInfoService.GetUserInfoFromToken().UserId;
        }

        var excelData = await _excelExportService.ExportAllDailySurveysToExcelAsync(userId.Value);
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ЕжедневныеОпросы.xlsx");
    }
    
    [HttpGet("daily-survey/{surveyId}")]
    [Authorize(Policy = Policies.GetDailySurveyById)]
    public async Task<IActionResult> GetDailySurveyExport(Guid surveyId)
    {
        var excelData = await _excelExportService.ExportDailySurveyToExcelAsync(surveyId);
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ЕжедневныйОпрос.xlsx");
    }
    
    [HttpGet("weekly-surveys")]
    [Authorize(Policy = Policies.GetWeeklySurveysForUser)]
    public async Task<IActionResult> GetWeeklySurveysExport(Guid? userId)
    {
        if (userId is not null)
        {
            await _userInfoService.EnsureUserHasAccessToPatientInfoAsync();
        }
        else
        {
            userId = _userInfoService.GetUserInfoFromToken().UserId;
        }

        var excelData = await _excelExportService.ExportAllWeeklySurveysToExcelAsync(userId.Value);
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ЕженедельныеОпросы.xlsx");
    }
    
    [HttpGet("weekly-survey/{surveyId}")]
    [Authorize(Policy = Policies.GetWeeklySurveyById)]
    public async Task<IActionResult> GetWeeklySurveyExport(Guid surveyId)
    {
        var excelData = await _excelExportService.ExportWeeklySurveyToExcelAsync(surveyId);
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ЕженедельныйОпрос.xlsx");
    }
    
    [HttpGet("medical-history")]
    [Authorize(Policy = Policies.GetWeeklySurveyById)]
    public async Task<IActionResult> GetMedicalHistoryExport(Guid? userId)
    {
        if (userId is not null)
        {
            await _userInfoService.EnsureUserHasAccessToPatientInfoAsync();
        }
        else
        {
            userId = _userInfoService.GetUserInfoFromToken().UserId;
        }

        var excelData = await _excelExportService.ExportMedicalHistoryToExcelAsync(userId.Value);
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "МедицинскаяИстория.xlsx");
    }
    
    [HttpGet("observation-parameters")]
    [Authorize(Policy = Policies.GetDailySurveyById)]
    public async Task<IActionResult> GetObservationParametersExport(Guid? userId)
    {
        if (userId is not null)
        {
            await _userInfoService.EnsureUserHasAccessToPatientInfoAsync();
        }
        else
        {
            userId = _userInfoService.GetUserInfoFromToken().UserId;
        }

        var excelData = await _excelExportService.ExportObservationParametersStatisticsAsync(userId.Value);
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ПараметрыНаблюдения.xlsx");
    }
}