using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCustomerAndCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "case_number_seq",
                startValue: 1000L);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_date",
                table: "cases",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "case_number_seq");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_date",
                table: "cases",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
