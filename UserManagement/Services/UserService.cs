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
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using UserManagement.Shared.ViewModel;

namespace UserManagement.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAll();
        // Task<User> GetById(int id);
        Task<ApiResponse<User>> Create(User user);
        Task<ApiResponse<User>> Update(User user);
        Task<bool> Delete(int id);
        Task<List<int>> CheckEmployeeCodes(List<int> emplyoeeCodes);
        Task<Tuple<bool, string>> Login(UserVM userAuthDto);
        Task LogOut();


    }
    public class UserService : IUserService
    {
        private readonly HttpService _httpService;
        private readonly NavigationManager _navigationManager;

        public UserService(
            HttpService httpService, NavigationManager navigationManager)
        {
            _httpService = httpService;
            _navigationManager = navigationManager;

        }
        public async Task<List<User>> GetAll()
        {
            return await _httpService.GetAsync<List<User>>("/api/user/all");
        }

        public async Task<ApiResponse<User>> Create(User user)
        {
            return await _httpService.PostAsync<User,ApiResponse<User>>("/api/user/", user);
        }

        public async Task<ApiResponse<User>> Update(User user)
        {
            return await _httpService.PutAsync<User, ApiResponse<User>>("/api/user/", user);
        }

        public async Task<bool> Delete(int id)
        {
            return await _httpService.DeleteAsync($"/api/user/{id}");
        }


        public async Task<List<int>> CheckEmployeeCodes(List<int> emplyoeeCodes)
        {
            try
            {
                return await _httpService.PostAsync<List<int>, List<int>>("/api/user/check-codes", emplyoeeCodes);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public async Task<Tuple<bool, string>> Login(UserVM userAuthDto)
        {
            var responseUserAuthDto = await _httpService.PostAsync<UserVM, TokenModel>("api/auth/login", userAuthDto);
            if (responseUserAuthDto is not null)
            {
                //AuthenticateUser(responseUserAuthDto.AccessToken);
                return new Tuple<bool, string>(true, responseUserAuthDto.AccessToken);
            }
            return new Tuple<bool, string>(false, "");
        }
        

        public async Task LogOut()
        {
            _navigationManager.NavigateTo("/logoutcallback");
        }

    }
}
