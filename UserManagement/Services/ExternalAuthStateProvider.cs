using Microsoft.AspNetCore.Components.Authorization;
using UserManagement.Shared.Interface;

namespace UserManagement.Services;
public class ExternalAuthStateProvider : AuthenticationStateProvider
{
    private readonly IExternalAuthService _externalAuthService;

    public ExternalAuthStateProvider(IExternalAuthService externalAuthService)
    {
        externalAuthService.AuthenticationStateChanged += (x) =>
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(x)));
        };
        _externalAuthService = externalAuthService;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await _externalAuthService.TryToAuthenticateFromLocalStorage();
        return new AuthenticationState(_externalAuthService.CurrentUser);
    }
}
