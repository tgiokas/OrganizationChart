using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationImport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrgUnitSchemaForJsonSpec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "code",
                table: "ImportedOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "enabled",
                table: "OrganizationUnitImportItems");

            migrationBuilder.AddColumn<bool>(
                name: "is_virtual",
                table: "OrganizationUnitImportItems",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "abbreviation",
                table: "OrganizationUnitImportItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "applied_at_utc",
                table: "OrganizationUnitImportItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "apply_error",
                table: "OrganizationUnitImportItems",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "dispatch_attempts",
                table: "OrganizationUnitImportItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "dispatched_at_utc",
                table: "OrganizationUnitImportItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "OrganizationUnitImportItems",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "OrganizationUnitImportItems",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_dispatch_error",
                table: "OrganizationUnitImportItems",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "OrganizationUnitImportItems",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "process_status",
                table: "OrganizationUnitImportItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "review_notes",
                table: "OrganizationUnitImportItems",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "review_status",
                table: "OrganizationUnitImportItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "reviewed_at_utc",
                table: "OrganizationUnitImportItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reviewed_by",
                table: "OrganizationUnitImportItems",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "abbreviation",
                table: "ImportedOrganizationUnits",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "ImportedOrganizationUnits",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_virtual",
                table: "ImportedOrganizationUnits",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "ImportedOrganizationUnits",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_unit_import_items_review_status_process_status_di",
                table: "OrganizationUnitImportItems",
                columns: new[] { "review_status", "process_status", "dispatched_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_organization_unit_import_items_review_status_process_status_di",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "abbreviation",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "applied_at_utc",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "apply_error",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "dispatch_attempts",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "dispatched_at_utc",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "email",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "last_dispatch_error",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "location",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "process_status",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "review_notes",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "review_status",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "reviewed_at_utc",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "reviewed_by",
                table: "OrganizationUnitImportItems");

            migrationBuilder.DropColumn(
                name: "abbreviation",
                table: "ImportedOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "email",
                table: "ImportedOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "is_virtual",
                table: "ImportedOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "location",
                table: "ImportedOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "is_virtual",
                table: "OrganizationUnitImportItems");

            migrationBuilder.AddColumn<bool>(
                name: "enabled",
                table: "OrganizationUnitImportItems",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "OrganizationUnitImportItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "ImportedOrganizationUnits",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
