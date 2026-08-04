using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRoleName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "role_name",
                table: "roles",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "ix_roles_role_name",
                table: "roles",
                newName: "ix_roles_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "roles",
                newName: "role_name");

            migrationBuilder.RenameIndex(
                name: "ix_roles_name",
                table: "roles",
                newName: "ix_roles_role_name");
        }
    }
}
