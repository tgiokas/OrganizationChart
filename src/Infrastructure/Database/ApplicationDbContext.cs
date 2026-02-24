using Microsoft.EntityFrameworkCore;

using ExternalIntegrations.OrganizationChart.Domain.Entities;

namespace ExternalIntegrations.OrganizationChart.Infrastructure.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public required DbSet<ImportJob> ImportJobs { get; set; }
    public required DbSet<ImportJobItem> ImportJobItems { get; set; }
    public required DbSet<ImportedUser> ImportedUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ImportJob
        modelBuilder.Entity<ImportJob>(entity =>
        {
            entity.ToTable("ImportJobs");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.JobId).IsUnique();
            entity.Property(e => e.JobId).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.HasMany(e => e.Items)
                  .WithOne(i => i.ImportJob)
                  .HasForeignKey(i => i.ImportJobId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ImportJobItem
        modelBuilder.Entity<ImportJobItem>(entity =>
        {
            entity.ToTable("ImportJobItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => new { e.ImportJobId, e.Status });
        });

        // Configure ImportedUser (mapping table)
        modelBuilder.Entity<ImportedUser>(entity =>
        {
            entity.ToTable("ImportedUsers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PartnerCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.ExternalId).IsUnique();
            entity.HasIndex(e => e.KeycloakUserId).IsUnique();
            entity.HasIndex(e => e.PartnerCode);
        });
    }
}
