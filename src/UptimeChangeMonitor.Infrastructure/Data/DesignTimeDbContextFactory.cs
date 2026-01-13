using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UptimeChangeMonitor.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration from environment variables and appsettings
        // Try multiple paths to find appsettings.json
        var basePath = Directory.GetCurrentDirectory();
        var possiblePaths = new[]
        {
            Path.Combine(basePath, "..", "UptimeChangeMonitor.API"),
            "/src/src/UptimeChangeMonitor.API",
            basePath,
            Path.Combine(basePath, "..", "..", "UptimeChangeMonitor.API"),
            "/src/UptimeChangeMonitor.API"
        };

        var apiPath = possiblePaths.FirstOrDefault(Directory.Exists) ?? basePath;

        var configBuilder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        // Try to add appsettings.json from the found path
        var appsettingsPath = Path.Combine(apiPath, "appsettings.json");
        if (File.Exists(appsettingsPath))
        {
            configBuilder.SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        }

        var configuration = configBuilder.Build();

        // Get connection string - prioritize environment variable, then config file
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=UptimeChangeMonitor;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
