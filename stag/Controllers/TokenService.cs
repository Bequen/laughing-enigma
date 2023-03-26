using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Stag;

namespace stag.Controllers;

/* From: https://medium.com/geekculture/how-to-add-jwt-authentication-to-an-asp-net-core-api-84e469e9f019
 Really good explanation on how to do JWT token generating etc. with a little bit more control */
public class TokenService
{
    /// <summary>
    /// Creates new login token
    /// </summary>
    /// <param name="user">User to create token for</param>
    /// <returns></returns>
    public string CreateToken(IdentityUser user)
    {
        var expiration = DateTime.UtcNow.AddDays(30);
        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
        DateTime expiration) =>
        new(
            "apiWithAuthBackend",
            "apiWithAuthBackend",
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

    private List<Claim> CreateClaims(IdentityUser user)
    {
        try
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? String.Empty),
                new(ClaimTypes.Email, user.Email ?? String.Empty)
            };
            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    
    private SigningCredentials CreateSigningCredentials()
    {
        Config config = Config.Load();

        return new SigningCredentials(
            config.GetSecurityKey(),
            SecurityAlgorithms.HmacSha256
        );
    }

    /// <summary>
    /// Validate token. Can throw exception when token is invalid.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ClaimsPrincipal Validate(string token) {
        Config config = Config.Load();

        var tokenHandler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "apiWithAuthBackend",
            ValidAudience = "apiWithAuthBackend",
            IssuerSigningKey = config.GetSecurityKey()
        };

        SecurityToken validated;
        return tokenHandler.ValidateToken(token, parameters, out validated);
    }
}