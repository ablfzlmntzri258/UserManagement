using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using UserManagement.Shared.Classes;
using UserManagement.Shared.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace UserManagement.Pages
{
    public class LoginCallbackModel : PageModel
    {
        private IConfiguration _config;
        public LoginCallbackModel(IConfiguration config)
        {
            _config = config;
        }

        public async Task OnGetAsync(string token, string redirectBackTo)
        {
            try
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
                    Response.Redirect("/login");
                if (JwtHelper.IsTokenExpired(token))
                {
                    Response.Redirect("/login");
                }
                var userClaims = JwtHelper.ParseClaimsFromJwt(token);
                var claimsIdentity = new ClaimsIdentity(
                                        userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { };
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties).GetAwaiter().GetResult();
                Response.Redirect("/dashboard");
            }
            catch (Exception ex)
            {
                Response.Redirect("/login");
            }
        }


    }
}
