using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Delete day on recipe deletion.
    /// </summary>
    public partial class DayCascadeDelete : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Day_Recipies_DinnerID",
                table: "Day");

            migrationBuilder.AddForeignKey(
                name: "FK_Day_Recipies_DinnerID",
                table: "Day",
                column: "DinnerID",
                principalTable: "Recipies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Day_Recipies_DinnerID",
                table: "Day");

            migrationBuilder.AddForeignKey(
                name: "FK_Day_Recipies_DinnerID",
                table: "Day",
                column: "DinnerID",
                principalTable: "Recipies",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
