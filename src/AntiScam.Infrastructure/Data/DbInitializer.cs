using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AntiScam.Infrastructure.Data;

/// <summary>
/// Seeds the database with required roles and a default admin account on first run.
/// Safe to call on every startup — all operations are idempotent.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Applies pending EF migrations, creates roles, and seeds the default admin account.
    /// </summary>
    public static async Task SeedAsync(
        ApplicationDbContext      db,
        UserManager<User>         userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration            configuration,
        ILogger                   logger)
    {
        // ── 1. Apply pending EF migrations ───────────────────────────────────
        try
        {
            var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
            if (pending.Count > 0)
            {
                logger.LogInformation("Applying {Count} pending migration(s): {Names}",
                    pending.Count, string.Join(", ", pending));
                await db.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Database schema is up to date — no migrations needed.");
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not apply migrations: {Message}. " +
                "The database may not exist yet or the connection is unavailable.", ex.Message);
        }

        // ── 2. Seed Identity Roles ────────────────────────────────────────────
        string[] roles = [nameof(UserRole.User), nameof(UserRole.Admin)];
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    logger.LogInformation("Role '{Role}' created.", roleName);
                else
                    logger.LogWarning("Failed to create role '{Role}': {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── 3. Seed default Admin account ─────────────────────────────────────
        // Credentials can be overridden via config — fallback to safe defaults
        var adminEmail    = configuration["Seed:AdminEmail"]    ?? "admin@antiscam.local";
        var adminPassword = configuration["Seed:AdminPassword"] ?? "Admin@123456";
        var adminFullName = configuration["Seed:AdminFullName"] ?? "Quản Trị Viên";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var admin = new User
            {
                UserName       = adminEmail,
                Email          = adminEmail,
                FullName       = adminFullName,
                Role           = UserRole.Admin,
                IsActive       = true,
                CreatedAt      = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, nameof(UserRole.Admin));
                logger.LogInformation(
                    "Default admin account created → Email: {Email}  Password: {Password}",
                    adminEmail, adminPassword);
            }
            else
            {
                logger.LogWarning("Failed to create admin account: {Errors}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            // Idempotent: ensure the existing account is in the Admin role
            if (!await userManager.IsInRoleAsync(existingAdmin, nameof(UserRole.Admin)))
            {
                await userManager.AddToRoleAsync(existingAdmin, nameof(UserRole.Admin));
                logger.LogInformation("Assigned Admin role to existing account: {Email}", adminEmail);
            }
            else
            {
                logger.LogInformation("Admin account already exists: {Email}", adminEmail);
            }
        }

        logger.LogInformation("Database initialization complete.");
    }
}
