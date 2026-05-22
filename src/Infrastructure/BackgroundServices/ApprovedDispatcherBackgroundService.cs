using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using IntegrationImport.Application.Configuration;
using IntegrationImport.Application.Interfaces;
using IntegrationImport.Domain.Interfaces;

namespace IntegrationImport.Infrastructure.BackgroundServices;

public class ApprovedDispatcherBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ApprovedDispatcherBackgroundService> _logger;
    private readonly IntegrationImportSettings _options;

    public ApprovedDispatcherBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ApprovedDispatcherBackgroundService> logger,
        IOptions<IntegrationImportSettings> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO: When scaling to multiple instances, add pg_advisory_lock here
        // to prevent duplicate Kafka publishes. Current single-pod deployment is safe.


        _logger.LogInformation("ApprovedDispatcherBackgroundService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var userRepository    = scope.ServiceProvider.GetRequiredService<IUserImportItemRepository>();
                var orgUnitRepository = scope.ServiceProvider.GetRequiredService<IOrganizationUnitImportItemRepository>();
                var dispatchService   = scope.ServiceProvider.GetRequiredService<IImportDispatchService>();

                // ---- Users ----
                var userItems = await userRepository
                    .GetApprovedPendingDispatchUserItemsAsync(_options.BatchSize, stoppingToken);

                foreach (var item in userItems)
                {
                    try
                    {
                        var result = await dispatchService.DispatchApprovedUserItemAsync(item.Id, stoppingToken);
                        if (!result.Success)
                            _logger.LogError($"Failed to dispatch approved user item {item.Id}: ErrorCode: {result.ErrorCode}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to dispatch approved user item {ItemId}", item.Id);
                    }
                }

                // ---- Organization Units ----
                var orgUnitItems = await orgUnitRepository
                    .GetApprovedPendingDispatchOrgUnitItemsAsync(_options.BatchSize, stoppingToken);

                foreach (var item in orgUnitItems)
                {
                    try
                    {
                        var result = await dispatchService.DispatchApprovedOrgUnitItemAsync(item.Id, stoppingToken);
                        if (!result.Success)
                            _logger.LogError($"Failed to dispatch approved org-unit item {item.Id}: ErrorCode: {result.ErrorCode}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to dispatch approved org-unit item {ItemId}", item.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Approved item dispatcher loop failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.PollIntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("ApprovedDispatcherBackgroundService stopped.");
    }
}