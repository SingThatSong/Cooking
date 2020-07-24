using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    public partial class MakeIDUppercase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Inconsistency fix: removing recipetags values where recipe is deleted
            migrationBuilder.Sql("delete from RecipeIngredients where RecipeID not in (select ID from Recipies);", suppressTransaction: true);

            // Disable foreign key checks
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientId",
                table: "RecipeIngredients");

            migrationBuilder.RenameColumn(
                name: "IngredientId",
                table: "RecipeIngredients",
                newName: "IngredientID");

            migrationBuilder.RenameIndex(
                name: "RecipeIngredients_RecipeIngredients_RecipeIngredients_IX_RecipeIngredients_IngredientId",
                table: "RecipeIngredients",
                newName: "IX_RecipeIngredients_IngredientID");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientID",
                table: "RecipeIngredients",
                column: "IngredientID",
                principalTable: "Ingredients",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            // Enable foreign key checks
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientID",
                table: "RecipeIngredients");

            migrationBuilder.RenameColumn(
                name: "IngredientID",
                table: "RecipeIngredients",
                newName: "IngredientId");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeIngredients_IngredientID",
                table: "RecipeIngredients",
                newName: "IX_RecipeIngredients_IngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientId",
                table: "RecipeIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
