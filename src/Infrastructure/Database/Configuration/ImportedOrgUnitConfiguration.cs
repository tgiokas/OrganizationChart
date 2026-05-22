using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using IntegrationImport.Domain.Entities;


namespace IntegrationImport.Infrastructure.Database.Configuration;

public class ImportedOrganizationUnitConfiguration : IEntityTypeConfiguration<ImportedOrganizationUnit>
{
    public void Configure(EntityTypeBuilder<ImportedOrganizationUnit> builder)
    {
        builder.ToTable("ImportedOrganizationUnits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ExternalId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .HasMaxLength(200);

        builder.Property(x => x.ParentExternalId)
            .HasMaxLength(100);

        builder.Property(x => x.Abbreviation)
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.Location)
            .HasMaxLength(200);

        builder.Property(x => x.IsVirtual)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.LastAction)
            .HasMaxLength(50);

        builder.Property(x => x.LastSyncStatus)
            .HasMaxLength(50);

        builder.HasIndex(x => x.ExternalId)
            .IsUnique();


        builder.HasOne<ImportedOrganizationUnit>()
            .WithMany()
            .HasForeignKey(x => x.ParentImportedOrganizationUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}