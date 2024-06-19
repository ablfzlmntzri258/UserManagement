using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using UserManagement.Shared.Classes;
using UserManagement.Shared.Interface;
using UserManagement.Shared.Models;
using UserManagement.Shared.ViewModel;

namespace UserManagement.Services;
public class ExternalAuthService : IExternalAuthService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    public ExternalAuthService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }
    
    public async Task<ClaimsPrincipal?> UserAuthenticated()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        Console.WriteLine(user.Identity.IsAuthenticated);
        if (user.Identity.IsAuthenticated)
        {
            return user;
        }
        else
        {
            return null;
        }
    }
    
}
