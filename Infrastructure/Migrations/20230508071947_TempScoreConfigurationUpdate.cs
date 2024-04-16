using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TempScoreConfigurationUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryMatchScores_TennisMatches_TennisMatchId",
                table: "TemporaryMatchScores");

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryMatchScores_TennisMatches_TennisMatchId",
                table: "TemporaryMatchScores",
                column: "TennisMatchId",
                principalTable: "TennisMatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryMatchScores_TennisMatches_TennisMatchId",
                table: "TemporaryMatchScores");

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryMatchScores_TennisMatches_TennisMatchId",
                table: "TemporaryMatchScores",
                column: "TennisMatchId",
                principalTable: "TennisMatches",
                principalColumn: "Id");
        }
    }
}
