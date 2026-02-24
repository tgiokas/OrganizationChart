using Microsoft.Extensions.DependencyInjection;

using OrganizationChart.Application.Interfaces;
using OrganizationChart.Application.Services;

namespace ExternalIntegrations.OrganizationChart.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IImportService, ImportService>();
        services.AddScoped<IImportProcessingService, ImportProcessingService>();
        return services;
    }
}
