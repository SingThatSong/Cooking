using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations;

/// <summary>
/// Add many-to-many migration between recipies and tags.
/// </summary>
public partial class AddManyToManyMigration : Migration
{
    /// <inheritdoc/>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Inconsistency fix: removing recipetags values where recipe is deleted
        migrationBuilder.Sql("delete from RecipeTag where RecipeId not in (select ID from Recipies);", suppressTransaction: true);

        // Disable foreign key checks
        migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

        migrationBuilder.DropForeignKey(
            name: "FK_RecipeTag_0_0",
            table: "RecipeTag");

        migrationBuilder.DropForeignKey(
            name: "FK_RecipeTag_1_0",
            table: "RecipeTag");

        migrationBuilder.RenameColumn(
            name: "TagId",
            table: "RecipeTag",
            newName: "Tag_ID");

        migrationBuilder.RenameColumn(
            name: "RecipeId",
            table: "RecipeTag",
            newName: "Recipe_ID");

        migrationBuilder.CreateIndex(
            table: "RecipeTag",
            name: "IX_RecipeTag_Tag_ID",
            column: "Tag_ID");

        migrationBuilder.AddForeignKey(
            name: "FK_RecipeTag_Recipies_Recipe_ID",
            table: "RecipeTag",
            column: "Recipe_ID",
            principalTable: "Recipies",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_RecipeTag_Tags_Tag_ID",
            table: "RecipeTag",
            column: "Tag_ID",
            principalTable: "Tags",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        // Enable foreign key checks
        migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
    }

    /// <inheritdoc/>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_RecipeTag_Recipies_Recipe_ID",
            table: "RecipeTag");

        migrationBuilder.DropForeignKey(
            name: "FK_RecipeTag_Tags_Tag_ID",
            table: "RecipeTag");

        migrationBuilder.RenameColumn(
            name: "Tag_ID",
            table: "RecipeTag",
            newName: "TagId");

        migrationBuilder.RenameColumn(
            name: "Recipe_ID",
            table: "RecipeTag",
            newName: "RecipeId");

        migrationBuilder.RenameIndex(
            name: "IX_RecipeTag_Tag_ID",
            table: "RecipeTag",
            newName: "IX_RecipeTag_TagId");

        migrationBuilder.AddForeignKey(
            name: "FK_RecipeTag_Recipies_RecipeId",
            table: "RecipeTag",
            column: "RecipeId",
            principalTable: "Recipies",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_RecipeTag_Tags_TagId",
            table: "RecipeTag",
            column: "TagId",
            principalTable: "Tags",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);
    }
}
