using Microsoft.EntityFrameworkCore.Migrations;

namespace WgWebsite.Migrations
{
    public partial class linkedbalanceentry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceEntries_KarmaBalances_KarmaBalanceId",
                table: "BalanceEntries");

            migrationBuilder.AlterColumn<long>(
                name: "KarmaBalanceId",
                table: "BalanceEntries",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceEntries_KarmaBalances_KarmaBalanceId",
                table: "BalanceEntries",
                column: "KarmaBalanceId",
                principalTable: "KarmaBalances",
                principalColumn: "KarmaBalanceId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceEntries_KarmaBalances_KarmaBalanceId",
                table: "BalanceEntries");

            migrationBuilder.AlterColumn<long>(
                name: "KarmaBalanceId",
                table: "BalanceEntries",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceEntries_KarmaBalances_KarmaBalanceId",
                table: "BalanceEntries",
                column: "KarmaBalanceId",
                principalTable: "KarmaBalances",
                principalColumn: "KarmaBalanceId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
