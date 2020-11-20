using Microsoft.EntityFrameworkCore.Migrations;

namespace WgWebsite.Migrations
{
    public partial class pk_change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Users_UserId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_UserId",
                table: "Todos");

            migrationBuilder.AddColumn<long>(
                name: "UserId1",
                table: "Todos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Todos_UserId1",
                table: "Todos",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Users_UserId1",
                table: "Todos",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Users_UserId1",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_UserId1",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Todos");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_UserId",
                table: "Todos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Users_UserId",
                table: "Todos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
