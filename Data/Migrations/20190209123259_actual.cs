using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class actual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA writable_schema=1;");
            migrationBuilder.Sql(@"UPDATE sqlite_master SET sql='CREATE TABLE ""Day""
(
    ""ID"" BLOB NOT NULL CONSTRAINT ""PK_Day"" PRIMARY KEY,
    ""DinnerID"" BLOB NULL,
    ""DinnerWasCooked"" INTEGER NOT NULL,
    ""Date"" TEXT NULL,
    CONSTRAINT ""FK_Day_Recipies_DinnerID"" FOREIGN KEY (""DinnerID"") REFERENCES ""Recipies"" (""ID"") ON DELETE SET NULL
)'
                WHERE type='table' AND name='Day';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
