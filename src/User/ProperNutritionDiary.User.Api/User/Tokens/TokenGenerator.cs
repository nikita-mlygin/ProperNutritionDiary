using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ProperNutritionDiary.User.Api.User.Tokens;

public class TokenGenerator(IConfiguration cfg)
{
    public (string jwt, string rt) GenerateTokens(Guid id, string login, string role)
    {
        return (GenerateJwt(id.ToString(), login, role), GenerateRefreshToken());
    }

    public (Guid id, string login, string role)? ParseExpiredLogin(string jwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(
            jwt,
            CreateExpiredTokenVP(),
            out SecurityToken securityToken
        );

        if (
            securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
            )
        )
        {
            return null;
        }

        var strId = (principal.Identity as ClaimsIdentity)
            ?.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (strId is null)
            return null;

        Guid id = Guid.Parse(strId);

        string login = principal.Identity!.Name!;
        string role = (principal.Identity as ClaimsIdentity)!.FindFirst(ClaimTypes.Role)!.Value;

        return (id, login, role);
    }

    private string GenerateJwt(string id, string login, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var descriptor = CreateSecureTokenDescriptor(
            id,
            login,
            role,
            CreateExpiredAt(DateTime.UtcNow, cfg)
        );

        return tokenHandler.WriteToken(tokenHandler.CreateToken(descriptor));
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static SymmetricSecurityKey GenerateKey(IConfiguration cfg) =>
        new(
            Encoding.UTF8.GetBytes(
                cfg["Jwt:Key"]
                    ?? throw new InvalidConfigurationException("Cannot find the key for jwt")
            )
        );

    private static DateTime CreateExpiredAt(DateTime now, IConfiguration cfg)
    {
        return now.AddSeconds(
            double.Parse(
                cfg["Jwt:Expired"] ?? throw new InvalidConfigurationException("Cannot find expired")
            )
        );
    }

    private SecurityTokenDescriptor CreateSecureTokenDescriptor(
        string id,
        string name,
        string role,
        DateTime expiredAt
    ) =>
        new()
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, name),
                    new(ClaimTypes.NameIdentifier, id),
                    new(ClaimTypes.Role, role)
                ]
            ),
            Expires = expiredAt,
            SigningCredentials = new SigningCredentials(
                GenerateKey(cfg),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

    /// <summary>
    /// Get expired token validation parameters
    /// </summary>
    /// <returns></returns>
    private TokenValidationParameters CreateExpiredTokenVP() =>
        new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GenerateKey(cfg),
            ClockSkew = TimeSpan.Zero
        };
}
