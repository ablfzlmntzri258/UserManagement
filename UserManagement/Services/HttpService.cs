using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using UserManagement.Shared.Interface;
using UserManagement.Shared.Models;

namespace UserManagement.Services;
public class HttpService
{
    private HttpClient _httpClient;
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    //private LocalStorageManager _localStorageManager;
    private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly IExternalAuthService _authService;
    public HttpService(HttpClient httpClient, IExternalAuthService authService, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _authService = authService;
    }
    public void SetBaseAddress(string address)
    {
        _httpClient.BaseAddress = new Uri(address);
    }
    public void AddHttpHeader(string key, string value)
    {
        _httpClient.DefaultRequestHeaders.Add(key, value);
    }
    public async Task AddAuthorizationHeader()
    {
        var token = await GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
    public async Task<string> GetToken()
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var user = await _authService.UserAuthenticated();

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                user.Claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_config["JWT:TokenValidityInMinutes"])),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch
        {
            return null;
        }
    }
   
    public async Task<T> GetAsync<T>(string resource)
    {
        try
        {
            await AddAuthorizationHeader();
            var json = await GetJsonAsync(resource);
            var response = JsonSerializer.Deserialize<T>(json, _serializerOptions);
            return response;
        }
        catch (Exception ex)
        {
            return default;
        }
    }
    public async Task<bool> PostAsync<TRequest>(string uri, TRequest payload)
    {
        await AddAuthorizationHeader();
        var dataToPost = JsonSerializer.Serialize(payload);
        var content = new StringContent(dataToPost, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(new Uri(_httpClient.BaseAddress, uri), content);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest payload)
    {
        try
        {
            await AddAuthorizationHeader();
            var dataToPost = JsonSerializer.Serialize(payload);
            var content = new StringContent(dataToPost, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, content);
            // response.EnsureSuccessStatusCode();

            string? jsonResponse = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonSerializer.Deserialize<TResult>(jsonResponse, _serializerOptions);

            return deserializedResponse;
        }
        catch (Exception ex)
        {
            return default;
        }
    }
    public async Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest payload)
    {
        try
        {
            await AddAuthorizationHeader();
            var dataToPost = JsonSerializer.Serialize(payload);
            var content = new StringContent(dataToPost, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(uri, content);
            Console.WriteLine(response);
            // response.EnsureSuccessStatusCode();

            string? jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonResponse);
            var deserializedResponse = JsonSerializer.Deserialize<TResult>(jsonResponse, _serializerOptions);

            return deserializedResponse;
        }
        catch (Exception ex)
        {
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string resource)
    {
        await AddAuthorizationHeader();
        var response = await _httpClient.DeleteAsync(new Uri(_httpClient.BaseAddress, resource));
        return response.IsSuccessStatusCode;
    }

    private async Task<string> GetJsonAsync(string resource)
    {
        var result = await _httpClient.GetAsync(new Uri(_httpClient.BaseAddress, resource));
        result.EnsureSuccessStatusCode();
        var json = await result.Content.ReadAsStringAsync();
        return json;
    }
}
