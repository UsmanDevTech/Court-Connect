using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatingCouchingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingFreeRankedMatches",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingFreeUnrankedMatches",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingRankedMatchesPerMonth",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingUnrankedMatchesPerMonth",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CouchingHubCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouchingHubCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<double>(type: "float", nullable: false),
                    PriceAfterDiscount = table.Column<double>(type: "float", nullable: false),
                    IsDiscountAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: true),
                    RankedGamesPerMonth = table.Column<int>(type: "int", nullable: true),
                    UnrankedGamesPerMonth = table.Column<int>(type: "int", nullable: true),
                    FreeRankedGamesPerMonth = table.Column<int>(type: "int", nullable: true),
                    FreeUnrankedGamesPerMonth = table.Column<int>(type: "int", nullable: true),
                    CostPerRankedGame = table.Column<double>(type: "float", nullable: true),
                    CostPerUnrankedGame = table.Column<double>(type: "float", nullable: true),
                    IsFreeCouchingContentAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsPaidCouchingContentAvailable = table.Column<bool>(type: "bit", nullable: false),
                    SubscriptionStatus = table.Column<int>(type: "int", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tax = table.Column<double>(type: "float", nullable: false),
                    StripeFee = table.Column<double>(type: "float", nullable: true),
                    PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionHistories_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CouchingHub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: true),
                    CouchingHubCategoryId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouchingHub", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouchingHub_CouchingHubCategory_CouchingHubCategoryId",
                        column: x => x.CouchingHubCategoryId,
                        principalTable: "CouchingHubCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CouchingHubDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeLength = table.Column<long>(type: "bigint", nullable: true),
                    CouchingHubId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouchingHubDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouchingHubDetail_CouchingHub_CouchingHubId",
                        column: x => x.CouchingHubId,
                        principalTable: "CouchingHub",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CouchingHub_CouchingHubCategoryId",
                table: "CouchingHub",
                column: "CouchingHubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CouchingHubDetail_CouchingHubId",
                table: "CouchingHubDetail",
                column: "CouchingHubId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionHistories_SubscriptionId",
                table: "SubscriptionHistories",
                column: "SubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CouchingHubDetail");

            migrationBuilder.DropTable(
                name: "SubscriptionHistories");

            migrationBuilder.DropTable(
                name: "CouchingHub");

            migrationBuilder.DropTable(
                name: "CouchingHubCategory");

            migrationBuilder.DropColumn(
                name: "RemainingFreeRankedMatches",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RemainingFreeUnrankedMatches",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RemainingRankedMatchesPerMonth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RemainingUnrankedMatchesPerMonth",
                table: "AspNetUsers");
        }
    }
}
