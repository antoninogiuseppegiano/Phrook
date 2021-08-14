using Microsoft.EntityFrameworkCore.Migrations;

namespace Phrook.Migrations
{
    public partial class TriggerLibraryBookVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"CREATE TRIGGER LibraryBooksSetRowVersionOnInsert
                                   AFTER INSERT ON LibraryBooks
                                   BEGIN
                                   UPDATE LibraryBooks SET RowVersion = CURRENT_TIMESTAMP WHERE Id=NEW.Id;
                                   END;");
            migrationBuilder.Sql(@"CREATE TRIGGER LibraryBooksSetRowVersionOnUpdate
                                   AFTER UPDATE ON LibraryBooks WHEN NEW.RowVersion <= OLD.RowVersion
                                   BEGIN
                                   UPDATE LibraryBooks SET RowVersion = CURRENT_TIMESTAMP WHERE Id=NEW.Id;
                                   END;");
            migrationBuilder.Sql("UPDATE LibraryBooks SET RowVersion = CURRENT_TIMESTAMP;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP TRIGGER BooksSetRowVersionOnInsert;");
            migrationBuilder.Sql("DROP TRIGGER BooksSetRowVersionOnUpdate;");
        }
    }
}
