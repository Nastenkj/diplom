namespace PregnancyAppBackend.Entities.Security;

public class RoleApiClaim
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public Guid ApiClaimId { get; set; }
    public ApiClaim ApiClaim { get; set; }
}