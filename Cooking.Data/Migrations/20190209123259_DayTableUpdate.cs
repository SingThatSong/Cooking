using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations;

/// <summary>
/// Cascade delete on Day table.
/// </summary>
public partial class DayTableUpdate : Migration
{
    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
