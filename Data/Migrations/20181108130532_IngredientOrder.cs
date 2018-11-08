using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class IngredientOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "RecipeIngredients",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "RecipeIngredients");
        }
    }
}
