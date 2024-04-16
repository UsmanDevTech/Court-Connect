using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CouchingHubTableCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CouchingHub_CouchingHubCategory_CouchingHubCategoryId",
                table: "CouchingHub");

            migrationBuilder.DropForeignKey(
                name: "FK_CouchingHubDetail_CouchingHub_CouchingHubId",
                table: "CouchingHubDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CouchingHubDetail",
                table: "CouchingHubDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CouchingHubCategory",
                table: "CouchingHubCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CouchingHub",
                table: "CouchingHub");

            migrationBuilder.RenameTable(
                name: "CouchingHubDetail",
                newName: "CouchingHubDetails");

            migrationBuilder.RenameTable(
                name: "CouchingHubCategory",
                newName: "CouchingHubCategories");

            migrationBuilder.RenameTable(
                name: "CouchingHub",
                newName: "CouchingHubs");

            migrationBuilder.RenameIndex(
                name: "IX_CouchingHubDetail_CouchingHubId",
                table: "CouchingHubDetails",
                newName: "IX_CouchingHubDetails_CouchingHubId");

            migrationBuilder.RenameIndex(
                name: "IX_CouchingHub_CouchingHubCategoryId",
                table: "CouchingHubs",
                newName: "IX_CouchingHubs_CouchingHubCategoryId");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Leagues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CouchingHubDetails",
                table: "CouchingHubDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CouchingHubCategories",
                table: "CouchingHubCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CouchingHubs",
                table: "CouchingHubs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CouchingHubDetails_CouchingHubs_CouchingHubId",
                table: "CouchingHubDetails",
                column: "CouchingHubId",
                principalTable: "CouchingHubs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CouchingHubs_CouchingHubCategories_CouchingHubCategoryId",
                table: "CouchingHubs",
                column: "CouchingHubCategoryId",
                principalTable: "CouchingHubCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CouchingHubDetails_CouchingHubs_CouchingHubId",
                table: "CouchingHubDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CouchingHubs_CouchingHubCategories_CouchingHubCategoryId",
                table: "CouchingHubs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CouchingHubs",
                table: "CouchingHubs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CouchingHubDetails",
                table: "CouchingHubDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CouchingHubCategories",
                table: "CouchingHubCategories");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Leagues");

            migrationBuilder.RenameTable(
                name: "CouchingHubs",
                newName: "CouchingHub");

            migrationBuilder.RenameTable(
                name: "CouchingHubDetails",
                newName: "CouchingHubDetail");

            migrationBuilder.RenameTable(
                name: "CouchingHubCategories",
                newName: "CouchingHubCategory");

            migrationBuilder.RenameIndex(
                name: "IX_CouchingHubs_CouchingHubCategoryId",
                table: "CouchingHub",
                newName: "IX_CouchingHub_CouchingHubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CouchingHubDetails_CouchingHubId",
                table: "CouchingHubDetail",
                newName: "IX_CouchingHubDetail_CouchingHubId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CouchingHub",
                table: "CouchingHub",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CouchingHubDetail",
                table: "CouchingHubDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CouchingHubCategory",
                table: "CouchingHubCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CouchingHub_CouchingHubCategory_CouchingHubCategoryId",
                table: "CouchingHub",
                column: "CouchingHubCategoryId",
                principalTable: "CouchingHubCategory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CouchingHubDetail_CouchingHub_CouchingHubId",
                table: "CouchingHubDetail",
                column: "CouchingHubId",
                principalTable: "CouchingHub",
                principalColumn: "Id");
        }
    }
}
