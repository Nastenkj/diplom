using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Dtos.Authentication;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.AuthenticationService;
using PregnancyAppBackend.Services.DoctorsService;
using PregnancyAppBackend.Services.PatientsService;
using PregnancyAppBackend.Services.TokenService;
using PregnancyAppBackend.Services.UserInfoService;
using PregnancyAppBackend.Services.UserService;

namespace PregnancyAppBackend.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[Authorize]
public class AuthenticationController : WebController
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IDatabaseContext _databaseContext;
    private readonly IPatientsService _patientsService;
    private readonly IDoctorsService _doctorsService;
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public AuthenticationController(ITokenService tokenService,
                                    IAuthenticationService authenticationService,
                                    IUserService userService,
                                    IDatabaseContext databaseContext,
                                    IPatientsService patientsService,
                                    IDoctorsService doctorsService,
                                    IUserInfoService userInfoService,
                                    ILogger<AuthenticationController> logger)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _databaseContext = databaseContext;
        _patientsService = patientsService;
        _doctorsService = doctorsService;
        _userInfoService = userInfoService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponseDto>> Login([FromBody] AuthenticationRequestDto request)
    {
        if (request is null)
        {
            throw new ApiException("Request body is null", "Тело запроса не может быть пустым. Необходимо добавить данные.");
        }

        var user = await _authenticationService.AuthenticateAsync(request.Email, request.Password);
        var token = _tokenService.CreateToken(user);
        return Ok(new AuthenticationResponseDto
        {
            JwtToken = token,
            UserId = user.Id,
            UserEmail = user.Email
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponseDto>> RegisterPatient([FromBody] PatientRegistrationRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(GetErrorListFromModelState(ModelState), "Произошла ошибка запроса, попробуйте позже.");
        }
        
        _logger.LogInformation("Registering patient. Register request={@dto}", request);

        await _userService.EnsureUniqueUserAsync(request.Email, request.PhoneNumber);

        var user = await _patientsService.CreatePatientAsync(request);
        var token = _tokenService.CreateToken(user);

        return Ok(new AuthenticationResponseDto
        {
            UserId = user.Id,
            UserEmail = user.Email,
            JwtToken = token
        });
    }
    
    [HttpPost("register-doctor")]
    [Authorize]
    public async Task<ActionResult<AuthenticationResponseDto>> RegisterDoctor([FromBody] UserRegistrationRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(GetErrorListFromModelState(ModelState), "Произошла ошибка запроса, попробуйте позже.");
        }
        
        _logger.LogInformation("Registering doctor. Register request={@dto}", request);
        
        await _userInfoService.EnsureUserIsAdminAsync();

        await _userService.EnsureUniqueUserAsync(request.Email, request.PhoneNumber);

        var user = await _doctorsService.CreateDoctorAsync(request);
        var token = _tokenService.CreateToken(user);

        return Ok(new AuthenticationResponseDto
        {
            UserId = user.Id,
            UserEmail = user.Email,
            JwtToken = token
        });
    }

    [HttpGet("check")]
    [Authorize]
    public IActionResult CheckAuthentication() => Ok();

    [HttpGet("users")]
    [Authorize(Policy = Policies.Test)]
    public async Task<ActionResult<List<string>>> Test()
    {
        var res = await _databaseContext.Users.Select(u => u.Email).ToListAsync();
        return Ok(res);
    }
}