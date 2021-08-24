using Microsoft.EntityFrameworkCore.Migrations;

namespace Phrook.Migrations
{
    public partial class IsVisibleUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visibility",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "AspNetUsers");
        }
    }
}
