using Microsoft.IdentityModel.Tokens;
using PregnancyAppBackend.Entities.Security;
using PregnancyAppBackend.Infrastructure.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PregnancyAppBackend.Services.TokenService;

public class TokenService : ITokenService
{
    private readonly AuthConfiguration _authConfiguration;

    public TokenService(AuthConfiguration authConfiguration)
    {
        _authConfiguration = authConfiguration ?? throw new ArgumentNullException(nameof(authConfiguration));
    }

    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(Claims.DateOfChangeOfAccessRights, user.GetDateOfChangeOfAccessRightsTokenValue())
        };

        var userRoleApiClaims = user.Roles.SelectMany(r => r.ApiClaims)
                                    .Select(ac => ac.Type)
                                    .Distinct()
                                    .ToList();

        userRoleApiClaims.ForEach(urc => claims.Add(new Claim(urc, "true")));
        user.Roles.ForEach(r => claims.Add(new Claim(Claims.Roles, r.Name)));

        var securityKey = _authConfiguration.GetSymmetricSecurityKey();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_authConfiguration.Issuer,
                                         _authConfiguration.Audience,
                                         claims,
                                         expires: DateTime.UtcNow.AddDays(30),
                                         signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
