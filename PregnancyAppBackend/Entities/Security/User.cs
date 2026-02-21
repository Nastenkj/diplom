namespace PregnancyAppBackend.Entities.Security;

public class User : Entity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public List<Role> Roles { get; set; } = new();
    public List<UserRole> UserRoles { get; set; } = new();

    public List<DailySurvey> DailySurveys { get; set; } = new();

    public List<WeeklySurvey> WeeklySurveys { get; set; } = new();

    public DateTime? DateOfChangeOfAccessRights { get; set; }

    public string GetDateOfChangeOfAccessRightsTokenValue() => DateOfChangeOfAccessRights.HasValue
        ? DateOfChangeOfAccessRights.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")
        : string.Empty;
}