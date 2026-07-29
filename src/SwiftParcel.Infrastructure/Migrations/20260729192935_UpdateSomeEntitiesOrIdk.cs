using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSomeEntitiesOrIdk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:enum_action", "assign,create,delete,escalate,login_failed,login_succeeded,note_add,permission_grant,role_change,status_change,update")
                .Annotation("Npgsql:Enum:enum_action.audit_action", "create,status_change,assign,update,permission_grant,login_failed,login_succeeded,role_change,delete,escalate,note_add")
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
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:Enum:enum_action", "assign,create,delete,escalate,login_failed,login_succeeded,node_add,permission_grant,role_change,status_change,update")
                .OldAnnotation("Npgsql:Enum:enum_action.audit_action", "create,status_change,assign,update,permission_grant,login_failed,login_succeeded,role_change,delete,escalate,node_add")
                .OldAnnotation("Npgsql:Enum:enum_case_status", "awaiting_customer,cancelled,closed,escalated,in_progress,open,resolved")
                .OldAnnotation("Npgsql:Enum:enum_case_status.case_status", "open,in_progress,awaiting_customer,resolved,closed,escalated,cancelled")
                .OldAnnotation("Npgsql:Enum:enum_case_type", "billing,damaged,delayed,delivery_change,lost,other,wrong_address")
                .OldAnnotation("Npgsql:Enum:enum_case_type.case_type", "lost,damaged,delayed,wrong_address,billing,delivery_change,other")
                .OldAnnotation("Npgsql:Enum:enum_channel", "chat,email,phone,portal")
                .OldAnnotation("Npgsql:Enum:enum_channel.channel", "email,phone,chat,portal")
                .OldAnnotation("Npgsql:Enum:enum_day_of_week", "friday,monday,saturday,sunday,thursday,tuesday,wednesday")
                .OldAnnotation("Npgsql:Enum:enum_day_of_week.day_of_week", "sunday,monday,tuesday,wednesday,thursday,friday,saturday")
                .OldAnnotation("Npgsql:Enum:enum_entity_type", "auto_assignment_rule,case,customer,email_template,handler,holiday,note,parcel,region,role,sla_rule,status_workflow,system_config,user,user_permission")
                .OldAnnotation("Npgsql:Enum:enum_entity_type.entity_type", "auto_assignment_rule,note,case,customer,email_template,handler,holiday,parcel,region,role,sla_rule,status_workflow,system_config,user_permission,user")
                .OldAnnotation("Npgsql:Enum:enum_parcel_status", "damaged,delivered,delivery_attempt_failed,in_transit,lost,out_for_delivery,pending_pickup,picked_up")
                .OldAnnotation("Npgsql:Enum:enum_parcel_status.parcel_status", "pending_pickup,picked_up,in_transit,out_for_delivery,delivered,delivery_attempt_failed,lost,damaged")
                .OldAnnotation("Npgsql:Enum:enum_priority", "critical,high,low,medium")
                .OldAnnotation("Npgsql:Enum:enum_priority.priority", "low,medium,high,critical")
                .OldAnnotation("Npgsql:Enum:enum_service_type", "express,same_day,standard")
                .OldAnnotation("Npgsql:Enum:enum_service_type.service_type", "standard,express,same_day")
                .OldAnnotation("Npgsql:Enum:enum_timeslot", "afternoon,evening,morning")
                .OldAnnotation("Npgsql:Enum:enum_timeslot.timeslot", "morning,afternoon,evening")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.AlterColumn<DateTime>(
                name: "expires",
                table: "user_permissions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:Enum:enum_action", "assign,create,delete,escalate,login_failed,login_succeeded,note_add,permission_grant,role_change,status_change,update")
                .OldAnnotation("Npgsql:Enum:enum_action.audit_action", "create,status_change,assign,update,permission_grant,login_failed,login_succeeded,role_change,delete,escalate,note_add")
                .OldAnnotation("Npgsql:Enum:enum_case_status", "awaiting_customer,cancelled,closed,escalated,in_progress,open,resolved")
                .OldAnnotation("Npgsql:Enum:enum_case_status.case_status", "open,in_progress,awaiting_customer,resolved,closed,escalated,cancelled")
                .OldAnnotation("Npgsql:Enum:enum_case_type", "billing,damaged,delayed,delivery_change,lost,other,wrong_address")
                .OldAnnotation("Npgsql:Enum:enum_case_type.case_type", "lost,damaged,delayed,wrong_address,billing,delivery_change,other")
                .OldAnnotation("Npgsql:Enum:enum_channel", "chat,email,phone,portal")
                .OldAnnotation("Npgsql:Enum:enum_channel.channel", "email,phone,chat,portal")
                .OldAnnotation("Npgsql:Enum:enum_day_of_week", "friday,monday,saturday,sunday,thursday,tuesday,wednesday")
                .OldAnnotation("Npgsql:Enum:enum_day_of_week.day_of_week", "sunday,monday,tuesday,wednesday,thursday,friday,saturday")
                .OldAnnotation("Npgsql:Enum:enum_entity_type", "auto_assignment_rule,case,customer,email_template,handler,holiday,note,parcel,region,role,sla_rule,status_workflow,system_config,user,user_permission")
                .OldAnnotation("Npgsql:Enum:enum_entity_type.entity_type", "auto_assignment_rule,note,case,customer,email_template,handler,holiday,parcel,region,role,sla_rule,status_workflow,system_config,user_permission,user")
                .OldAnnotation("Npgsql:Enum:enum_parcel_status", "damaged,delivered,delivery_attempt_failed,in_transit,lost,out_for_delivery,pending_pickup,picked_up")
                .OldAnnotation("Npgsql:Enum:enum_parcel_status.parcel_status", "pending_pickup,picked_up,in_transit,out_for_delivery,delivered,delivery_attempt_failed,lost,damaged")
                .OldAnnotation("Npgsql:Enum:enum_priority", "critical,high,low,medium")
                .OldAnnotation("Npgsql:Enum:enum_priority.priority", "low,medium,high,critical")
                .OldAnnotation("Npgsql:Enum:enum_service_type", "express,same_day,standard")
                .OldAnnotation("Npgsql:Enum:enum_service_type.service_type", "standard,express,same_day")
                .OldAnnotation("Npgsql:Enum:enum_timeslot", "afternoon,evening,morning")
                .OldAnnotation("Npgsql:Enum:enum_timeslot.timeslot", "morning,afternoon,evening")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.AlterColumn<DateTime>(
                name: "expires",
                table: "user_permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
