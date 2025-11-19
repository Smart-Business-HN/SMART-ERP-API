using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SMART.ERP.Infrastructure
{
    /// <summary>
    /// Fábrica de contexto para herramientas de EF Core (migraciones en tiempo de diseño).
    /// Busca la cadena de conexión en variables de entorno y, como respaldo,
    /// en los appsettings del proyecto API.
    /// </summary>
    public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                 ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                                 ?? "Development";

            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            // Intentar cargar appsettings del proyecto API si existen localmente
            string currentDir = Directory.GetCurrentDirectory();
            string? solutionDir = Directory.GetParent(currentDir)?.FullName ?? currentDir;
            string apiDir = Path.Combine(solutionDir, "SMART.ERP.API");

            if (Directory.Exists(apiDir))
            {
                configurationBuilder
                    .SetBasePath(apiDir)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false);
            }

            IConfiguration configuration = configurationBuilder.Build();

            string? connectionString = configuration.GetConnectionString("Database");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("No se encontró la cadena de conexión 'Database'. Configúrala en variables de entorno o en appsettings.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure();
            });

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
