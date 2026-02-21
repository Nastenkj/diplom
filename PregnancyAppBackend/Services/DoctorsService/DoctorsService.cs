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

namespace PregnancyAppBackend.Services.DoctorsService;

public class DoctorsService : IDoctorsService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly IUserService _userService;
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<DoctorsService> _logger;

    public DoctorsService(IDatabaseContext databaseContext, IUserService userService, IUserInfoService userInfoService, ILogger<DoctorsService> logger)
    {
        _databaseContext = databaseContext;
        _userService = userService;
        _userInfoService = userInfoService;
        _logger = logger;
    }
    
    public async Task<User> CreateDoctorAsync(UserRegistrationRequestDto request)
    {
        var user = await _userService.CreateUserAsync(request.Email, request.Password);
        var role = await _databaseContext.Roles.SingleAsync(r => r.Name == Role.DoctorName);
        user.Roles = [role];

        var userCommonInfo = new UserCommonInfo
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            BirthDate = request.BirthDate,
            User = user,
            InsuranceNumber = string.Empty,
            TrustedPersonEmail = string.Empty,
            TrustedPersonFullName = string.Empty,
            TrustedPersonPhoneNumber = string.Empty,
        };

        _databaseContext.UserCommonInfos.Add(userCommonInfo);
        await _databaseContext.SaveChangesAsync();

        await SeedDefaultDataForDoctorInDbAsync(user.Id);

        return user;
    }

    public async Task<TableUsersDto> GetDoctorsAsync(TableUserRequestDto request)
    {
        var doctors = await _databaseContext.UserCommonInfos
                                            .AsNoTracking()
                                            .Include(pci => pci.User)
                                            .Skip((request.PageNumber - 1) * request.PageSize)
                                            .Take(request.PageSize)
                                            .Where(pci => pci.User.Roles.Any(r => r.Name == Role.DoctorName))
                                            .Where(pci => request.Email == null || pci.User.Email.Contains(request.Email))
                                            .Where(pci => request.Name == null || pci.FullName.Contains(request.Name))
                                            .Where(pci => request.PhoneNumber == null || pci.PhoneNumber.Contains(request.PhoneNumber))
                                            .ToListAsync();

        var total = await _databaseContext.UserCommonInfos
                                          .AsNoTracking()
                                          .Include(pci => pci.User)
                                          .Where(pci => pci.User.Roles.Any(r => r.Name == Role.DoctorName))
                                          .Where(pci => request.Email == null || pci.User.Email.Contains(request.Email))
                                          .Where(pci => request.Name == null || pci.FullName.Contains(request.Name))
                                          .Where(pci => request.PhoneNumber == null || pci.PhoneNumber.Contains(request.PhoneNumber))
                                          .CountAsync();
        
        return doctors.ConvertAll(EntityToDtoConverters.ConvertToTableUserDto).ConvertToTableUsersDto(total);
    }

    public async Task<UserDto> GetDoctorAsync(Guid? userId)
    {
        // admin request
        if (userId is not null)
        {
            await _userInfoService.EnsureUserIsAdminAsync();
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
                                   "Доктор не найден. Попробуйте позже.");
        }

        return patient.ConvertToUserDto();
    }

    public async Task<UserDto> EditDoctorInfoAsync(UserDto userEditDto)
    {
        _logger.LogInformation("Editing doctor info userEditInfo={@dto}", userEditDto);

        var doctor = await _databaseContext.UserCommonInfos
                                            .Include(pci => pci.User)
                                            .SingleOrDefaultAsync(pci => pci.UserId == userEditDto.Id);

        if (doctor is null)
        {
            throw new ApiException($"User with id={userEditDto.Id} not found", 
                                   "Доктор не найден. Попробуйте позже.");
        }
        
        doctor.FullName = userEditDto.FullName;
        doctor.PhoneNumber = userEditDto.PhoneNumber;
        doctor.BirthDate = userEditDto.BirthDate;

        var emailPopulated = await _databaseContext.Users.AnyAsync(u => u.Email == userEditDto.Email && u.Id != userEditDto.Id);

        if (emailPopulated)
        {
            throw new ApiException($"Email {userEditDto.Email} already in use.", 
                                   "Указанный email уже используется.");
        }

        doctor.User.Email = userEditDto.Email;
    
        await _databaseContext.SaveChangesAsync();
        
        return doctor.ConvertToUserDto();
    }

    private async Task SeedDefaultDataForDoctorInDbAsync(Guid userId)
    {
        var parameterNorms = await _databaseContext.Parameters.ToListAsync();

        foreach (var parameter in parameterNorms)
        {
            var norm = new ObservationParameterNorm
            {
                Id = Guid.NewGuid(),
                ParameterId = parameter.Id,
                LowerBound = parameter.DefaultLowerBoundValue,
                UpperBound = parameter.DefaultUpperBoundValue,
                UserId = userId
            };
            
            await _databaseContext.ObservationParameterNorms.AddAsync(norm);
        }
        
        await _databaseContext.SaveChangesAsync();
    }
}