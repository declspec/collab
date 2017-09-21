using Dapper;
using Collab.Api.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Collab.Api.Config {
    public static class DatabaseConfig {
        // Create an extension method for the IServiceCollection
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration config) {
            var connectionString = config.GetConnectionString("DefaultConnection");
            var currentAssembly = typeof(Startup).GetTypeInfo().Assembly;
            var provider = new SqliteConnectionProvider(connectionString);

            // Dapper config
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            // Run the migrations
            using (var connection = provider.GetConnection()) {
                var migrator = new DatabaseMigrator(connection, currentAssembly);
                migrator.MigrateToLatest();
            }

            // Add the service
            services.AddSingleton<IDbConnectionProvider>(provider);
        }
    }
}