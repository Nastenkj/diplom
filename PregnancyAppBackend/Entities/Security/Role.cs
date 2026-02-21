namespace PregnancyAppBackend.Entities.Security;

public class Role : Entity
{
    public const string AdministratorName = "Администратор";
    public const string AdministratorId = "A987303B-BEB1-4ABE-8FCB-3E5C35D33C1C";

    public const string PatientName = "Пациент";
    public const string PatientId = "59A687D0-7D05-4988-B04D-0807C2A57E24";

    public const string DoctorName = "Врач";
    public const string DoctorId = "39B15202-0123-48E1-9BD4-7015D1DF09F9";

    public string Name { get; set; }
    public List<ApiClaim> ApiClaims { get; set; } = new();
    public List<RoleApiClaim> RoleApiClaims { get; set; } = new();

    public List<User> Users { get; set; } = new();
    public List<UserRole> UserRoles { get; set; } = new();
}