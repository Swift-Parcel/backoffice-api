using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixShadowProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cases_handlers_escalated_to_id",
                table: "cases");

            migrationBuilder.DropTable(
                name: "case_parcels");

            migrationBuilder.DropTable(
                name: "case_tags");

            migrationBuilder.DropTable(
                name: "holiday_regions");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "status_workflow_roles");

            migrationBuilder.DropTable(
                name: "user_regions");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropIndex(
                name: "ix_cases_escalated_to_id",
                table: "cases");

            migrationBuilder.DropColumn(
                name: "recipient_address",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "sender_address",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "sender_name",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "address",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "escalated_to_id",
                table: "cases");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "system_configs",
                newName: "updated_by_id");

            migrationBuilder.RenameColumn(
                name: "rule_name",
                table: "sla_rules",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "region_name",
                table: "regions",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "ix_regions_region_name",
                table: "regions",
                newName: "ix_regions_name");

            migrationBuilder.RenameColumn(
                name: "region",
                table: "cases",
                newName: "region_id");

            migrationBuilder.RenameColumn(
                name: "author_user_id",
                table: "case_notes",
                newName: "author_id");

            migrationBuilder.AddColumn<int>(
                name: "role_id",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "escalation_handler_id",
                table: "sla_rules",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "escalation_department",
                table: "sla_rules",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "status_workflow_id",
                table: "roles",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "country_code",
                table: "regions",
                type: "character varying(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "holiday_id",
                table: "regions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "role_id",
                table: "permissions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recipient_address_id",
                table: "parcels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "address_id",
                table: "customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    street = table.Column<string>(type: "text", nullable: false),
                    street_number = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    postal_code = table.Column<string>(type: "text", nullable: false),
                    country_code = table.Column<string>(type: "character varying(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_addresses_countries_country_code",
                        column: x => x.country_code,
                        principalTable: "countries",
                        principalColumn: "country_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "case_parcel",
                columns: table => new
                {
                    cases_id = table.Column<int>(type: "integer", nullable: false),
                    parcels_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_case_parcel", x => new { x.cases_id, x.parcels_id });
                    table.ForeignKey(
                        name: "fk_case_parcel_cases_cases_id",
                        column: x => x.cases_id,
                        principalTable: "cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_case_parcel_parcels_parcels_id",
                        column: x => x.parcels_id,
                        principalTable: "parcels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "case_tag",
                columns: table => new
                {
                    cases_id = table.Column<int>(type: "integer", nullable: false),
                    tags_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_case_tag", x => new { x.cases_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_case_tag_cases_cases_id",
                        column: x => x.cases_id,
                        principalTable: "cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_case_tag_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "region_user",
                columns: table => new
                {
                    regions_id = table.Column<int>(type: "integer", nullable: false),
                    users_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_region_user", x => new { x.regions_id, x.users_id });
                    table.ForeignKey(
                        name: "fk_region_user_regions_regions_id",
                        column: x => x.regions_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_region_user_users_users_id",
                        column: x => x.users_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_configs_updated_by_id",
                table: "system_configs",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_sla_rules_escalation_handler_id",
                table: "sla_rules",
                column: "escalation_handler_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_status_workflow_id",
                table: "roles",
                column: "status_workflow_id");

            migrationBuilder.CreateIndex(
                name: "ix_regions_country_code",
                table: "regions",
                column: "country_code");

            migrationBuilder.CreateIndex(
                name: "ix_regions_holiday_id",
                table: "regions",
                column: "holiday_id");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_role_id",
                table: "permissions",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_parcels_customer_id",
                table: "parcels",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_parcels_recipient_address_id",
                table: "parcels",
                column: "recipient_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_address_id",
                table: "customers",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "ix_cases_region_id",
                table: "cases",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_case_notes_author_id",
                table: "case_notes",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_case_notes_case_id",
                table: "case_notes",
                column: "case_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_addresses_country_code",
                table: "addresses",
                column: "country_code");

            migrationBuilder.CreateIndex(
                name: "ix_case_parcel_parcels_id",
                table: "case_parcel",
                column: "parcels_id");

            migrationBuilder.CreateIndex(
                name: "ix_case_tag_tags_id",
                table: "case_tag",
                column: "tags_id");

            migrationBuilder.CreateIndex(
                name: "ix_region_user_users_id",
                table: "region_user",
                column: "users_id");

            migrationBuilder.AddForeignKey(
                name: "fk_audit_logs_users_user_id",
                table: "audit_logs",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_case_notes_cases_case_id",
                table: "case_notes",
                column: "case_id",
                principalTable: "cases",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_case_notes_users_author_id",
                table: "case_notes",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cases_regions_region_id",
                table: "cases",
                column: "region_id",
                principalTable: "regions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_customers_addresses_address_id",
                table: "customers",
                column: "address_id",
                principalTable: "addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_handlers_users_user_id",
                table: "handlers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_parcels_addresses_recipient_address_id",
                table: "parcels",
                column: "recipient_address_id",
                principalTable: "addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_parcels_customers_customer_id",
                table: "parcels",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_permissions_roles_role_id",
                table: "permissions",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_regions_countries_country_code",
                table: "regions",
                column: "country_code",
                principalTable: "countries",
                principalColumn: "country_code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_regions_holidays_holiday_id",
                table: "regions",
                column: "holiday_id",
                principalTable: "holidays",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roles_status_workflows_status_workflow_id",
                table: "roles",
                column: "status_workflow_id",
                principalTable: "status_workflows",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_sla_rules_handlers_escalation_handler_id",
                table: "sla_rules",
                column: "escalation_handler_id",
                principalTable: "handlers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_system_configs_users_updated_by_id",
                table: "system_configs",
                column: "updated_by_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_roles_role_id",
                table: "users",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_audit_logs_users_user_id",
                table: "audit_logs");

            migrationBuilder.DropForeignKey(
                name: "fk_case_notes_cases_case_id",
                table: "case_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_case_notes_users_author_id",
                table: "case_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_cases_regions_region_id",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "fk_customers_addresses_address_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "fk_handlers_users_user_id",
                table: "handlers");

            migrationBuilder.DropForeignKey(
                name: "fk_parcels_addresses_recipient_address_id",
                table: "parcels");

            migrationBuilder.DropForeignKey(
                name: "fk_parcels_customers_customer_id",
                table: "parcels");

            migrationBuilder.DropForeignKey(
                name: "fk_permissions_roles_role_id",
                table: "permissions");

            migrationBuilder.DropForeignKey(
                name: "fk_regions_countries_country_code",
                table: "regions");

            migrationBuilder.DropForeignKey(
                name: "fk_regions_holidays_holiday_id",
                table: "regions");

            migrationBuilder.DropForeignKey(
                name: "fk_roles_status_workflows_status_workflow_id",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "fk_sla_rules_handlers_escalation_handler_id",
                table: "sla_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_system_configs_users_updated_by_id",
                table: "system_configs");

            migrationBuilder.DropForeignKey(
                name: "fk_users_roles_role_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "case_parcel");

            migrationBuilder.DropTable(
                name: "case_tag");

            migrationBuilder.DropTable(
                name: "region_user");

            migrationBuilder.DropIndex(
                name: "ix_users_role_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_system_configs_updated_by_id",
                table: "system_configs");

            migrationBuilder.DropIndex(
                name: "ix_sla_rules_escalation_handler_id",
                table: "sla_rules");

            migrationBuilder.DropIndex(
                name: "ix_roles_status_workflow_id",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "ix_regions_country_code",
                table: "regions");

            migrationBuilder.DropIndex(
                name: "ix_regions_holiday_id",
                table: "regions");

            migrationBuilder.DropIndex(
                name: "ix_permissions_role_id",
                table: "permissions");

            migrationBuilder.DropIndex(
                name: "ix_parcels_customer_id",
                table: "parcels");

            migrationBuilder.DropIndex(
                name: "ix_parcels_recipient_address_id",
                table: "parcels");

            migrationBuilder.DropIndex(
                name: "ix_customers_address_id",
                table: "customers");

            migrationBuilder.DropIndex(
                name: "ix_cases_region_id",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "ix_case_notes_author_id",
                table: "case_notes");

            migrationBuilder.DropIndex(
                name: "ix_case_notes_case_id",
                table: "case_notes");

            migrationBuilder.DropIndex(
                name: "ix_audit_logs_user_id",
                table: "audit_logs");

            migrationBuilder.DropColumn(
                name: "role_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "status_workflow_id",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "holiday_id",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "role_id",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "recipient_address_id",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "address_id",
                table: "customers");

            migrationBuilder.RenameColumn(
                name: "updated_by_id",
                table: "system_configs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "sla_rules",
                newName: "rule_name");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "regions",
                newName: "region_name");

            migrationBuilder.RenameIndex(
                name: "ix_regions_name",
                table: "regions",
                newName: "ix_regions_region_name");

            migrationBuilder.RenameColumn(
                name: "region_id",
                table: "cases",
                newName: "region");

            migrationBuilder.RenameColumn(
                name: "author_id",
                table: "case_notes",
                newName: "author_user_id");

            migrationBuilder.AlterColumn<int>(
                name: "escalation_handler_id",
                table: "sla_rules",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "escalation_department",
                table: "sla_rules",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "country_code",
                table: "regions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)");

            migrationBuilder.AddColumn<string>(
                name: "recipient_address",
                table: "parcels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sender_address",
                table: "parcels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sender_name",
                table: "parcels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "escalated_to_id",
                table: "cases",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "case_parcels",
                columns: table => new
                {
                    case_id = table.Column<int>(type: "integer", nullable: false),
                    parcel_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_case_parcels", x => new { x.case_id, x.parcel_id });
                    table.ForeignKey(
                        name: "fk_case_parcels_cases_case_id",
                        column: x => x.case_id,
                        principalTable: "cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_case_parcels_parcels_parcel_id",
                        column: x => x.parcel_id,
                        principalTable: "parcels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "case_tags",
                columns: table => new
                {
                    case_id = table.Column<int>(type: "integer", nullable: false),
                    tag_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_case_tags", x => new { x.case_id, x.tag_id });
                    table.ForeignKey(
                        name: "fk_case_tags_cases_case_id",
                        column: x => x.case_id,
                        principalTable: "cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_case_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "holiday_regions",
                columns: table => new
                {
                    holiday_id = table.Column<int>(type: "integer", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_holiday_regions", x => new { x.holiday_id, x.region_id });
                    table.ForeignKey(
                        name: "fk_holiday_regions_holidays_holiday_id",
                        column: x => x.holiday_id,
                        principalTable: "holidays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_holiday_regions_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    permission_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permissions", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "fk_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "status_workflow_roles",
                columns: table => new
                {
                    workflow_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_status_workflow_roles", x => new { x.workflow_id, x.role_id });
                });

            migrationBuilder.CreateTable(
                name: "user_regions",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_regions", x => new { x.user_id, x.region_id });
                    table.ForeignKey(
                        name: "fk_user_regions_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_regions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cases_escalated_to_id",
                table: "cases",
                column: "escalated_to_id");

            migrationBuilder.CreateIndex(
                name: "ix_case_parcels_parcel_id",
                table: "case_parcels",
                column: "parcel_id");

            migrationBuilder.CreateIndex(
                name: "ix_case_tags_tag_id",
                table: "case_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_holiday_regions_region_id",
                table: "holiday_regions",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_regions_region_id",
                table: "user_regions",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cases_handlers_escalated_to_id",
                table: "cases",
                column: "escalated_to_id",
                principalTable: "handlers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
