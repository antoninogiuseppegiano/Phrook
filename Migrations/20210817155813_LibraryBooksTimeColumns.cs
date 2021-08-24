using Microsoft.EntityFrameworkCore.Migrations;

namespace Phrook.Migrations
{
    public partial class LibraryBooksTimeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinalTime",
                table: "LibraryBooks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InitialTime",
                table: "LibraryBooks",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalTime",
                table: "LibraryBooks");

            migrationBuilder.DropColumn(
                name: "InitialTime",
                table: "LibraryBooks");
        }
    }
}
