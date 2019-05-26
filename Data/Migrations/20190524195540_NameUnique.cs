using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class NameUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Recipies_Name",
                table: "Recipies",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipies_Name",
                table: "Recipies");
        }
    }
}
