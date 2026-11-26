using AntiScam.API.Extensions;
using AntiScam.API.Middleware;
using AntiScam.Application.Extensions;
using AntiScam.Domain.Entities;
using AntiScam.Infrastructure.Data;
using AntiScam.Infrastructure.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Validate critical config early — fail fast ────────────────────────────
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Secret is missing or too short (must be at least 32 characters). " +
        "Set the JWT_SECRET environment variable or update appsettings.");
}

// ── 2. Serilog ───────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/antiscam-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ── 3. Add Layers ────────────────────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// ── 4. Web API, CORS, Swagger ────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:3000", "http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ── 5. Health Checks ─────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"));

// ── 6. Build App ─────────────────────────────────────────────────────────────
var app = builder.Build();

// ── 7. Seed Database (Migrations + Roles + Admin) ────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context      = services.GetRequiredService<ApplicationDbContext>();
        var userManager  = services.GetRequiredService<UserManager<User>>();
        var roleManager  = services.GetRequiredService<RoleManager<IdentityRole>>();
        var config       = services.GetRequiredService<IConfiguration>();
        var seedLogger   = services.GetRequiredService<ILogger<Program>>();

        await DbInitializer.SeedAsync(context, userManager, roleManager, config, seedLogger);
    }
    catch (Exception ex)
    {
        Log.Warning("Database seeding failed: {Message}", ex.Message);
    }
}

// ── 8. Middleware Pipeline ────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<AuditLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AntiScam API v1");
    c.RoutePrefix = "swagger";
});

app.UseSerilogRequestLogging();
app.UseCors("DefaultPolicy");

app.UseAuthentication();
app.UseAuthorization();

// ── 9. Health Check Endpoint ─────────────────────────────────────────────────
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status  = report.Status.ToString(),
            checks  = report.Entries.Select(e => new
            {
                name    = e.Key,
                status  = e.Value.Status.ToString(),
                message = e.Value.Description
            }),
            duration = report.TotalDuration
        });
        await context.Response.WriteAsync(result);
    }
});

app.MapControllers();

app.Run();
