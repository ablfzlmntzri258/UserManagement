using System.Security.Claims;
using UserManagement.Shared.Models;
using UserManagement.Shared.ViewModel;

namespace UserManagement.Shared.Interface;
public interface IExternalAuthService
{
 
    // Task<Tuple<bool, string>> Login(UserVM userAuthDto);
    // Task LogOut();
    Task<ClaimsPrincipal?> UserAuthenticated();
}