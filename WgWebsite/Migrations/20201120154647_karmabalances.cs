using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace WgWebsite.Migrations
{
    public partial class karmabalances : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KarmaBalances",
                columns: table => new
                {
                    KarmaBalanceId = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BalanceFrom = table.Column<DateTime>(nullable: false),
                    BalanceTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KarmaBalances", x => x.KarmaBalanceId);
                });

            migrationBuilder.CreateTable(
                name: "BalanceEntries",
                columns: table => new
                {
                    KarmaBalanceEntryId = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    Karma = table.Column<long>(nullable: false),
                    KarmaBalanceId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceEntries", x => x.KarmaBalanceEntryId);
                    table.ForeignKey(
                        name: "FK_BalanceEntries_KarmaBalances_KarmaBalanceId",
                        column: x => x.KarmaBalanceId,
                        principalTable: "KarmaBalances",
                        principalColumn: "KarmaBalanceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BalanceEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceEntries_KarmaBalanceId",
                table: "BalanceEntries",
                column: "KarmaBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceEntries_UserId",
                table: "BalanceEntries",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceEntries");

            migrationBuilder.DropTable(
                name: "KarmaBalances");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Users");
        }
    }
}
