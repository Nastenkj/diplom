using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Dtos.Web.Patients;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Services.PatientsService;

namespace PregnancyAppBackend.Controllers.Web;

public class PatientsController : WebController
{
    private readonly IPatientsService _patientsService;
    public PatientsController(IPatientsService patientsService)
    {
        _patientsService = patientsService;
    }
    
    [HttpGet]
    [Authorize(Policy = Policies.GetAllPatients)]
    public async Task<ActionResult<TableUsersDto>> GetAllPatients([FromQuery] TableUserRequestDto tableUserRequestDto)
    {
        return await _patientsService.GetPatientsAsync(tableUserRequestDto);
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy = Policies.GetPatient)]
    public async Task<ActionResult<UserDto>> GetPatient(Guid? id)
    {
        return await _patientsService.GetPatientAsync(id);
    }
    
    [HttpPut("{id}")]
    [Authorize(Policy = Policies.EditPatientInfo)]
    public async Task<ActionResult<UserDto>> EditPatientInfo(UserDto userEditDto)
    {
        return await _patientsService.EditPatientInfoAsync(userEditDto);
    }
}