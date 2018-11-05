using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class DayMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Day_DinnerID",
                table: "Day");

            migrationBuilder.CreateIndex(
                name: "IX_Day_DinnerID",
                table: "Day",
                column: "DinnerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Day_DinnerID",
                table: "Day");

            migrationBuilder.CreateIndex(
                name: "IX_Day_DinnerID",
                table: "Day",
                column: "DinnerID",
                unique: true);
        }
    }
}
