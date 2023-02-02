using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ShitheadApi;
using ShitheadApi.Models.Entities;
using ShitheadApi.Services;
using ShitheadApi.Settings;
using ShitheadApi.Utilities;

/// <summary>
/// Web application builder
/// </summary>
var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Services
/// </summary>
var services = builder.Services;

/// <summary>
/// The ShitHead Game cross policy
/// </summary>
var cross = "CrossPolicy";

/// <summary>
/// Configuration
/// </summary>
var configuration = builder.Configuration;

/// <summary>
/// Environment
/// </summary>
var environment = builder.Environment;

#region Services

// Database context
services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

// Mvc & ignore reference loop
services.AddMvc()
        .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

// Controllers
services.AddControllers();

// Identity
services.AddIdentity<User, Role>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1d);
    options.Lockout.MaxFailedAccessAttempts = 10;
}).AddEntityFrameworkStores<DatabaseContext>()
  .AddDefaultTokenProviders();

//JSON Web Tokens
services.Configure<Jwt>(configuration.GetSection("Jwt"));

var jwtSettings = configuration.GetSection("Jwt").Get<Jwt>();
services.AddAuth(jwtSettings);

//Cros
//Only the specified domain names can call up on the YasurfApi
services.AddCors(options =>
{
    options.AddPolicy(cross,
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

// Service declaration 
services.AddSingleton<IAnalyticsService, AnalyticsService>();
services.AddSingleton<ICryptoService, CryptoService>();
services.AddSingleton<IEmailService, EmailService>();
services.AddSingleton<IRequestService, RequestService>();

#endregion

#region App

/// <summary>
/// App builder
/// </summary>
var app = builder.Build();

if (environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(cross);

app.UseAuth();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

#endregion