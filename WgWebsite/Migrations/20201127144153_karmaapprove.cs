using Microsoft.EntityFrameworkCore.Migrations;

namespace WgWebsite.Migrations
{
    public partial class karmaapprove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "TasksDone",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "TasksDone");
        }
    }
}
