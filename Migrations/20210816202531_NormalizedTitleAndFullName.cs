using Microsoft.EntityFrameworkCore.Migrations;

namespace Phrook.Migrations
{
    public partial class NormalizedTitleAndFullName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isVisible",
                table: "AspNetUsers",
                newName: "Visibility");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedTitle",
                table: "Books",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedFullName",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

			//Annullato
			// migrationBuilder.Sql(@"CREATE TRIGGER BooksSetNormalizedTitleOnInsert
            //                        AFTER INSERT ON Books
            //                        BEGIN
            //                        UPDATE Books SET NormalizedTitle = lower(Title) WHERE Id=NEW.Id;
            //                        END;");
            // migrationBuilder.Sql(@"CREATE TRIGGER BooksSetNormalizedTitleOnUpdate
            //                        AFTER UPDATE ON Books
            //                        BEGIN
            //                        UPDATE Books SET NormalizedTitle = lower(Title) WHERE Id=NEW.Id;
            //                        END;");
            // migrationBuilder.Sql("UPDATE Books SET NormalizedTitle = lower(Title);");

			// migrationBuilder.Sql(@"CREATE TRIGGER AspNetUsersSetNormalizedFullNameOnInsert
            //                        AFTER INSERT ON AspNetUsers
            //                        BEGIN
            //                        UPDATE AspNetUsers SET NormalizedFullName = lower(FullName) WHERE Id=NEW.Id;
            //                        END;");
            // migrationBuilder.Sql(@"CREATE TRIGGER AspNetUsersSetNormalizedFullNameOnUpdate
            //                        AFTER UPDATE ON AspNetUsers
            //                        BEGIN
            //                        UPDATE AspNetUsers SET NormalizedFullName = lower(FullName) WHERE Id=NEW.Id;
            //                        END;");
            // migrationBuilder.Sql("UPDATE AspNetUsers SET NormalizedFullName = lower(FullName);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedTitle",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "NormalizedFullName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Visibility",
                table: "AspNetUsers",
                newName: "isVisible");

			//Annullato
			// migrationBuilder.Sql("DROP TRIGGER BooksSetNormalizedTitleOnInsert;");
            // migrationBuilder.Sql("DROP TRIGGER BooksSetNormalizedTitleOnUpdate;");

			// migrationBuilder.Sql("DROP TRIGGER AspNetUsersSetNormalizedFullNameOnInsert;");
            // migrationBuilder.Sql("DROP TRIGGER AspNetUsersSetNormalizedFullNameOnUpdate;");
        }
    }
}
