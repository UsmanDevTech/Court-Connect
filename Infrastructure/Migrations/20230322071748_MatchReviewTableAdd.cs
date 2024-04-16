using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MatchReviewTableAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeRankedGamesPerMonth",
                table: "SubscriptionHistories");

            migrationBuilder.DropColumn(
                name: "FreeUnrankedGamesPerMonth",
                table: "SubscriptionHistories");

            migrationBuilder.DropColumn(
                name: "RemainingRankedMatchesPerMonth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RemainingUnrankedMatchesPerMonth",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UnrankedGamesPerMonth",
                table: "Subscriptions",
                newName: "UnrankedGames");

            migrationBuilder.RenameColumn(
                name: "RankedGamesPerMonth",
                table: "Subscriptions",
                newName: "RankedGames");

            migrationBuilder.RenameColumn(
                name: "FreeUnrankedGamesPerMonth",
                table: "Subscriptions",
                newName: "FreeUnrankedGames");

            migrationBuilder.RenameColumn(
                name: "FreeRankedGamesPerMonth",
                table: "Subscriptions",
                newName: "FreeRankedGames");

            migrationBuilder.RenameColumn(
                name: "UnrankedGamesPerMonth",
                table: "SubscriptionHistories",
                newName: "FreeUnrankedGames");

            migrationBuilder.RenameColumn(
                name: "RankedGamesPerMonth",
                table: "SubscriptionHistories",
                newName: "FreeRankedGames");

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeRankedUnlimited",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeUnrankedUnlimited",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRankedUnlimited",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnrankedUnlimited",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeRankedUnlimited",
                table: "SubscriptionHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeUnrankedUnlimited",
                table: "SubscriptionHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GamePoint",
                table: "MatchMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMatchWon",
                table: "MatchMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewPoints",
                table: "MatchMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreviousPoint",
                table: "MatchMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetOnePoint",
                table: "MatchMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetThreePoint",
                table: "MatchMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetTwoPoint",
                table: "MatchMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamMemberId",
                table: "MatchMembers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeRankedUnlimited",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeUnrankedUnlimited",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MatchReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewedTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Forehand = table.Column<double>(type: "float", nullable: false),
                    Backhand = table.Column<double>(type: "float", nullable: false),
                    Serve = table.Column<double>(type: "float", nullable: false),
                    Fairness = table.Column<double>(type: "float", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TennisMatchId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchReviews_TennisMatches_TennisMatchId",
                        column: x => x.TennisMatchId,
                        principalTable: "TennisMatches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchReviews_TennisMatchId",
                table: "MatchReviews",
                column: "TennisMatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchReviews");

            migrationBuilder.DropColumn(
                name: "IsFreeRankedUnlimited",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsFreeUnrankedUnlimited",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsRankedUnlimited",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsUnrankedUnlimited",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsFreeRankedUnlimited",
                table: "SubscriptionHistories");

            migrationBuilder.DropColumn(
                name: "IsFreeUnrankedUnlimited",
                table: "SubscriptionHistories");

            migrationBuilder.DropColumn(
                name: "GamePoint",
                table: "MatchMembers");

            migrationBuilder.DropColumn(
                name: "IsMatchWon",
                table: "MatchMembers");

            migrationBuilder.DropColumn(
                name: "NewPoints",
                table: "MatchMembers");

            migrationBuilder.DropColumn(
                name: "PreviousPoint",
                table: "MatchMembers");

            migrationBuilder.DropColumn(
                name: "SetOnePoint",
                table: "MatchMembers");

            migrationBuilder.DropColumn(
                name: "SetThreePoint",
                table: "MatchMembers");

            migrationBuilder.DropColumn(
                name: "SetTwoPoint",
                table: "MatchMembers");

            migrationBuilder.DropColumn(
                name: "TeamMemberId",
                table: "MatchMembers");

            migrationBuilder.DropColumn(
                name: "IsFreeRankedUnlimited",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsFreeUnrankedUnlimited",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionExpiry",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UnrankedGames",
                table: "Subscriptions",
                newName: "UnrankedGamesPerMonth");

            migrationBuilder.RenameColumn(
                name: "RankedGames",
                table: "Subscriptions",
                newName: "RankedGamesPerMonth");

            migrationBuilder.RenameColumn(
                name: "FreeUnrankedGames",
                table: "Subscriptions",
                newName: "FreeUnrankedGamesPerMonth");

            migrationBuilder.RenameColumn(
                name: "FreeRankedGames",
                table: "Subscriptions",
                newName: "FreeRankedGamesPerMonth");

            migrationBuilder.RenameColumn(
                name: "FreeUnrankedGames",
                table: "SubscriptionHistories",
                newName: "UnrankedGamesPerMonth");

            migrationBuilder.RenameColumn(
                name: "FreeRankedGames",
                table: "SubscriptionHistories",
                newName: "RankedGamesPerMonth");

            migrationBuilder.AddColumn<int>(
                name: "FreeRankedGamesPerMonth",
                table: "SubscriptionHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FreeUnrankedGamesPerMonth",
                table: "SubscriptionHistories",
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
        }
    }
}
