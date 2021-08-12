using Microsoft.EntityFrameworkCore.Migrations;

namespace Phrook.Migrations
{
    public partial class TriggersBookVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"CREATE TRIGGER BooksSetRowVersionOnInsert
                                   AFTER INSERT ON Books
                                   BEGIN
                                   UPDATE Books SET RowVersion = CURRENT_TIMESTAMP WHERE Id=NEW.Id;
                                   END;");
            migrationBuilder.Sql(@"CREATE TRIGGER BooksSetRowVersionOnUpdate
                                   AFTER UPDATE ON Books WHEN NEW.RowVersion <= OLD.RowVersion
                                   BEGIN
                                   UPDATE Books SET RowVersion = CURRENT_TIMESTAMP WHERE Id=NEW.Id;
                                   END;");
            migrationBuilder.Sql("UPDATE Books SET RowVersion = CURRENT_TIMESTAMP;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP TRIGGER BooksSetRowVersionOnInsert;");
            migrationBuilder.Sql("DROP TRIGGER BooksSetRowVersionOnUpdate;");
        }
    }
}
