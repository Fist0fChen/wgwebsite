using Microsoft.EntityFrameworkCore.Migrations;

namespace WgWebsite.Migrations
{
    public partial class acknowledgebalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Acknowledged",
                table: "KarmaBalances",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acknowledged",
                table: "KarmaBalances");
        }
    }
}
