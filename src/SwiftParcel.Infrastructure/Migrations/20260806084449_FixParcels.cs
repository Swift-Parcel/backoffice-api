using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftParcel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixParcels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sender_City",
                table: "parcels",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sender_CountryCode",
                table: "parcels",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sender_PostalCode",
                table: "parcels",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sender_Street",
                table: "parcels",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sender_StreetNumber",
                table: "parcels",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sender_City",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "Sender_CountryCode",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "Sender_PostalCode",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "Sender_Street",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "Sender_StreetNumber",
                table: "parcels");
        }
    }
}
