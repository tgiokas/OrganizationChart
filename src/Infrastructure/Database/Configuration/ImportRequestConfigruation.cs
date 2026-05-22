using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Infrastructure.Database.Configuration;

public class ImportRequestConfiguration : IEntityTypeConfiguration<ImportRequest>
{
    public void Configure(EntityTypeBuilder<ImportRequest> builder)
    {
        builder.ToTable("ImportRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ReferenceId)
            .IsRequired()
            .HasMaxLength(100);


        builder.Property(x => x.EffectiveDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);


        builder.HasIndex(x => new { x.ReferenceId, x.EntityType }).IsUnique();

        builder.Property(x => x.ReceivedAtUtc)
            .IsRequired();

        builder.Property(x => x.ItemCount)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.ReviewedAtUtc);

        builder.Property(x => x.CompletedAtUtc);

        builder.Property(x => x.CreatedByClientId)
            .HasMaxLength(200);

        builder.HasMany(x => x.Users)
            .WithOne(x => x.ImportRequest)
            .HasForeignKey(x => x.ImportRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.OrganizationUnits)
            .WithOne(x => x.ImportRequest)
            .HasForeignKey(x => x.ImportRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.ReferenceId)
            .IsUnique();

    }
}