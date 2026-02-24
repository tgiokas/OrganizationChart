using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ExternalIntegrations.OrganizationChart.Application.Interfaces;
using ExternalIntegrations.OrganizationChart.Domain.Interfaces;
using ExternalIntegrations.OrganizationChart.Infrastructure.Database;
using ExternalIntegrations.OrganizationChart.Infrastructure.ExternalServices;
using ExternalIntegrations.OrganizationChart.Infrastructure.Messaging;
using ExternalIntegrations.OrganizationChart.Infrastructure.Repositories;

namespace ExternalIntegrations.OrganizationChart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration["ORGCHART_DB_CONNECTION"])
                   .UseSnakeCaseNamingConvention());

        // Repositories
        services.AddScoped<IImportJobRepository, ImportJobRepository>();
        services.AddScoped<IImportJobItemRepository, ImportJobItemRepository>();
        services.AddScoped<IImportedUserRepository, ImportedUserRepository>();

        // External Services — HttpClient for DMS.Authentication
        services.AddHttpClient<IAuthApiClient, AuthApiHttpClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["AUTH_API_BASE_URL"]
                ?? throw new InvalidOperationException("AUTH_API_BASE_URL is not configured"));
        });

        // Kafka produser
        services.AddSingleton<IMessagePublisher, KafkaPublisher>();
        services.AddSingleton<IImportMessagePublisher, KafkaImportPublisher>();

        // Kafka consumer — BackgroundService
        services.AddHostedService<KafkaImportConsumer>();

        return services;
    }
}
