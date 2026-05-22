using Microsoft.EntityFrameworkCore;

using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Infrastructure.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }


    public DbSet<ImportRequest> ImportRequests => Set<ImportRequest>();
    public DbSet<UserImportItem> UserImportItems => Set<UserImportItem>();
    public DbSet<ImportedUser> ImportedUsers => Set<ImportedUser>();

    public DbSet<OrganizationUnitImportItem> OrganizationUnitImportItems => Set<OrganizationUnitImportItem>();
    public DbSet<ImportedOrganizationUnit> ImportedOrganizationUnits => Set<ImportedOrganizationUnit>();
    public DbSet<ImportedUserOrganizationUnit> ImportedUserOrganizationUnits => Set<ImportedUserOrganizationUnit>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    }
}
