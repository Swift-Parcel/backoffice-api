using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCaseNoteHandlerNavProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_case_notes_users_handler_id",
                table: "case_notes");

            migrationBuilder.AddForeignKey(
                name: "fk_case_notes_handlers_handler_id",
                table: "case_notes",
                column: "handler_id",
                principalTable: "handlers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_case_notes_handlers_handler_id",
                table: "case_notes");

            migrationBuilder.AddForeignKey(
                name: "fk_case_notes_users_handler_id",
                table: "case_notes",
                column: "handler_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
