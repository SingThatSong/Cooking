using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations;

/// <summary>
/// Ingredient order migration.
/// </summary>
public partial class IngredientOrder : Migration
{
    /// <inheritdoc/>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Order",
            table: "RecipeIngredients",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc/>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Order",
            table: "RecipeIngredients");
    }
}
