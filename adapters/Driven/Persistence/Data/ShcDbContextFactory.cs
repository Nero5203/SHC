using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace adapters.Driven.Persistence.Data
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

            var candidates = new[]
            {
                Path.Combine(currentDirectory, "adapters", "Driving", "Api"),
                Path.Combine(currentDirectory, "Driving", "Api"),
                Path.Combine(currentDirectory, "..", "Driving", "Api"),
                Path.Combine(currentDirectory, "..", "..", "Driving", "Api")
            };

            foreach (var candidate in candidates)
            {
                var fullPath = Path.GetFullPath(candidate);

                if (File.Exists(Path.Combine(fullPath, "appsettings.json")))
                {
                    return fullPath;
                }
            }

            throw new InvalidOperationException("Could not find the API appsettings folder.");
        }
    }
}
