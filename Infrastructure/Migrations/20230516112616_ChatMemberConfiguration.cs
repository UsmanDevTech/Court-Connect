using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChatMemberConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMembers_ChatHeads_ChatHeadId",
                table: "ChatMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMembers_ChatHeads_ChatHeadId",
                table: "ChatMembers",
                column: "ChatHeadId",
                principalTable: "ChatHeads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMembers_ChatHeads_ChatHeadId",
                table: "ChatMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMembers_ChatHeads_ChatHeadId",
                table: "ChatMembers",
                column: "ChatHeadId",
                principalTable: "ChatHeads",
                principalColumn: "Id");
        }
    }
}
