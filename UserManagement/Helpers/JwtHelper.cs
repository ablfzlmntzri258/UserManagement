using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserManagement.Shared.Classes;
public class JwtHelper
{
    public static string GetJwtTokenString(string uniqueKey,
                                     string issuer,
                                     string audience,
                                     Claim[] claims,
                                     TimeSpan expiration)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(uniqueKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(expiration),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = creds,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        return jwtToken;
    }
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(jwt).Claims;
        }
        catch 
        {
            return null;
        }
    }
    public static bool IsTokenExpired(string token)
    {
        var expiration = GetExpirationDateFromJwt(token);
        var currentTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        return currentTime > expiration;
    }
    public static bool IsTokenExpired(long expiration)
    {
        var currentTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        return currentTime > expiration;
    }
    public static long GetExpirationDateFromJwt(string token)
    {
        var timeStamp = GetValueFromJwt(token, "exp");
        var expirationTime = long.Parse(timeStamp!);
        return expirationTime;
    }
    public static string? GetValueFromJwt(string token, string value)
    {
        var claims = ParseClaimsFromJwt(token);
        var extractedValue = claims.FirstOrDefault(x => x.Type == value)?.Value;
        return extractedValue;
    }
}
