using System;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SwiftParcel.Domain.Enums;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:enum_action", "assign,create,delete,escalate,login_failed,login_succeeded,node_add,permission_grant,role_change,status_change,update")
                .Annotation("Npgsql:Enum:enum_action.audit_action", "create,status_change,assign,update,permission_grant,login_failed,login_succeeded,role_change,delete,escalate,node_add")
                .Annotation("Npgsql:Enum:enum_case_status", "awaiting_customer,cancelled,closed,escalated,in_progress,open,resolved")
                .Annotation("Npgsql:Enum:enum_case_status.case_status", "open,in_progress,awaiting_customer,resolved,closed,escalated,cancelled")
                .Annotation("Npgsql:Enum:enum_case_type", "billing,damaged,delayed,delivery_change,lost,other,wrong_address")
                .Annotation("Npgsql:Enum:enum_case_type.case_type", "lost,damaged,delayed,wrong_address,billing,delivery_change,other")
                .Annotation("Npgsql:Enum:enum_channel", "chat,email,phone,portal")
                .Annotation("Npgsql:Enum:enum_channel.channel", "email,phone,chat,portal")
                .Annotation("Npgsql:Enum:enum_day_of_week", "friday,monday,saturday,sunday,thursday,tuesday,wednesday")
                .Annotation("Npgsql:Enum:enum_day_of_week.day_of_week", "sunday,monday,tuesday,wednesday,thursday,friday,saturday")
                .Annotation("Npgsql:Enum:enum_entity_type", "auto_assignment_rule,case,customer,email_template,handler,holiday,note,parcel,region,role,sla_rule,status_workflow,system_config,user,user_permission")
                .Annotation("Npgsql:Enum:enum_entity_type.entity_type", "auto_assignment_rule,note,case,customer,email_template,handler,holiday,parcel,region,role,sla_rule,status_workflow,system_config,user_permission,user")
                .Annotation("Npgsql:Enum:enum_parcel_status", "damaged,delivered,delivery_attempt_failed,in_transit,lost,out_for_delivery,pending_pickup,picked_up")
                .Annotation("Npgsql:Enum:enum_parcel_status.parcel_status", "pending_pickup,picked_up,in_transit,out_for_delivery,delivered,delivery_attempt_failed,lost,damaged")
                .Annotation("Npgsql:Enum:enum_priority", "critical,high,low,medium")
                .Annotation("Npgsql:Enum:enum_priority.priority", "low,medium,high,critical")
                .Annotation("Npgsql:Enum:enum_service_type", "express,same_day,standard")
                .Annotation("Npgsql:Enum:enum_service_type.service_type", "standard,express,same_day")
                .Annotation("Npgsql:Enum:enum_timeslot", "afternoon,evening,morning")
                .Annotation("Npgsql:Enum:enum_timeslot.timeslot", "morning,afternoon,evening")
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    audit_action = table.Column<AuditAction>(type: "enum_action", nullable: false),
                    entity_type = table.Column<EntityType>(type: "enum_entity_type", nullable: false),
                    entity_id = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    old_value = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    details = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auto_assignment_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rule_name = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false, defaultValue: 99),
                    conditions = table.Column<string>(type: "text", nullable: false),
                    assign_to_handler = table.Column<int>(type: "integer", nullable: false),
                    assign_to_department = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auto_assignment_rules", x => x.id);
                    table.CheckConstraint("CK_auto_assignment_rules_priority", "priority >= 0 AND priority <= 99");
                });

            migrationBuilder.CreateTable(
                name: "case_notes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    case_id = table.Column<int>(type: "integer", nullable: false),
                    author_user_id = table.Column<int>(type: "integer", nullable: false),
                    note_text = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_internal = table.Column<bool>(type: "boolean", nullable: false),
                    attachment = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_case_notes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    country_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    country_name = table.Column<string>(type: "text", nullable: false),
                    time_zone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_countries", x => x.country_code);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "citext", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    registered_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    vip = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "email_templates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    template_name = table.Column<string>(type: "text", nullable: false),
                    language = table.Column<string>(type: "text", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false),
                    subject = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "handlers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    department = table.Column<string>(type: "text", nullable: false),
                    hire_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    max_cases = table.Column<int>(type: "integer", nullable: false, defaultValue: 10)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_handlers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "holidays",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    holiday_name = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    is_recurring = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_holidays", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parcels",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tracking_number = table.Column<string>(type: "text", nullable: false),
                    sender_name = table.Column<string>(type: "text", nullable: false),
                    sender_address = table.Column<string>(type: "text", nullable: false),
                    recipient_name = table.Column<string>(type: "text", nullable: false),
                    recipient_address = table.Column<string>(type: "text", nullable: false),
                    weight = table.Column<float>(type: "real", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    length = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<ParcelStatus>(type: "enum_parcel_status", nullable: false, defaultValue: ParcelStatus.PendingPickup),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delivered_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    service_type = table.Column<ServiceType>(type: "enum_service_type", nullable: false),
                    declared_value_in_euros = table.Column<int>(type: "integer", nullable: false),
                    customer_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parcels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    granular = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    region_name = table.Column<string>(type: "text", nullable: false),
                    country_code = table.Column<string>(type: "text", nullable: false),
                    business_hours_start = table.Column<TimeOnly>(type: "time", nullable: false),
                    business_hours_end = table.Column<TimeOnly>(type: "time", nullable: false),
                    manager_email = table.Column<string>(type: "citext", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    business_days = table.Column<DayOfWeek[]>(type: "enum_day_of_week[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    can_access_all_regions = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sla_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rule_name = table.Column<string>(type: "text", nullable: false),
                    case_type = table.Column<CaseType>(type: "enum_case_type", nullable: true),
                    priority = table.Column<Priority>(type: "enum_priority", nullable: true),
                    service_type = table.Column<ServiceType>(type: "enum_service_type", nullable: true),
                    sla_hours = table.Column<int>(type: "integer", nullable: false),
                    is_business_hours = table.Column<bool>(type: "boolean", nullable: false),
                    escalation_after = table.Column<int>(type: "integer", nullable: false),
                    escalation_handler_id = table.Column<int>(type: "integer", nullable: false),
                    escalation_department = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sla_rules", x => x.id);
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
                name: "status_workflows",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    from_status = table.Column<CaseStatus>(type: "enum_case_status", nullable: true),
                    to_status = table.Column<CaseStatus>(type: "enum_case_status", nullable: true),
                    require_note = table.Column<bool>(type: "boolean", nullable: false),
                    require_resolution = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_status_workflows", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "system_configs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    config_key = table.Column<string>(type: "text", nullable: false),
                    config_value = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_configs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "citext", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cases",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    case_number = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    case_type = table.Column<CaseType>(type: "enum_case_type", nullable: false),
                    status = table.Column<CaseStatus>(type: "enum_case_status", nullable: false, defaultValue: CaseStatus.Open),
                    priority = table.Column<Priority>(type: "enum_priority", nullable: false, defaultValue: Priority.Low),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    handler_id = table.Column<int>(type: "integer", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolved_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sla_deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    region = table.Column<int>(type: "integer", nullable: false),
                    channel = table.Column<Channel>(type: "enum_channel", nullable: false),
                    escalated_to_id = table.Column<int>(type: "integer", nullable: false),
                    resolution = table.Column<string>(type: "text", nullable: false),
                    satisfaction_score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cases", x => x.id);
                    table.ForeignKey(
                        name: "fk_cases_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cases_handlers_escalated_to_id",
                        column: x => x.escalated_to_id,
                        principalTable: "handlers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cases_handlers_handler_id",
                        column: x => x.handler_id,
                        principalTable: "handlers",
                        principalColumn: "id");
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
                name: "user_permissions",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    permission_id = table.Column<int>(type: "integer", nullable: false),
                    expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_permissions", x => new { x.user_id, x.permission_id });
                    table.ForeignKey(
                        name: "fk_user_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_permissions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "ix_case_parcels_parcel_id",
                table: "case_parcels",
                column: "parcel_id");

            migrationBuilder.CreateIndex(
                name: "ix_case_tags_tag_id",
                table: "case_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_cases_customer_id",
                table: "cases",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_cases_escalated_to_id",
                table: "cases",
                column: "escalated_to_id");

            migrationBuilder.CreateIndex(
                name: "ix_cases_handler_id",
                table: "cases",
                column: "handler_id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_email",
                table: "customers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_email_templates_template_name_language_region_id",
                table: "email_templates",
                columns: new[] { "template_name", "language", "region_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_handlers_user_id",
                table: "handlers",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_holiday_regions_region_id",
                table: "holiday_regions",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_parcels_tracking_number",
                table: "parcels",
                column: "tracking_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_permissions_name",
                table: "permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_regions_region_name",
                table: "regions",
                column: "region_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_role_name",
                table: "roles",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_system_configs_config_key",
                table: "system_configs",
                column: "config_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_permissions_permission_id",
                table: "user_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_regions_region_id",
                table: "user_regions",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_created_by_id",
                table: "users",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "auto_assignment_rules");

            migrationBuilder.DropTable(
                name: "case_notes");

            migrationBuilder.DropTable(
                name: "case_parcels");

            migrationBuilder.DropTable(
                name: "case_tags");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropTable(
                name: "email_templates");

            migrationBuilder.DropTable(
                name: "holiday_regions");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "sla_rules");

            migrationBuilder.DropTable(
                name: "status_workflow_roles");

            migrationBuilder.DropTable(
                name: "status_workflows");

            migrationBuilder.DropTable(
                name: "system_configs");

            migrationBuilder.DropTable(
                name: "user_permissions");

            migrationBuilder.DropTable(
                name: "user_regions");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "parcels");

            migrationBuilder.DropTable(
                name: "cases");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "holidays");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "regions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "handlers");
        }
    }
}
