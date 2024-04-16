using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChatConversationAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CouchingHubs_CouchingHubCategories_CouchingHubCategoryId",
                table: "CouchingHubs");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueRewards_Leagues_LeagueId",
                table: "LeagueRewards");

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewsAvailable",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsScoreAvailable",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStatsAvailable",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewsAvailable",
                table: "SubscriptionHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsScoreAvailable",
                table: "SubscriptionHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStatsAvailable",
                table: "SubscriptionHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TennisMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MatchImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MatchCategory = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<Point>(type: "geography", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatchDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsMembersLimitFull = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TennisMatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatHeads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TennisMatchId = table.Column<int>(type: "int", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatHeads_TennisMatches_TennisMatchId",
                        column: x => x.TennisMatchId,
                        principalTable: "TennisMatches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MatchMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TennisMatchId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchMembers_TennisMatches_TennisMatchId",
                        column: x => x.TennisMatchId,
                        principalTable: "TennisMatches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatConversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadStatus = table.Column<bool>(type: "bit", nullable: false),
                    Latitute = table.Column<double>(type: "float", nullable: true),
                    Longitue = table.Column<double>(type: "float", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageType = table.Column<int>(type: "int", nullable: false),
                    ChatHeadId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatConversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatConversations_ChatHeads_ChatHeadId",
                        column: x => x.ChatHeadId,
                        principalTable: "ChatHeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsChatHeadDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ChatHeadId = table.Column<int>(type: "int", nullable: false),
                    ChatHeadDeleteLastMsgId = table.Column<int>(type: "int", nullable: true),
                    LastMsgSeenId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMembers_ChatConversations_ChatHeadDeleteLastMsgId",
                        column: x => x.ChatHeadDeleteLastMsgId,
                        principalTable: "ChatConversations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMembers_ChatConversations_LastMsgSeenId",
                        column: x => x.LastMsgSeenId,
                        principalTable: "ChatConversations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMembers_ChatHeads_ChatHeadId",
                        column: x => x.ChatHeadId,
                        principalTable: "ChatHeads",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatConversations_ChatHeadId",
                table: "ChatConversations",
                column: "ChatHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatHeads_TennisMatchId",
                table: "ChatHeads",
                column: "TennisMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_ChatHeadDeleteLastMsgId",
                table: "ChatMembers",
                column: "ChatHeadDeleteLastMsgId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_ChatHeadId",
                table: "ChatMembers",
                column: "ChatHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_LastMsgSeenId",
                table: "ChatMembers",
                column: "LastMsgSeenId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchMembers_TennisMatchId",
                table: "MatchMembers",
                column: "TennisMatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_CouchingHubs_CouchingHubCategories_CouchingHubCategoryId",
                table: "CouchingHubs",
                column: "CouchingHubCategoryId",
                principalTable: "CouchingHubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueRewards_Leagues_LeagueId",
                table: "LeagueRewards",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CouchingHubs_CouchingHubCategories_CouchingHubCategoryId",
                table: "CouchingHubs");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueRewards_Leagues_LeagueId",
                table: "LeagueRewards");

            migrationBuilder.DropTable(
                name: "ChatMembers");

            migrationBuilder.DropTable(
                name: "MatchMembers");

            migrationBuilder.DropTable(
                name: "ChatConversations");

            migrationBuilder.DropTable(
                name: "ChatHeads");

            migrationBuilder.DropTable(
                name: "TennisMatches");

            migrationBuilder.DropColumn(
                name: "IsReviewsAvailable",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsScoreAvailable",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsStatsAvailable",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsReviewsAvailable",
                table: "SubscriptionHistories");

            migrationBuilder.DropColumn(
                name: "IsScoreAvailable",
                table: "SubscriptionHistories");

            migrationBuilder.DropColumn(
                name: "IsStatsAvailable",
                table: "SubscriptionHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_CouchingHubs_CouchingHubCategories_CouchingHubCategoryId",
                table: "CouchingHubs",
                column: "CouchingHubCategoryId",
                principalTable: "CouchingHubCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueRewards_Leagues_LeagueId",
                table: "LeagueRewards",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id");
        }
    }
}
