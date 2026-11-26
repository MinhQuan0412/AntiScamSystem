using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace AntiScam.Infrastructure.Data;

/// <summary>
/// Design-time factory used by EF Core CLI tools (migrations).
/// Prefers SqlServerConnection; falls back to MySQL DefaultConnection.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "AntiScam.API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.Combine(configPath, "appsettings.json"), optional: true)
            .AddJsonFile(Path.Combine(configPath, "appsettings.Development.json"), optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var sqlServerConn = configuration.GetConnectionString("SqlServerConnection");
        if (!string.IsNullOrWhiteSpace(sqlServerConn))
        {
            optionsBuilder.UseSqlServer(sqlServerConn);
        }
        else
        {
            var mySqlConn = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=localhost;Port=3306;Database=antiscam_db;User=root;Password=root_password;CharSet=utf8mb4;";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));
            optionsBuilder.UseMySql(mySqlConn, serverVersion);
        }

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
