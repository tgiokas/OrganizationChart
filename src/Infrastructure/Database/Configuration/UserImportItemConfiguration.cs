using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Infrastructure.Database.Configuration;

public class UserImportItemConfiguration : IEntityTypeConfiguration<UserImportItem>
{
    public void Configure(EntityTypeBuilder<UserImportItem> builder)
    {
        builder.ToTable("UserImportItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ImportRequestId)
            .IsRequired();

        builder.Property(x => x.HrmsId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Username)
            .HasMaxLength(150);

        builder.Property(x => x.FirstName)
            .HasMaxLength(150);

        builder.Property(x => x.LastName)
            .HasMaxLength(150);

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(x => x.VatId)
            .HasMaxLength(50);

        builder.Property(x => x.ChangedFieldsJson)
            .HasColumnType("text");

        builder.Property(x => x.OrgUnitHrmsIdsJson)
            .HasColumnType("text");

        builder.Property(x => x.ReviewStatus)
.IsRequired()
.HasConversion<string>()
.HasMaxLength(50);

        builder.Property(x => x.ReviewedBy)
            .HasMaxLength(200);

        builder.Property(x => x.ReviewedAtUtc);

        builder.Property(x => x.ReviewNotes)
            .HasMaxLength(2000);

        builder.Property(x => x.ProcessStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.DispatchedAtUtc);

        builder.Property(x => x.DispatchAttempts)
            .IsRequired();

        builder.Property(x => x.LastDispatchError)
            .HasMaxLength(4000);

        builder.Property(x => x.AppliedAtUtc);

        builder.Property(x => x.ApplyError)
            .HasMaxLength(4000);

        builder.HasIndex(x => new { x.ReviewStatus, x.ProcessStatus, x.DispatchedAtUtc });
        builder.HasIndex(x => x.ImportRequestId);

        builder.HasIndex(x => x.HrmsId);

        builder.HasIndex(x => new { x.ImportRequestId, x.HrmsId });
    }
}
