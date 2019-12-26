using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class DayWeek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Weeks_FridayID",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_MondayID",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_SaturdayID",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_SundayID",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_ThursdayID",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_TuesdayID",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_WednesdayID",
                table: "Weeks");

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "Day",
                nullable: false,
                defaultValue: 0);


            migrationBuilder.Sql(@"update Day
                                   set DayOfWeek = 0
                                   where WeekID in (select ID from Weeks where SundayID = Day.ID);
	   
                                   update Day
                                   set DayOfWeek = 1
                                   where WeekID in (select ID from Weeks where MondayID = Day.ID);

                                   update Day
                                   set DayOfWeek = 2
                                   where WeekID in (select ID from Weeks where TuesdayID = Day.ID);

                                   update Day
                                   set DayOfWeek = 3
                                   where WeekID in (select ID from Weeks where WednesdayID = Day.ID);

                                   update Day
                                   set DayOfWeek = 4
                                   where WeekID in (select ID from Weeks where ThursdayID = Day.ID);

                                   update Day
                                   set DayOfWeek = 5
                                   where WeekID in (select ID from Weeks where FridayID = Day.ID);

                                   update Day
                                   set DayOfWeek = 6
                                   where WeekID in (select ID from Weeks where SaturdayID = Day.ID);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
