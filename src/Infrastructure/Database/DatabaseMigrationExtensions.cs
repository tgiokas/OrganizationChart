using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IntegrationImport.Infrastructure.Database;

public static class DatabaseMigrationExtensions
{
    // Arbitrary 64-bit key — same value across all instances so Postgres
    // serializes concurrent migration attempts.

    // ----- Postgres -----
    private const long PgMigrationLockKey = 7263548102934756L;

    // ----- SQL Server -----
    private const string SqlServerLockResource = "IntegrationImport_Migration";


    /// <summary>
    /// Applies any pending EF Core migrations if "RunMigrationsOnStartup" is enabled.
    /// Uses a Postgres advisory lock so multiple instances starting at once
    /// don't race. Throws on failure so the host can crash cleanly.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this IHost host, string databaseProvider, CancellationToken ct = default)
    {
        using var scope = host.Services.CreateScope();
        var sp = scope.ServiceProvider;

        var config = sp.GetRequiredService<IConfiguration>();
        var logger = sp.GetRequiredService<ILogger<ApplicationDbContext>>();

        if (!bool.TryParse(config["RUN_MIGRATIONS_ON_STARTUP"], out var runMigrations) || !runMigrations)
        {
            logger.LogInformation("RUN_MIGRATIONS_ON_STARTUP=false; skipping migrations.");
            return;
        }

        var db = sp.GetRequiredService<ApplicationDbContext>();

        try
        {
            switch (databaseProvider.ToLowerInvariant())
            {
                case "postgresql":
                    await MigrateWithPostgresLockAsync(db, logger, ct);
                    break;

                case "sqlserver":
                    await MigrateWithSqlServerLockAsync(db, logger, ct);
                    break;

                case "sqlite":
                // SQLite is single-writer; no distributed lock needed.
                //await MigrateUnlockedAsync(db, logger, ct);
                //break;

                default:
                    throw new ArgumentException(
                        $"Unsupported database provider for migrations: {databaseProvider}");
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Database migration failed at startup. Aborting.");
            throw;
        }
    }


    private static async Task MigrateWithPostgresLockAsync(ApplicationDbContext db, ILogger logger, CancellationToken ct)
    {
        await db.Database.OpenConnectionAsync(ct);
        try
        {
            await db.Database.ExecuteSqlRawAsync(
                "SELECT pg_advisory_lock({0})", new object[] { PgMigrationLockKey }, ct);
            try
            {
                await ApplyPendingAsync(db, logger, ct);
            }
            finally
            {
                await db.Database.ExecuteSqlRawAsync(
                    "SELECT pg_advisory_unlock({0})", new object[] { PgMigrationLockKey }, ct);
            }
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }

    private static async Task MigrateWithSqlServerLockAsync(ApplicationDbContext db, ILogger logger, CancellationToken ct)
    {
        await db.Database.OpenConnectionAsync(ct);
        try
        {
            // sp_getapplock requires an open transaction with @LockOwner='Transaction',
            // or 'Session' for connection-scoped. We use Session for simplicity.
            await db.Database.ExecuteSqlRawAsync(
                "EXEC sp_getapplock @Resource = {0}, @LockMode = 'Exclusive', @LockOwner = 'Session'",
                new object[] { SqlServerLockResource }, ct);
            try
            {
                await ApplyPendingAsync(db, logger, ct);
            }
            finally
            {
                await db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_releaseapplock @Resource = {0}, @LockOwner = 'Session'",
                    new object[] { SqlServerLockResource }, ct);
            }
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }


    // ----- Shared core -----
    private static async Task ApplyPendingAsync(ApplicationDbContext db, ILogger logger, CancellationToken ct)
    {
        var pending = (await db.Database.GetPendingMigrationsAsync(ct)).ToList();
        if (pending.Count == 0)
        {
            logger.LogInformation("No pending database migrations.");
            return;
        }

        logger.LogInformation(
            "Applying {Count} pending migration(s): {Migrations}",
            pending.Count, string.Join(", ", pending));

        await db.Database.MigrateAsync(ct);

        logger.LogInformation("Database migrations applied successfully.");
    }
}