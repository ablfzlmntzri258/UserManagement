using System.Text;
using System.Text.Json;

namespace UserManagement.Services;
public class HttpService
{
    private HttpClient _httpClient;
    private LocalStorageManager _localStorageManager;
    private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        
    }
    public void SetBaseAddress(string address)
    {
        _httpClient.BaseAddress = new Uri(address);
    }
    public void AddHttpHeader(string key, string value)
    {
        _httpClient.DefaultRequestHeaders.Add(key, value);
    }
    public void AddAuthorizationHeader(string? token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
    public async Task<T> GetAsync<T>(string resource)
    {
        try
        {
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
            var dataToPost = JsonSerializer.Serialize(payload);
            var content = new StringContent(dataToPost, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(new Uri(_httpClient.BaseAddress, uri), content);
            response.EnsureSuccessStatusCode();

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
            var dataToPost = JsonSerializer.Serialize(payload);
            var content = new StringContent(dataToPost, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(new Uri(_httpClient.BaseAddress, uri), content);

            response.EnsureSuccessStatusCode();

            string? jsonResponse = await response.Content.ReadAsStringAsync();

            var deserializedResponse = JsonSerializer.Deserialize<TResult>(jsonResponse, _serializerOptions);

            return deserializedResponse;
        }
        catch (Exception ex)
        {
            return default;
        }
    }
    public async Task<ApiResponse> DeleteAsync(string resource)
    {
        var response = await _httpClient.DeleteAsync(new Uri(_httpClient.BaseAddress, resource));
        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse { Success = false, ErrorMessage = response.ReasonPhrase };
        }
        string? jsonResponse = await response.Content.ReadAsStringAsync();

        var deserializedResponse = JsonSerializer.Deserialize<ApiResponse>(jsonResponse, _serializerOptions);

        return deserializedResponse;
    }
    private async Task<string> GetJsonAsync(string resource)
    {
        var result = await _httpClient.GetAsync(new Uri(_httpClient.BaseAddress, resource));
        result.EnsureSuccessStatusCode();
        var json = await result.Content.ReadAsStringAsync();
        return json;
    }
}
