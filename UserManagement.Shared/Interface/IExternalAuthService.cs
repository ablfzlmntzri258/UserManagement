using System.Security.Claims;
using UserManagement.Shared.Models;
using UserManagement.Shared.ViewModel;

namespace UserManagement.Shared.Interface;
public interface IExternalAuthService
{
    ClaimsPrincipal CurrentUser { get; set; }

    event Action<ClaimsPrincipal>? AuthenticationStateChanged;

    void AuthenticateUser(string jwt);
    Task TryToAuthenticateFromLocalStorage();
    Task<string?> GetAccessToken();
    Task<Tuple<bool, string>> Login(UserVM userAuthDto);
    Task Logout();
    Task<bool> VerifyUser(int userId, string password);
    int GetUserId();
    Task LogOut();
}