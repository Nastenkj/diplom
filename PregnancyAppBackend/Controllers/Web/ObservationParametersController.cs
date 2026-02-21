using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Dtos.Web.ObservationParameterNorm;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Services.ObservationParameterNormService;
using PregnancyAppBackend.Services.UserInfoService;

namespace PregnancyAppBackend.Controllers.Web;

public class ObservationParametersController : WebController
{
    private readonly IObservationParameterNormService _observationParameterNormService;
    private readonly IUserInfoService _userInfoService;

    public ObservationParametersController(IObservationParameterNormService observationParameterNormService,
                                           IUserInfoService userInfoService)
    {
        _observationParameterNormService = observationParameterNormService;
        _userInfoService = userInfoService;
    }

    [HttpPost]
    [Authorize(Policy = Policies.EditDoctorInfo)]
    public async Task<ObservationParameterNormDto> UpdateObservationParameterNormBounds(Guid? userId, 
                                                                                        decimal lowerBound, 
                                                                                        decimal upperBound, 
                                                                                        string parameterName)
    {
        if (userId is not null)
        {
            await _userInfoService.EnsureUserIsAdminAsync();
        }
        else
        {
            userId = _userInfoService.GetUserInfoFromToken().UserId;
        }

        return await _observationParameterNormService.UpdateObservationParameterNormBoundsAsync(userId.Value,
                                                                                                lowerBound,
                                                                                                upperBound,
                                                                                                parameterName);
    }

    [HttpGet]
    [Authorize(Policy = Policies.EditDoctorInfo)]
    public async Task<List<ObservationParameterNormDto>> GetObservationParameterNorms(Guid? userId)
    {
        if (userId is not null)
        {
            await _userInfoService.EnsureUserIsAdminAsync();
        }
        else
        {
            userId = _userInfoService.GetUserInfoFromToken().UserId;
        }

        return await _observationParameterNormService.GetObservationParameterNormsAsync(userId.Value);
    }
}