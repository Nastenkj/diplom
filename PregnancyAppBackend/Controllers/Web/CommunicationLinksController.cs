using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Dtos.Web.CommunicationLinks;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Services.CommunicationLinkService;

namespace PregnancyAppBackend.Controllers.Web;

public class CommunicationLinksController : WebController
{
    private readonly ICommunicationLinkService _communicationService;
    
    public CommunicationLinksController(ICommunicationLinkService communicationService)
    {
        _communicationService = communicationService;
    }
    
    [HttpPost]
    [Authorize(Policy = Policies.CreateCommunicationLink)]
    public async Task<ActionResult<PatientDoctorCommunicationLinkDto>> CreateCommunicationLink(CreateCommunicationLinkDto createDto)
    {
        return await _communicationService.CreateCommunicationLinkAsync(createDto);
    }
    
    [HttpGet]
    [Authorize(Policy = Policies.GetMyCommunicationLinks)]
    public async Task<ActionResult<List<PatientDoctorCommunicationLinkDto>>> GetMyCommunicationLinks()
    {
        return await _communicationService.GetUserCommunicationLinksAsync();
    }
}