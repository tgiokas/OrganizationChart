using Microsoft.Extensions.DependencyInjection;

using ExternalIntegrations.OrganizationChart.Application.Interfaces;
using ExternalIntegrations.OrganizationChart.Application.Services;

namespace ExternalIntegrations.OrganizationChart.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IImportService, ImportService>();
        services.AddScoped<IImportProcessingService, ImportProcessingService>();
        return services;
    }
}
