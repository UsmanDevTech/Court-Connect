using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CouchingHubTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "CouchingHubDetails");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CouchingHubDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "CouchingHubDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CouchingHubDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "CouchingHubDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CouchingHubDetails");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "CouchingHubDetails");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "CouchingHubDetails");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "CouchingHubDetails");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "CouchingHubDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
