using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MatchMemberConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchMembers_TennisMatches_TennisMatchId",
                table: "MatchMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchMembers_TennisMatches_TennisMatchId",
                table: "MatchMembers",
                column: "TennisMatchId",
                principalTable: "TennisMatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchMembers_TennisMatches_TennisMatchId",
                table: "MatchMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchMembers_TennisMatches_TennisMatchId",
                table: "MatchMembers",
                column: "TennisMatchId",
                principalTable: "TennisMatches",
                principalColumn: "Id");
        }
    }
}
