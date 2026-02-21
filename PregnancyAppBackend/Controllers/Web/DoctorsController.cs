using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PregnancyAppBackend.Dtos.Web.Patients;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Services.DoctorsService;

namespace PregnancyAppBackend.Controllers.Web;

public class DoctorsController : WebController
{
    private readonly IDoctorsService _doctorsService;

    public DoctorsController(IDoctorsService doctorsService)
    {
        _doctorsService = doctorsService;
    }
    
    [HttpGet]
    [Authorize(Policy = Policies.GetAllDoctors)]
    public async Task<ActionResult<TableUsersDto>> GetAllDoctors([FromQuery] TableUserRequestDto doctorsRequestDto)
    {
        return await _doctorsService.GetDoctorsAsync(doctorsRequestDto);
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy = Policies.GetDoctor)]
    public async Task<ActionResult<UserDto>> GetDoctor(Guid? id)
    {
        return await _doctorsService.GetDoctorAsync(id);
    }
    
    [HttpPut("{id}")]
    [Authorize(Policy = Policies.EditDoctorInfo)]
    public async Task<ActionResult<UserDto>> EditDoctorInfo(UserDto doctorEditDto)
    {
        return await _doctorsService.EditDoctorInfoAsync(doctorEditDto);
    }
}