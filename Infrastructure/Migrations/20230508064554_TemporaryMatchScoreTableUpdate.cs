using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TemporaryMatchScoreTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryMatchScores_TennisMatches_MatchId",
                table: "TemporaryMatchScores");

            migrationBuilder.RenameColumn(
                name: "MatchId",
                table: "TemporaryMatchScores",
                newName: "TennisMatchId");

            migrationBuilder.RenameIndex(
                name: "IX_TemporaryMatchScores_MatchId",
                table: "TemporaryMatchScores",
                newName: "IX_TemporaryMatchScores_TennisMatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryMatchScores_TennisMatches_TennisMatchId",
                table: "TemporaryMatchScores",
                column: "TennisMatchId",
                principalTable: "TennisMatches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryMatchScores_TennisMatches_TennisMatchId",
                table: "TemporaryMatchScores");

            migrationBuilder.RenameColumn(
                name: "TennisMatchId",
                table: "TemporaryMatchScores",
                newName: "MatchId");

            migrationBuilder.RenameIndex(
                name: "IX_TemporaryMatchScores_TennisMatchId",
                table: "TemporaryMatchScores",
                newName: "IX_TemporaryMatchScores_MatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryMatchScores_TennisMatches_MatchId",
                table: "TemporaryMatchScores",
                column: "MatchId",
                principalTable: "TennisMatches",
                principalColumn: "Id");
        }
    }
}
