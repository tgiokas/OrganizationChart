using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Infrastructure.Database.Configuration;

public class OrganizationUnitImportItemConfiguration : IEntityTypeConfiguration<OrganizationUnitImportItem>
{
    public void Configure(EntityTypeBuilder<OrganizationUnitImportItem> builder)
    {
        builder.ToTable("OrganizationUnitImportItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.HrmsId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(50);

        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Abbreviation).HasMaxLength(50);
        builder.Property(x => x.ParentHrmsId).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Location).HasMaxLength(200);

        builder.Property(x => x.ReviewStatus)
            .HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.ReviewedBy).HasMaxLength(200);
        builder.Property(x => x.ReviewNotes).HasMaxLength(2000);
        builder.Property(x => x.ProcessStatus)
            .HasConversion<string?>().HasMaxLength(50);
        builder.Property(x => x.LastDispatchError).HasMaxLength(4000);
        builder.Property(x => x.ApplyError).HasMaxLength(4000);

        builder.HasOne(x => x.ImportRequest)
            .WithMany(r => r.OrganizationUnits)
            .HasForeignKey(x => x.ImportRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.HrmsId);
        builder.HasIndex(x => x.ImportRequestId);
        builder.HasIndex(x => new { x.ImportRequestId, x.HrmsId });
        builder.HasIndex(x => new { x.ReviewStatus, x.ProcessStatus, x.DispatchedAtUtc });
    }
}