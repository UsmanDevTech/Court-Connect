using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePurchasedClassesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchasedClassess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<double>(type: "float", nullable: false),
                    PriceAfterDiscount = table.Column<double>(type: "float", nullable: false),
                    IsDiscountAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: true),
                    StripeFee = table.Column<double>(type: "float", nullable: true),
                    PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CouchingHubId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasedClassess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasedClassess_CouchingHubs_CouchingHubId",
                        column: x => x.CouchingHubId,
                        principalTable: "CouchingHubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchasedClassess_CouchingHubId",
                table: "PurchasedClassess",
                column: "CouchingHubId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchasedClassess");
        }
    }
}
