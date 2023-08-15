using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GoratLoans.Identity.Api;

internal static class TokenService
{
    public static string CreateToken(Identity identity, string signingToken)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, identity.Username)
        };
        if (identity.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingToken));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: cred);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}