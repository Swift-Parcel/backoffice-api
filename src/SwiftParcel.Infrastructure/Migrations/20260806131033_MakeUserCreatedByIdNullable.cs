using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserCreatedByIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_users_created_by_id",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "created_by_id",
                table: "users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_users_users_created_by_id",
                table: "users",
                column: "created_by_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_users_created_by_id",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "created_by_id",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_users_users_created_by_id",
                table: "users",
                column: "created_by_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
