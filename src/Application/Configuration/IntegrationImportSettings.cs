using Microsoft.Extensions.Configuration;

namespace IntegrationImport.Application.Configuration;

public class IntegrationImportSettings
{
    //Database POrovider 
    public string DatabaseProvider { get; set; } = "Postgres"; 

    // Database
    public string DbConnection { get; set; } = string.Empty;

    // Frontend
    public string FrontendEntraIdRedirectUri { get; set; } = string.Empty;

    // Email Whitelist ("off" to disable, "domain" or "email" to enable)
    public string EmailsWhitelist { get; set; } = "off";

    //Dispatch 
    public int PollIntervalSeconds { get; set; } = 10;
    public int BatchSize { get; set; } = 20;

    //Process Import
    public bool RequireAdminApproval { get; set; } = true;  //HumanInTheLoop
    public bool RunMigrationOnStartup { get; set; } = false;

    public static IntegrationImportSettings BindFromConfiguration(IConfiguration configuration)
    {
        return new IntegrationImportSettings
        {
            DatabaseProvider = configuration["INTEGRATIONIMPORT_DB_PROVIDER"] ?? "Postgres",

            DbConnection = configuration["INTEGRATIONIMPORT_DB_CONNECTION"]
                ?? throw new ArgumentNullException(nameof(configuration), "INTEGRATIONIMPORT_DB_CONNECTION is not set."),

            PollIntervalSeconds = int.TryParse(configuration["DISPATCH_POLL_INTERVAL_SECONDS"], out var pollInterval) ? pollInterval : 10,
            
            BatchSize = int.TryParse(configuration["DISPATCH_BATCH_SIZE"], out var batchSize) ? batchSize : 20,
            
            RequireAdminApproval = bool.TryParse(configuration["REQUIRE_ADMIN_APPROVAL"], out var requireAdminApproval) ? requireAdminApproval : true,

            RunMigrationOnStartup = bool.TryParse(configuration["RUN_MIGRATIONS_ON_STARTUP"], out var runMigrationOnStartup) ? runMigrationOnStartup : false
        };
    }
}
