using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using UserManagement.Helpers;
using UserManagement.Services;
using UserManagement.Shared.Interface;
using Blazored.LocalStorage;
using MudBlazor;
using MudBlazor.Services;
using System.Net;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using UserManagement.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagement.Shared;
using static MudBlazor.CategoryTypes;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
// Add services to the container.
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<AuthenticationService, AuthenticationService>();
builder.Services.AddMudServices();

builder.Services.AddHttpClient<HttpService>(options =>
{;
    options.BaseAddress = new Uri("http://localhost:5186");
});

//builder.Services.AddScoped<AuthenticationStateProvider, ExternalAuthStateProvider>();
//builder.Services.AddScoped<ExternalAuthStateProvider>();
builder.Services.AddScoped<HttpContextAccessor>();
builder.Services.AddScoped<IExternalAuthService, ExternalAuthService>();
//builder.Services.AddScoped<ILocalStorageManager, LocalStorageManager>();
builder.Services.AddScoped<PDFHelper>();
builder.Services.AddScoped<FileHelper>();
builder.Services.AddScoped<HashHelper>();
builder.Services.AddScoped<DateHelper>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddBlazoredLocalStorage();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(configureOptions =>
    {
        configureOptions.Cookie.Name = "AspCookie";
        configureOptions.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        configureOptions.ExpireTimeSpan = TimeSpan.FromMinutes(120);
        configureOptions.SlidingExpiration = false;
    });


builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    //
    // config.SnackbarConfiguration.PreventDuplicates = false;
    // config.SnackbarConfiguration.NewestOnTop = false;
    // config.SnackbarConfiguration.ShowCloseIcon = true;
    // config.SnackbarConfiguration.VisibleStateDuration = 10000;
    // config.SnackbarConfiguration.HideTransitionDuration = 500;
    // config.SnackbarConfiguration.ShowTransitionDuration = 500;
    // config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


//app.UseStaticFilesAuthorization();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = Constants.PublicFolderAddress,
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, Constants.ProjectFolderAddress)),
    OnPrepareResponse = ctx =>
    {
        if (!ctx.Context.User.Identity.IsAuthenticated)
        {
            ctx.Context.Response.ContentLength = 0;
            ctx.Context.Response.Body = Stream.Null;
            ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            ctx.Context.Response.Redirect("/");
        }

        if (!ctx.Context.User.IsInRole("admin") && !ctx.Context.User.IsInRole("financial")) {
            string[] splittedPath = ctx.Context.Request.Path.ToString().Split("/");
            int index = Array.IndexOf(splittedPath, "f");
            if (index != -1 && index + 1 < splittedPath.Length)
            {
                string urlEmployeeCode = splittedPath[index + 1];
                string userEmployeeCode = ctx.Context.User.Claims.FirstOrDefault(c => c.Type == "EmployeeCode").Value;
                if (userEmployeeCode != urlEmployeeCode) {
                    ctx.Context.Response.ContentLength = 0;
                    ctx.Context.Response.Body = Stream.Null;
                    ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    ctx.Context.Response.Redirect("/");
                }
            }
        }
    }
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
    endpoints.MapRazorPages();
});


app.Run();