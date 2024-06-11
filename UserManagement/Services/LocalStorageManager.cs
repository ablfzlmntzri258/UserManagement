using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using UserManagement.Shared.Interface;

namespace UserManagement.Services;

public class LocalStorageManager : ILocalStorageManager
{
    private readonly ProtectedLocalStorage _localStorageService;

    public LocalStorageManager(ProtectedLocalStorage localStorageService)
    {
        _localStorageService = localStorageService;
    }
    public async Task<string> GetItem(string key)
    {
        var result = await _localStorageService.GetAsync<string>(key);
        return result.Value ?? "";
    }

    public async Task RemoveItem(string key)
    {
        await _localStorageService.DeleteAsync(key);
    }

    public async Task SetItem(string key, string value)
    {
        await _localStorageService.SetAsync(key, value);
    }
}
