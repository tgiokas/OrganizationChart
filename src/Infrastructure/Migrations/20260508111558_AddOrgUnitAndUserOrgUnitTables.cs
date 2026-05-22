using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationImport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgUnitAndUserOrgUnitTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImportedOrganizationUnits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_import_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    external_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    parent_external_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    parent_imported_organization_unit_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_synced_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_import_request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_import_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    last_sync_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_imported_organization_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_imported_organization_units_imported_organization_units_parent_",
                        column: x => x.parent_imported_organization_unit_id,
                        principalTable: "ImportedOrganizationUnits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImportedUserOrganizationUnits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    imported_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    imported_organization_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_external_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    org_unit_external_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_synced_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_import_request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_import_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    external_sync_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    external_synced_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    external_sync_error = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_imported_user_organization_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_imported_user_organization_units_imported_organization_units_imp",
                        column: x => x.imported_organization_unit_id,
                        principalTable: "ImportedOrganizationUnits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_imported_user_organization_units_imported_users_imported_user_id",
                        column: x => x.imported_user_id,
                        principalTable: "ImportedUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_imported_organization_units_external_id",
                table: "ImportedOrganizationUnits",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_imported_organization_units_parent_imported_organization_unit",
                table: "ImportedOrganizationUnits",
                column: "parent_imported_organization_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_imported_user_organization_units_external_sync_status",
                table: "ImportedUserOrganizationUnits",
                column: "external_sync_status");

            migrationBuilder.CreateIndex(
                name: "ix_imported_user_organization_units_imported_organization_unit_id",
                table: "ImportedUserOrganizationUnits",
                column: "imported_organization_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_imported_user_organization_units_imported_user_id_imported_org",
                table: "ImportedUserOrganizationUnits",
                columns: new[] { "imported_user_id", "imported_organization_unit_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_imported_user_organization_units_org_unit_external_id",
                table: "ImportedUserOrganizationUnits",
                column: "org_unit_external_id");

            migrationBuilder.CreateIndex(
                name: "ix_imported_user_organization_units_user_external_id",
                table: "ImportedUserOrganizationUnits",
                column: "user_external_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportedUserOrganizationUnits");

            migrationBuilder.DropTable(
                name: "ImportedOrganizationUnits");
        }
    }
}
