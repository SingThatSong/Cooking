using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Migration to remove Week table.
    /// </summary>
    public partial class RemoveWeek : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Day_WeekID",
                table: "Day");

            // Drop WeekID Foreign key

            // Rename old Day table to temp
            migrationBuilder.Sql("ALTER TABLE Day RENAME TO _Day;");

            // Recreate table Day without WeekID FK
            migrationBuilder.Sql(@"CREATE TABLE [Day] (
  [ID] image NOT NULL
, [DinnerID] image NULL
, [DinnerWasCooked] bigint NOT NULL
, [Date] text NULL
, [DayOfWeek] bigint DEFAULT (0) NOT NULL
, [Culture] text DEFAULT ('ru-RU') NOT NULL
, CONSTRAINT [sqlite_autoindex__Day_1] PRIMARY KEY ([ID])
, CONSTRAINT [FK__Day_0_0] FOREIGN KEY ([DinnerID]) REFERENCES [Recipies] ([ID]) ON DELETE SET NULL ON UPDATE NO ACTION
);");

            migrationBuilder.Sql("CREATE INDEX [Day_IX_Day_DinnerID] ON [Day] ([DinnerID] ASC);");

            migrationBuilder.Sql(@"INSERT INTO Day 
SELECT [ID]
      ,[DinnerID]
      ,[DinnerWasCooked]
      ,[Date]
      ,[DayOfWeek]
      ,[Culture]
  FROM [_Day];");

            migrationBuilder.Sql("DROP TABLE [_Day];");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
