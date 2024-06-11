using UserManagement.Shared.Interface;
using UserManagement.Shared.Models;

namespace UserManagement.Services;
public interface IApiService
{
    Task<List<User>> GetUsers();
    Task<string> GetUserFullName();
    Task<bool> RecycleServer();
}
public class ApiService : IApiService
{
    private readonly HttpService _httpService;
    private readonly IExternalAuthService _authService;

    public ApiService(
        HttpService httpService,
        IExternalAuthService authService)
    {
        _httpService = httpService;
        _authService = authService;
        _httpService.AddHttpHeader("X-APIKEY", "91FF625F-5227-48CD-9B09-DAB81BA25FAC");
    }
    public async Task<List<User>> GetUsers()
    {
        await AddBearerToken();

        var url = $"api/User/portal/0";
        var apiResponse = await _httpService.GetAsync<IEnumerable<User>>(url);
        return apiResponse.ToList();
    }
    public async Task<string> GetUserFullName()
    {
        var users = await GetUsers();
        var currentUserId = _authService.GetUserId();
        var user = users.Where(x => x.Id == currentUserId).FirstOrDefault();
        if (user is not null)
        {
            return $"{user.Name}";
        }
        return "";
    }
    public async Task<bool> RecycleServer()
    {
        return await _httpService.GetAsync<bool>($"api/RecycleServer");
    }
    protected async Task AddBearerToken()
    {
        var tokenTask = await _authService.GetAccessToken();
        _httpService.AddAuthorizationHeader(tokenTask);
    }
}
