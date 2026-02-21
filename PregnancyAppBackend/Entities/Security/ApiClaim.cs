namespace PregnancyAppBackend.Entities.Security;

public class ApiClaim : Entity
{
    public string Type { get; set; }
    public List<Role> Roles { get; set; } = new();
    public List<RoleApiClaim> RoleApiClaims { get; set; } = new();
}