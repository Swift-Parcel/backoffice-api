using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_case_notes_users_author_id",
                table: "case_notes");

            migrationBuilder.DropIndex(
                name: "ix_case_notes_author_id",
                table: "case_notes");

            migrationBuilder.DropColumn(
                name: "author_id",
                table: "case_notes");

            migrationBuilder.AddColumn<int>(
                name: "customer_id",
                table: "case_notes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "handler_id",
                table: "case_notes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_case_notes_customer_id",
                table: "case_notes",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_case_notes_handler_id",
                table: "case_notes",
                column: "handler_id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CaseNote_Author",
                table: "case_notes",
                sql: "(handler_id IS NOT NULL AND customer_id IS NULL) OR (handler_id IS NULL AND customer_id IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "fk_case_notes_customers_customer_id",
                table: "case_notes",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_case_notes_users_handler_id",
                table: "case_notes",
                column: "handler_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_case_notes_customers_customer_id",
                table: "case_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_case_notes_users_handler_id",
                table: "case_notes");

            migrationBuilder.DropIndex(
                name: "ix_case_notes_customer_id",
                table: "case_notes");

            migrationBuilder.DropIndex(
                name: "ix_case_notes_handler_id",
                table: "case_notes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CaseNote_Author",
                table: "case_notes");

            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "case_notes");

            migrationBuilder.DropColumn(
                name: "handler_id",
                table: "case_notes");

            migrationBuilder.AddColumn<int>(
                name: "author_id",
                table: "case_notes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_case_notes_author_id",
                table: "case_notes",
                column: "author_id");

            migrationBuilder.AddForeignKey(
                name: "fk_case_notes_users_author_id",
                table: "case_notes",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
