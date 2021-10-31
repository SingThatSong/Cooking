using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations;

/// <summary>
/// Migration to change week-days relation from set of FKs to one-to-many.
/// </summary>
public partial class DayWeekRelation : Migration
{
    /// <inheritdoc/>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "WeekID",
            table: "Day",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Day_WeekID",
            table: "Day",
            column: "WeekID");

        migrationBuilder.Sql(@"update Day
                                   set WeekID = (select ID from Weeks where MondayID = Day.ID 
									                                      or TuesdayID = Day.ID
									                                      or WednesdayID = Day.ID
									                                      or ThursdayID = Day.ID
									                                      or FridayID = Day.ID
									                                      or SaturdayID = Day.ID
									                                      or SundayID = Day.ID);");
    }

    /// <inheritdoc/>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Day_Weeks_WeekID",
            table: "Day");

        migrationBuilder.DropIndex(
            name: "IX_Day_WeekID",
            table: "Day");

        migrationBuilder.DropColumn(
            name: "WeekID",
            table: "Day");
    }
}
