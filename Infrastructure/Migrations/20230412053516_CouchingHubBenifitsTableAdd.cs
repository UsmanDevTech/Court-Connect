using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CouchingHubBenifitsTableAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CouchingHubBenifits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CouchingHubId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouchingHubBenifits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouchingHubBenifits_CouchingHubs_CouchingHubId",
                        column: x => x.CouchingHubId,
                        principalTable: "CouchingHubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CouchingHubBenifits_CouchingHubId",
                table: "CouchingHubBenifits",
                column: "CouchingHubId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CouchingHubBenifits");
        }
    }
}
