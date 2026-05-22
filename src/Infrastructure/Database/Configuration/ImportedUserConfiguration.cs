using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using IntegrationImport.Domain.Entities;

namespace IntegrationImport.Infrastructure.Database.Configuration;

public class ImportedUserConfiguration : IEntityTypeConfiguration<ImportedUser>
{
    public void Configure(EntityTypeBuilder<ImportedUser> builder)
    {
        builder.ToTable("ImportedUsers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ExternalId)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(x => x.KeycloakUserId);            

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(x => x.Email)
                .HasMaxLength(200);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.LastSyncedAtUtc);

        builder.Property(x => x.LastAction)
            .HasMaxLength(50);

        builder.Property(x => x.LastSyncStatus)
            .HasMaxLength(50);

            builder.Property(x => x.SourceImportItemId)          
                .IsRequired(false);


            builder.HasIndex(x => x.SourceImportItemId)
                .IsUnique()               
                .HasFilter("\"source_import_item_id\" IS NOT NULL");

            builder.HasIndex(x => x.ExternalId)
                .IsUnique();


            // multiple PENDING_AUTH rows (NULL) don't collide on it:
            builder.HasIndex(x => x.KeycloakUserId)
                .IsUnique()
                .HasFilter("\"keycloak_user_id\" IS NOT NULL");


        builder.HasIndex(x => x.Username);
    }
}
