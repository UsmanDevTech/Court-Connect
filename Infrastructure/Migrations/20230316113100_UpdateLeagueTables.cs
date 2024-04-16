using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLeagueTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeagueRewards_Leagues_LeagueId",
                table: "LeagueRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_Leagues_LeagueId",
                table: "UserSettings");

            migrationBuilder.RenameColumn(
                name: "LeagueId",
                table: "UserSettings",
                newName: "SubLeagueId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSettings_LeagueId",
                table: "UserSettings",
                newName: "IX_UserSettings_SubLeagueId");

            migrationBuilder.RenameColumn(
                name: "PointsEqualAbove",
                table: "Leagues",
                newName: "MinRange");

            migrationBuilder.RenameColumn(
                name: "LeagueId",
                table: "LeagueRewards",
                newName: "SubLeagueId");

            migrationBuilder.RenameIndex(
                name: "IX_LeagueRewards_LeagueId",
                table: "LeagueRewards",
                newName: "IX_LeagueRewards_SubLeagueId");

            migrationBuilder.AddColumn<int>(
                name: "MaxRange",
                table: "Leagues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SubLeagues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinRange = table.Column<int>(type: "int", nullable: false),
                    MaxRange = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    LeagueId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubLeagues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubLeagues_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubLeagues_LeagueId",
                table: "SubLeagues",
                column: "LeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueRewards_SubLeagues_SubLeagueId",
                table: "LeagueRewards",
                column: "SubLeagueId",
                principalTable: "SubLeagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_SubLeagues_SubLeagueId",
                table: "UserSettings",
                column: "SubLeagueId",
                principalTable: "SubLeagues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeagueRewards_SubLeagues_SubLeagueId",
                table: "LeagueRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_SubLeagues_SubLeagueId",
                table: "UserSettings");

            migrationBuilder.DropTable(
                name: "SubLeagues");

            migrationBuilder.DropColumn(
                name: "MaxRange",
                table: "Leagues");

            migrationBuilder.RenameColumn(
                name: "SubLeagueId",
                table: "UserSettings",
                newName: "LeagueId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSettings_SubLeagueId",
                table: "UserSettings",
                newName: "IX_UserSettings_LeagueId");

            migrationBuilder.RenameColumn(
                name: "MinRange",
                table: "Leagues",
                newName: "PointsEqualAbove");

            migrationBuilder.RenameColumn(
                name: "SubLeagueId",
                table: "LeagueRewards",
                newName: "LeagueId");

            migrationBuilder.RenameIndex(
                name: "IX_LeagueRewards_SubLeagueId",
                table: "LeagueRewards",
                newName: "IX_LeagueRewards_LeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueRewards_Leagues_LeagueId",
                table: "LeagueRewards",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_Leagues_LeagueId",
                table: "UserSettings",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id");
        }
    }
}
