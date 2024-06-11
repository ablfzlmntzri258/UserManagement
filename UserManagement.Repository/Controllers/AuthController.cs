using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Repository.Repositories;
using UserManagement.Shared.Models;
using UserManagement.Shared.ViewModel;

namespace UserManagement.Repository.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IUserRepository userRepository;
    private readonly ILogger<UserController> _logger;
    private IConfiguration _config;
    public AuthController(IUserRepository userRepository, ILogger<UserController> logger, IConfiguration config)
    {
        this.userRepository = userRepository;
        _logger = logger;
        _config = config;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserVM userCredetials)
    {
        var username = userCredetials.UserName;
        var password = userCredetials.Password;
        User user = userRepository.AuthenticateUser(username, password);

        if (user == null)
        {
            return Unauthorized(new TokenModel()
            {
                AccessToken = null,
                RefreshToken = null,
                IsAuthenticated = false
            });
        }
        else
        {
            var tokenString = GenerateJSONWebToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(int.Parse(_config["JWT:RefreshTokenValidityInDays"]));
            await userRepository.Update(user);
            _logger.LogInformation("User {UserName} logged in at {Time}.", 
                user.UserName, DateTime.Now);
            return Ok(new TokenModel()
            {
                AccessToken = tokenString,
                RefreshToken = refreshToken,
                IsAuthenticated = true
            });
        }
        
    }
    
    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel is null)
        {
            return BadRequest("Invalid client request");
        }

        string? accessToken = tokenModel.AccessToken;
        string? refreshToken = tokenModel.RefreshToken;
        
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return BadRequest("Invalid access token or refresh token");
        }
        
        var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        
        var user = userRepository.GetById(int.Parse(userId));

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Invalid access token or refresh token");
        }
        
        var newAccessToken = GenerateJSONWebToken(user);
        var newRefreshToken = GenerateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        await userRepository.Update(user);
        
        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken
        });
    }
    
    private string GenerateJSONWebToken(User userInfo)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.NameId, userInfo.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, userInfo.Name),
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserName),
            new Claim("EmployeeCode", userInfo.EmployeeCode.ToString()),
            new Claim(ClaimTypes.Role, userInfo.Permission.ToString() == "1" ? "admin" : userInfo.Permission.ToString() == "2" 
                ? "financial" : "employee")
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddMinutes(int.Parse(_config["JWT:TokenValidityInMinutes"])),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"])),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;

    }
    
    // [HttpGet("logout")]
    // public async Task LogOut()
    // {
    //     Console.WriteLine("LogOut");
    //     await HttpContext.SignOutAsync(
    //         CookieAuthenticationDefaults.AuthenticationScheme);
    // }
}