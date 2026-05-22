using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationImport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialHrmsSyncSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImportedUsers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_import_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    external_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    keycloak_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    first_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("pk_imported_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ImportRequests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    effective_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entity_type = table.Column<int>(type: "integer", nullable: false),
                    received_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reviewed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    item_count = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_by_client_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_import_requests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUnitImportItems",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    import_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hrms_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    parent_hrms_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    enabled = table.Column<bool>(type: "boolean", nullable: true),
                    changed_fields_json = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_unit_import_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_organization_unit_import_items_import_requests_import_request_id",
                        column: x => x.import_request_id,
                        principalTable: "ImportRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserImportItems",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    import_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hrms_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    username = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    first_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    last_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    enabled = table.Column<bool>(type: "boolean", nullable: true),
                    vat_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    changed_fields_json = table.Column<string>(type: "text", nullable: true),
                    org_unit_hrms_ids_json = table.Column<string>(type: "text", nullable: true),
                    review_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    reviewed_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    reviewed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    review_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    process_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    dispatched_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dispatch_attempts = table.Column<int>(type: "integer", nullable: false),
                    last_dispatch_error = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    applied_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    apply_error = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_import_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_import_items_import_requests_import_request_id",
                        column: x => x.import_request_id,
                        principalTable: "ImportRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_imported_users_external_id",
                table: "ImportedUsers",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_imported_users_keycloak_user_id",
                table: "ImportedUsers",
                column: "keycloak_user_id",
                unique: true,
                filter: "\"keycloak_user_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_imported_users_source_import_item_id",
                table: "ImportedUsers",
                column: "source_import_item_id",
                unique: true,
                filter: "\"source_import_item_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_imported_users_username",
                table: "ImportedUsers",
                column: "username");

            migrationBuilder.CreateIndex(
                name: "ix_import_requests_reference_id",
                table: "ImportRequests",
                column: "reference_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_import_requests_reference_id_entity_type",
                table: "ImportRequests",
                columns: new[] { "reference_id", "entity_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_import_requests_status",
                table: "ImportRequests",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_organization_unit_import_items_hrms_id",
                table: "OrganizationUnitImportItems",
                column: "hrms_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_unit_import_items_import_request_id",
                table: "OrganizationUnitImportItems",
                column: "import_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_unit_import_items_import_request_id_hrms_id",
                table: "OrganizationUnitImportItems",
                columns: new[] { "import_request_id", "hrms_id" });

            migrationBuilder.CreateIndex(
                name: "ix_user_import_items_hrms_id",
                table: "UserImportItems",
                column: "hrms_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_import_items_import_request_id",
                table: "UserImportItems",
                column: "import_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_import_items_import_request_id_hrms_id",
                table: "UserImportItems",
                columns: new[] { "import_request_id", "hrms_id" });

            migrationBuilder.CreateIndex(
                name: "ix_user_import_items_review_status_process_status_dispatched_at_",
                table: "UserImportItems",
                columns: new[] { "review_status", "process_status", "dispatched_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportedUsers");

            migrationBuilder.DropTable(
                name: "OrganizationUnitImportItems");

            migrationBuilder.DropTable(
                name: "UserImportItems");

            migrationBuilder.DropTable(
                name: "ImportRequests");
        }
    }
}