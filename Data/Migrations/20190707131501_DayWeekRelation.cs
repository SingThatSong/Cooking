using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class DayWeekRelation : Migration
    {
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
}
