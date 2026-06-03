using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace adapters.DrivenAdapters.Data
{
    public class ShcDbContextFactory : IDesignTimeDbContextFactory<ShcDbContext>
    {
        public ShcDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var apiPath = ResolveApiPath();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("DefaultConnection is missing from API appsettings.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ShcDbContext>();
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));

            return new ShcDbContext(optionsBuilder.Options);
        }

        private static string ResolveApiPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            if (File.Exists(Path.Combine(currentDirectory, "appsettings.json")))
            {
                return currentDirectory;
            }

            var apiDirectory = Path.Combine(currentDirectory, "api");

            if (File.Exists(Path.Combine(apiDirectory, "appsettings.json")))
            {
                return apiDirectory;
            }

            return Path.GetFullPath(Path.Combine(currentDirectory, "..", "api"));
        }
    }
}
