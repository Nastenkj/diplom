using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Converters;
using PregnancyAppBackend.Dtos.Authentication;
using PregnancyAppBackend.Dtos.Web.Patients;
using PregnancyAppBackend.Entities;
using PregnancyAppBackend.Entities.Security;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Persistance;
using PregnancyAppBackend.Services.UserInfoService;
using PregnancyAppBackend.Services.UserService;

namespace PregnancyAppBackend.Services.PatientsService;

public class PatientsService : IPatientsService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly IUserService _userService;
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<PatientsService> _logger;

    public PatientsService(IDatabaseContext databaseContext, IUserService userService, IUserInfoService userInfoService, ILogger<PatientsService> logger)
    {
        _databaseContext = databaseContext;
        _userService = userService;
        _userInfoService = userInfoService;
        _logger = logger;
    }

    public async Task<User> CreatePatientAsync(PatientRegistrationRequestDto request)
    {
        var user = await _userService.CreateUserAsync(request.Email, request.Password);
        var role = await _databaseContext.Roles.SingleAsync(r => r.Name == Role.PatientName);
        user.Roles = [role];

        var userCommonInfo = new UserCommonInfo
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            TrustedPersonEmail = request.TrustedPersonEmail,
            InsuranceNumber = request.TrustedPersonEmail,
            TrustedPersonFullName = request.TrustedPersonFullName,
            TrustedPersonPhoneNumber = request.TrustedPersonPhoneNumber,
            BirthDate = request.BirthDate,
            User = user
        };

        _databaseContext.UserCommonInfos.Add(userCommonInfo);
        await _databaseContext.SaveChangesAsync();

        return user;
    }

    public async Task<TableUsersDto> GetPatientsAsync(TableUserRequestDto request)
    {
        var patients = await _databaseContext.UserCommonInfos
                                             .AsNoTracking()
                                             .Include(pci => pci.User)
                                             .Skip((request.PageNumber - 1) * request.PageSize)
                                             .Take(request.PageSize)
                                             .Where(pci => pci.User.Roles.Any(r => r.Name == Role.PatientName))
                                             .Where(pci => request.Email == null || pci.User.Email.Contains(request.Email))
                                             .Where(pci => request.Name == null || pci.FullName.Contains(request.Name))
                                             .Where(pci => request.PhoneNumber == null || pci.PhoneNumber.Contains(request.PhoneNumber))
                                             .ToListAsync();

        var total = await _databaseContext.UserCommonInfos
                                          .AsNoTracking()
                                          .Where(pci => pci.User.Roles.Any(r => r.Name == Role.PatientName))
                                          .Where(pci => request.Email == null || pci.User.Email.Contains(request.Email))
                                          .Where(pci => request.Name == null || pci.FullName.Contains(request.Name))
                                          .Where(pci => request.PhoneNumber == null || pci.PhoneNumber.Contains(request.PhoneNumber))
                                          .CountAsync();

        return patients.ConvertAll(EntityToDtoConverters.ConvertToTableUserDto).ConvertToTableUsersDto(total);
    }

    public async Task<UserDto> GetPatientAsync(Guid? userId)
    {
        // admin/doctor request
        if (userId is not null)
        {
            await _userInfoService.EnsureUserHasAccessToPatientInfoAsync();
        }
        else
        {
            var userInfo = _userInfoService.GetUserInfoFromToken();
            userId ??= userInfo.UserId;    
        }
        
        var patient = await _databaseContext.UserCommonInfos
                                            .AsNoTracking()
                                            .Include(pci => pci.User)
                                            .SingleOrDefaultAsync(pci => pci.UserId == userId);
        if (patient is null)
        {
            throw new ApiException($"User with id={userId} not found", 
                                   "Пациент не найден. Попробуйте позже.");
        }

        return patient.ConvertToUserDto();
    }

    public async Task<UserDto> EditPatientInfoAsync(UserDto userEditDto)
    {
        _logger.LogInformation("Editing patient info userEditInfo={@dto}", userEditDto);
        
        var patient = await _databaseContext.UserCommonInfos
                                            .Include(pci => pci.User)
                                            .SingleOrDefaultAsync(pci => pci.UserId == userEditDto.Id);

        if (patient is null)
        {
            throw new ApiException($"User with id={userEditDto.Id} not found", 
                                   "Пациент не найден. Попробуйте позже.");
        }
        
        patient.FullName = userEditDto.FullName;
        patient.PhoneNumber = userEditDto.PhoneNumber;
        patient.TrustedPersonFullName = userEditDto.TrustedPersonFullName;
        patient.TrustedPersonPhoneNumber = userEditDto.TrustedPersonPhoneNumber;
        patient.TrustedPersonEmail = userEditDto.TrustedPersonEmail;
        patient.InsuranceNumber = userEditDto.InsuranceNumber;
        patient.BirthDate = userEditDto.BirthDate;

        var emailPopulated = await _databaseContext.Users.AnyAsync(u => u.Email == userEditDto.Email && u.Id != userEditDto.Id);

        if (emailPopulated)
        {
            throw new ApiException($"Email {userEditDto.Email} already in use.", 
                                   "Указанный email уже используется.");
        }

        patient.User.Email = userEditDto.Email;
    
        await _databaseContext.SaveChangesAsync();
        
        return patient.ConvertToUserDto();
    }
}