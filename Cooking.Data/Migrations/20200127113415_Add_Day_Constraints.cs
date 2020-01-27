using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    public partial class Add_Day_Constraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename old Day table to temp
            migrationBuilder.Sql($"ALTER TABLE Day RENAME TO _Day;");

            // Recreate table with new types for Day
            migrationBuilder.Sql(@"CREATE TABLE [Day] (
                                  [ID] text NOT NULL
                                , [DinnerID] text NOT NULL
                                , [DinnerWasCooked] bigint NOT NULL
                                , [Date] text NOT NULL
                                , [DayOfWeek] bigint DEFAULT (0) NOT NULL
                                , [Culture] text DEFAULT ('ru-RU') NOT NULL
                                , CONSTRAINT [sqlite_autoindex_Day_1] PRIMARY KEY ([ID])
                                , CONSTRAINT [FK_Day_0_0] FOREIGN KEY ([DinnerID]) REFERENCES [Recipies] ([ID]) ON DELETE SET NULL ON UPDATE NO ACTION
                                );");

            // Copy data from backup
            migrationBuilder.Sql(@$"INSERT INTO Day SELECT * FROM [_Day] WHERE DinnerID IS NOT NULL;");

            // Drop backup
            migrationBuilder.Sql($"DROP TABLE [_Day];");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
