using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeHandlers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "handlers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "handlers");
        }
    }
}
