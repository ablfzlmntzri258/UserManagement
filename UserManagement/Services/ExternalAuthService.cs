using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UserManagement.Shared.Classes;
using UserManagement.Shared.Interface;
using UserManagement.Shared.Models;
using UserManagement.Shared.ViewModel;

namespace UserManagement.Services;
public class ExternalAuthService : IExternalAuthService
{
    private readonly HttpService _httpService;
    private readonly ILocalStorageManager _localStorageManager;
    private readonly NavigationManager navigationManager;

    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public ExternalAuthService(HttpService httpService,
        ILocalStorageManager localStorage, NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor)
    {
        _httpService = httpService;
        _localStorageManager = localStorage;
        this.navigationManager = navigationManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public event Action<ClaimsPrincipal>? AuthenticationStateChanged;
    public ClaimsPrincipal CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            AuthenticationStateChanged?.Invoke(_currentUser);
        }
    }

    public async Task<bool> VerifyUser(int userId, string password)
    {
        var url = $"api/User/Verify?userId={userId}&password={password}";
        var isUserVerified = await _httpService.GetAsync<bool>(url);
        return isUserVerified;
    }
    public async Task<Tuple<bool, string>> Login(UserVM userAuthDto)
    {
        var responseUserAuthDto = await _httpService.PostAsync<UserVM, TokenModel>("api/auth/login", userAuthDto);
        if (responseUserAuthDto is not null)
        {
            await _localStorageManager.SetItem("LocalStorageToken", responseUserAuthDto.AccessToken);
            await _localStorageManager.SetItem("LocalStorageRefreshToken", responseUserAuthDto.RefreshToken);
            AuthenticateUser(responseUserAuthDto.AccessToken);
            Console.WriteLine(responseUserAuthDto.AccessToken);
            //_httpContextAccessor.HttpContext.Response.Redirect("~/logincallback");
            return new Tuple<bool, string>(true, responseUserAuthDto.AccessToken);
        }
        return new Tuple<bool, string>(false, "");
    }
    public async void AuthenticateUser(string jwt)
    {
        var identity = new ClaimsIdentity("Custom");
        var claims = JwtHelper.ParseClaimsFromJwt(jwt);
        identity.AddClaims(claims);
        CurrentUser = new ClaimsPrincipal(identity);
        //User user = new()
        //{
        //    Id = int.Parse(claims.FirstOrDefault(c => c.Type == "nameid").Value),
        //    UserName = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,
        //    Role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value
        //};
        //await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,CurrentUser);
    }
    public int GetUserId()
    {
        var nameIdentifier = CurrentUser.Claims
             .Where(x => x.Type == "nameid")
             .Select(x => x.Value)
             .FirstOrDefault();
        if (nameIdentifier is null)
        {
            return 0;
        }
        return int.Parse(nameIdentifier);
    }
    public async Task Logout()
    {
        await _localStorageManager.RemoveItem("LocalStorageToken");
        await _localStorageManager.RemoveItem("LocalStorageRefreshToken");
        CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());
    }
    public async Task<string?> GetAccessToken()
    {
        var token = await _localStorageManager.GetItem("LocalStorageToken");
        if (!string.IsNullOrEmpty(token) && !JwtHelper.IsTokenExpired(token))
        {
            return token;
        }
        else
        {
            return await RefreshTokens(token);
        }
    }
    public async Task TryToAuthenticateFromLocalStorage()
    {
        var token = await GetAccessToken();
        if (!string.IsNullOrEmpty(token))
        {
            AuthenticateUser(token);
        }
    }
    private async Task<string?> RefreshTokens(string token)
    {
        var refreshToken = await _localStorageManager.GetItem("LocalStorageRefreshToken");
        if (string.IsNullOrEmpty(refreshToken))
        {
            return null;
        }
        var tokenDto = new TokenModel
        {
            RefreshToken = refreshToken,
            AccessToken = token,
        };
        var userAuthDto = await _httpService.PostAsync<TokenModel, TokenModel>("api/auth/refresh-token", tokenDto);
        if (userAuthDto is not null)
        {
            await _localStorageManager.SetItem("LocalStorageToken", userAuthDto.AccessToken);
            await _localStorageManager.SetItem("LocalStorageRefreshToken", userAuthDto.RefreshToken);
            return userAuthDto.AccessToken;
        }
        return null;
    }

    public async Task LogOut()
    {
        await _localStorageManager.RemoveItem("LocalStorageToken");
        await _localStorageManager.RemoveItem("LocalStorageRefreshToken");
    }
}
