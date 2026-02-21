using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Dtos.Web.MedicalHistory;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Services.MedicalHistoriesService;
using PregnancyAppBackend.Services.UserInfoService;

namespace PregnancyAppBackend.Controllers.Web;

public class MedicalHistoriesController : WebController
{
    private readonly IMedicalHistoriesService _medicalHistoriesService;
    private readonly IUserInfoService _userInfoService;

    public MedicalHistoriesController(IMedicalHistoriesService medicalHistoriesService,
                                      IUserInfoService userInfoService)
    {
        _medicalHistoriesService = medicalHistoriesService;
        _userInfoService = userInfoService;
    }

    [HttpPost]
    [Authorize(Policy = Policies.PostMedicalHistory)]
    public async Task<ActionResult<MedicalHistoryDto>> PostMedicalHistory(MedicalHistoryDto medicalHistoryDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(GetErrorListFromModelState(ModelState), "Произошла ошибка запроса, попробуйте позже.");
        }

        return await _medicalHistoriesService.AddMedicalHistoryAsync(medicalHistoryDto);
    }

    [HttpGet]
    [Authorize(Policy = Policies.GetMedicalHistory)]
    public async Task<ActionResult<MedicalHistoryDto?>> GetMedicalHistory(Guid? userId)
    {
        if (userId is not null)
        {
            await _userInfoService.EnsureUserHasAccessToPatientInfoAsync();
        }
        else
        {
            userId = _userInfoService.GetUserInfoFromToken().UserId;
        }
        
        return await _medicalHistoriesService.GetMedicalHistoryAsync(userId.Value);
    }
}