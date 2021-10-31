using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations;

/// <summary>
/// Migration to address EF Core new namings.
/// </summary>
public partial class EFCoreNamingUpdate : Migration
{
    /// <inheritdoc/>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_GarnishRecipe_Garnishes_GarnishID",
            table: "GarnishRecipe");

        migrationBuilder.DropForeignKey(
            name: "FK_GarnishRecipe_Recipies_RecipeID",
            table: "GarnishRecipe");

        migrationBuilder.DropForeignKey(
            name: "FK_RecipeTag_Recipies_RecipeID",
            table: "RecipeTag");

        migrationBuilder.DropForeignKey(
            name: "FK_RecipeTag_Tags_TagID",
            table: "RecipeTag");

        migrationBuilder.RenameColumn(
            name: "TagID",
            table: "RecipeTag",
            newName: "TagsID");

        migrationBuilder.RenameColumn(
            name: "RecipeID",
            table: "RecipeTag",
            newName: "RecipiesID");

        migrationBuilder.RenameIndex(
            name: "IX_RecipeTag_TagID",
            table: "RecipeTag",
            newName: "IX_RecipeTag_TagsID");

        migrationBuilder.RenameColumn(
            name: "RecipeID",
            table: "GarnishRecipe",
            newName: "RecipiesID");

        migrationBuilder.RenameColumn(
            name: "GarnishID",
            table: "GarnishRecipe",
            newName: "GarnishesID");

        migrationBuilder.RenameIndex(
            name: "IX_GarnishRecipe_RecipeID",
            table: "GarnishRecipe",
            newName: "IX_GarnishRecipe_RecipiesID");

        migrationBuilder.AddForeignKey(
            name: "FK_GarnishRecipe_Garnishes_GarnishesID",
            table: "GarnishRecipe",
            column: "GarnishesID",
            principalTable: "Garnishes",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_GarnishRecipe_Recipies_RecipiesID",
            table: "GarnishRecipe",
            column: "RecipiesID",
            principalTable: "Recipies",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_RecipeTag_Recipies_RecipiesID",
            table: "RecipeTag",
            column: "RecipiesID",
            principalTable: "Recipies",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_RecipeTag_Tags_TagsID",
            table: "RecipeTag",
            column: "TagsID",
            principalTable: "Tags",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc/>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_GarnishRecipe_Garnishes_GarnishesID",
            table: "GarnishRecipe");

        migrationBuilder.DropForeignKey(
            name: "FK_GarnishRecipe_Recipies_RecipiesID",
            table: "GarnishRecipe");

        migrationBuilder.DropForeignKey(
            name: "FK_RecipeTag_Recipies_RecipiesID",
            table: "RecipeTag");

        migrationBuilder.DropForeignKey(
            name: "FK_RecipeTag_Tags_TagsID",
            table: "RecipeTag");

        migrationBuilder.RenameColumn(
            name: "TagsID",
            table: "RecipeTag",
            newName: "TagID");

        migrationBuilder.RenameColumn(
            name: "RecipiesID",
            table: "RecipeTag",
            newName: "RecipeID");

        migrationBuilder.RenameIndex(
            name: "IX_RecipeTag_TagsID",
            table: "RecipeTag",
            newName: "IX_RecipeTag_TagID");

        migrationBuilder.RenameColumn(
            name: "RecipiesID",
            table: "GarnishRecipe",
            newName: "RecipeID");

        migrationBuilder.RenameColumn(
            name: "GarnishesID",
            table: "GarnishRecipe",
            newName: "GarnishID");

        migrationBuilder.RenameIndex(
            name: "IX_GarnishRecipe_RecipiesID",
            table: "GarnishRecipe",
            newName: "IX_GarnishRecipe_RecipeID");

        migrationBuilder.AddForeignKey(
            name: "FK_GarnishRecipe_Garnishes_GarnishID",
            table: "GarnishRecipe",
            column: "GarnishID",
            principalTable: "Garnishes",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_GarnishRecipe_Recipies_RecipeID",
            table: "GarnishRecipe",
            column: "RecipeID",
            principalTable: "Recipies",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_RecipeTag_Recipies_RecipeID",
            table: "RecipeTag",
            column: "RecipeID",
            principalTable: "Recipies",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_RecipeTag_Tags_TagID",
            table: "RecipeTag",
            column: "TagID",
            principalTable: "Tags",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);
    }
}
