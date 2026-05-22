using IntegrationImport.Application.Helpers;
using IntegrationImport.Application.Interfaces;
using IntegrationImport.Application.Interfaces.Validation;
using IntegrationImport.Application.Services;
using IntegrationImport.Application.Validation;

using Microsoft.Extensions.DependencyInjection;

namespace IntegrationImport.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IImportDispatchService, ImportDispatchService>();

        services.AddScoped<IUserImportValidator, UserImportValidator>();

        services.AddScoped<IUserImportService, UserImportService>();
        services.AddScoped<IUserImportMapper, UserImportMapper>();
        services.AddScoped<IReviewStatusService, ReviewStatusService>();

        services.AddScoped<IOrgUnitImportValidator, OrgUnitImportValidator>();
        services.AddScoped<IOrgUnitImportMapper, OrgUnitImportMapper>();
        services.AddScoped<IOrgUnitImportService, OrgUnitImportService>();

        services.AddScoped<IUserOrgUnitSynchronizer, UserOrgUnitSynchronizer>();

        return services;
    }
}
