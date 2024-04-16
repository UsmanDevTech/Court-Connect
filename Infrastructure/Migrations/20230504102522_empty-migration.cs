using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class emptymigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemporaryMatchScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamAScoreOne = table.Column<int>(type: "int", nullable: true),
                    TeamAScoreTwo = table.Column<int>(type: "int", nullable: true),
                    TeamAScoreThree = table.Column<int>(type: "int", nullable: true),
                    TeamBScoreOne = table.Column<int>(type: "int", nullable: true),
                    TeamBScoreTwo = table.Column<int>(type: "int", nullable: true),
                    TeamBScoreThree = table.Column<int>(type: "int", nullable: true),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporaryMatchScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemporaryMatchScores_TennisMatches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "TennisMatches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemporaryMatchScores_MatchId",
                table: "TemporaryMatchScores",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemporaryMatchScores");
        }
    }
}
