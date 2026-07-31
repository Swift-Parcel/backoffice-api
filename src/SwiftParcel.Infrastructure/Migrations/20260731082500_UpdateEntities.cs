using System;
using Microsoft.EntityFrameworkCore.Migrations;
using SwiftParcel.Domain.Enums;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "declared_value_in_euros",
                table: "parcels",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "preferred_pickup_date",
                table: "parcels",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Timeslot>(
                name: "preferred_pickup_timeslot",
                table: "parcels",
                type: "enum_timeslot",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "preferred_pickup_date",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "preferred_pickup_timeslot",
                table: "parcels");

            migrationBuilder.AlterColumn<int>(
                name: "declared_value_in_euros",
                table: "parcels",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
