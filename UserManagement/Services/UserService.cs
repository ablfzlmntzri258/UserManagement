using static MudBlazor.Icons;
using System.Net.Http.Headers;
using System.Net.Http;
using UserManagement.Shared.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using UserManagement.Pages.Login;
using UserManagement.Shared.Interface;
using Newtonsoft.Json.Linq;


namespace UserManagement.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAll();
        // Task<User> GetById(int id);
        Task<HttpResponseMessage> Create(User user);
        Task<HttpResponseMessage> Update(User user);
        Task<bool> Delete(int id);
        Task<List<int>> CheckEmployeeCodes(List<int> emplyoeeCodes);
    }
    public class UserService: IUserService
    {
        private readonly HttpClient _HttpClient;
        private readonly ILocalStorageManager _localStorageManager;
        public UserService(HttpClient httpHttpClient, ILocalStorageManager localStorageManager)
        {
            _HttpClient = httpHttpClient;
            _HttpClient.BaseAddress = new Uri("http://localhost:5186");
            _localStorageManager = localStorageManager;
        }
        public async Task<List<User>> GetAll()
        {
            string token = await _localStorageManager.GetItem("LocalStorageToken");
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _HttpClient.GetAsync("/api/user/all");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<User>>(jsonString);
            }
            return null;
        }
        
        public async Task<HttpResponseMessage> Create(User user)
        {
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var jsonValue = JsonConvert.SerializeObject(user);
            var httpContent = new StringContent(jsonValue, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _HttpClient.PostAsync("/api/user/", httpContent);
            return response;
            // if (response.IsSuccessStatusCode)
            // {
            //     var jsonString = await response.Content.ReadAsStringAsync();
            //     return JsonConvert.DeserializeObject<User>(jsonString);
            // }
            // return null;
        }
        
        public async Task<HttpResponseMessage> Update(User user)
        {
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var jsonValue = JsonConvert.SerializeObject(user);
            var httpContent = new StringContent(jsonValue, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _HttpClient.PutAsync("/api/user/", httpContent);
            return response;
            // if (response.IsSuccessStatusCode)
            // {
            //     var jsonString = await response.Content.ReadAsStringAsync();
            //     return JsonConvert.DeserializeObject<User>(jsonString);
            // }
            // return null;
        }
        
        public async Task<bool> Delete(int id)
        {
            HttpResponseMessage response = await _HttpClient.DeleteAsync($"/api/user/{id}");
            return response.IsSuccessStatusCode;
        }


        public async Task<List<int>> CheckEmployeeCodes(List<int> emplyoeeCodes) {
            string token = await _localStorageManager.GetItem("LocalStorageToken");
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var jsonValue = JsonConvert.SerializeObject(emplyoeeCodes);
            Console.Write(jsonValue);
            var httpContent = new StringContent(jsonValue, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _HttpClient.PostAsync("/api/user/check-codes", httpContent);
            Console.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<int>>(jsonString);
            }
            return null;
        }

    }
}
