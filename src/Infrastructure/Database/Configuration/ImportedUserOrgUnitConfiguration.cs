using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using IntegrationImport.Domain.Entities;



namespace IntegrationImport.Infrastructure.Database.Configuration;

public class ImportedUserOrganizationUnitConfiguration : IEntityTypeConfiguration<ImportedUserOrganizationUnit>
{
    public void Configure(EntityTypeBuilder<ImportedUserOrganizationUnit> builder)
    {
        builder.ToTable("ImportedUserOrganizationUnits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserExternalId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.OrgUnitExternalId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.ExternalSyncStatus)
            .HasMaxLength(50);

        builder.Property(x => x.ExternalSyncError)
            .HasMaxLength(4000);

        builder.HasOne(x => x.ImportedUser)
            .WithMany(x => x.OrganizationUnits)
            .HasForeignKey(x => x.ImportedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ImportedOrganizationUnit)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.ImportedOrganizationUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ImportedUserId, x.ImportedOrganizationUnitId })
            .IsUnique();

        builder.HasIndex(x => x.UserExternalId);

        builder.HasIndex(x => x.OrgUnitExternalId);

        builder.HasIndex(x => x.ExternalSyncStatus);
    }
}