using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Npgsql;

using IntegrationImport.Application.Configuration;
using IntegrationImport.Application.Errors;
using IntegrationImport.Application.Interfaces;
using IntegrationImport.Application.Services;
using IntegrationImport.Domain.Interfaces;
using IntegrationImport.Infrastructure.BackgroundServices;
using IntegrationImport.Infrastructure.Database;
using IntegrationImport.Infrastructure.ExternalServices;
using IntegrationImport.Infrastructure.Messaging;
using IntegrationImport.Infrastructure.Repositories;

namespace IntegrationImport.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, string databaseProvider)
    {
        // Bind KafkaSettings from env variables
        var kafkaSettings = KafkaSettings.BindFromConfiguration(configuration);
        services.AddSingleton(Options.Create(kafkaSettings));
        
        // Bind IntegrationImportSettings from env variables
        var integrationImportSettings = IntegrationImportSettings.BindFromConfiguration(configuration);
        services.AddSingleton(Options.Create(integrationImportSettings));

        // Database
        var connectionString = integrationImportSettings.DbConnection;

        NpgsqlDataSource? dataSource = null;
        if (databaseProvider.Equals("postgresql", StringComparison.OrdinalIgnoreCase))
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson();
            dataSource = dataSourceBuilder.Build();
        }
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            switch (databaseProvider.ToLower())
            {
                case "sqlserver":
                    options.UseSqlServer(connectionString);
                    break;

                case "postgresql":
                    // Pin history table to "public" to avoid schema mismatch with Zalando prepared DBs (default schema "data")
                    // and Npgsql existence check (see npgsql/efcore.pg#2787, #2878, #3354)
                    options.UseNpgsql(dataSource, npgsql =>
                    {
                        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                    }).UseSnakeCaseNamingConvention();
                    break;

                case "sqlite":
                    //options.UseSqlite(connectionString);                     
                    break;

                default:
                    throw new ArgumentException($"Unsupported database provider: {databaseProvider}");
            }
        });


        // Repositories
        services.AddScoped<IImportRequestRepository, ImportRequestRepository>();
        services.AddScoped<IUserImportItemRepository, UserImportItemRepository>();
        services.AddScoped<IOrganizationUnitImportItemRepository, OrgUnitImportItemRepository>();
        services.AddScoped<IImportedUserRepository, ImportedUserRepository>();
        services.AddScoped<IImportedOrganizationUnitRepository, ImportedOrganizationUnitRepository>();
        services.AddScoped<IImportedUserOrganizationUnitRepository, ImportedUserOrganizationUnitRepository>();
       
        services.AddScoped<IDbTransactionScope, DbTransactionScope>();

        // External Services — HttpClient for DMS.Authentication
        services.AddHttpClient<IAuthApiClient, AuthApiHttpClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["AUTH_API_BASE_URL"]
                ?? throw new InvalidOperationException("AUTH_API_BASE_URL is not configured"));
        });

        // Kafka producer
        services.AddSingleton<IMessagePublisher, KafkaPublisher>();

        // Kafka consumer 
        services.AddHostedService<KafkaImportConsumer>();

        //Background service 
        services.AddHostedService<ApprovedDispatcherBackgroundService>();

        services.AddScoped<IImportProcessingService, ImportProcessingService>();

        //Add Error Catalog Path
        var path = Path.Combine(Environment.CurrentDirectory, "errors.json");
        if (!File.Exists(path))
            throw new FileNotFoundException($"errors.json not found at: {path}");

        var errorcat = ErrorCatalog.LoadFromFile(path);
        services.AddSingleton<IErrorCatalog>(errorcat);



        return services;
    }
}
