using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Filters;
using UserManagement.Repository.Contexts;
using UserManagement.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     })
//     .AddCookie(options =>
//         {
//             options.Cookie.SameSite = SameSiteMode.Lax;
//             options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
//             options.SlidingExpiration = false;
//             options.Events.OnRedirectToLogin = context =>
//             {
//                 context.Response.StatusCode = 401;
//                 return Task.CompletedTask;
//             };
//         }
//         );
//
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: "EnableCORS",
//         policy  =>
//         {
//             policy.WithOrigins("http://localhost:*")
//                 .AllowAnyHeader()
//                 .AllowAnyMethod();;
//         });
// });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
// Configure the HTTP request pipeline.
builder.Services.AddScoped<IUserRepository,UserRepository>();

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
    .Filter.ByExcluding(Matching.FromSource("System"))
    .Filter.ByExcluding(Matching.FromSource("Microsoft"))
    .CreateLogger();
builder.Services.AddSerilog();
builder.Services.AddHttpContextAccessor();


builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();