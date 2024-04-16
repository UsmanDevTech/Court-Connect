using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsStatsAvailable",
                table: "Subscriptions",
                newName: "IsRatingAvailable");

            migrationBuilder.RenameColumn(
                name: "IsStatsAvailable",
                table: "SubscriptionHistories",
                newName: "IsRatingAvailable");

            migrationBuilder.AddColumn<bool>(
                name: "IsMatchBalanceAvailable",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMatchBalanceAvailable",
                table: "SubscriptionHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMatchBalanceAvailable",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsMatchBalanceAvailable",
                table: "SubscriptionHistories");

            migrationBuilder.RenameColumn(
                name: "IsRatingAvailable",
                table: "Subscriptions",
                newName: "IsStatsAvailable");

            migrationBuilder.RenameColumn(
                name: "IsRatingAvailable",
                table: "SubscriptionHistories",
                newName: "IsStatsAvailable");
        }
    }
}
