using System.Text;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Interfaces;
using AntiScam.Infrastructure.BackgroundServices;
using AntiScam.Infrastructure.Data;
using AntiScam.Infrastructure.Data.Repositories;
using AntiScam.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AntiScam.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. DbContext & MySQL
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=localhost;Port=3306;Database=antiscam_db;User=root;Password=root_password;";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mysqlOptions =>
            {
                mysqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
            });
        });

        // 2. Identity
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // 3. JWT Authentication
        var secret = configuration["Jwt:Secret"] ?? "AntiScamSuperSecretKey2026!MustBeAtLeast32BytesLong!";
        var issuer = configuration["Jwt:Issuer"] ?? "AntiScam";
        var audience = configuration["Jwt:Audience"] ?? "AntiScam";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // 4. Repositories & Unit of Work
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 5. Domain & Infrastructure Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITelegramService, TelegramService>();
        services.AddScoped<IRiskScoreCalculator, RiskScoreCalculator>();
        services.AddScoped<IRouteDeviationDetector, RouteDeviationDetector>();
        services.AddScoped<IReportExportService, ReportExportService>();

        // 6. ML Service HTTP Client
        services.AddHttpClient<IMlClassificationService, MlClassificationService>(client =>
        {
            var mlBaseUrl = configuration["MlService:BaseUrl"] ?? "http://localhost:8001";
            client.BaseAddress = new Uri(mlBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // 7. Background Hosted Services
        services.AddSingleton<AlertCountdownService>();
        services.AddSingleton<IAlertCountdownService>(sp => sp.GetRequiredService<AlertCountdownService>());
        services.AddHostedService(sp => sp.GetRequiredService<AlertCountdownService>());
        services.AddHostedService<LocationCleanupService>();

        return services;
    }
}
