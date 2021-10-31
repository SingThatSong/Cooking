using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations;

/// <summary>
/// Migration to enforce that recipe names must be unique.
/// </summary>
public partial class NameUnique : Migration
{
    /// <inheritdoc/>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Recipies_Name",
            table: "Recipies",
            column: "Name",
            unique: true);
    }

    /// <inheritdoc/>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Recipies_Name",
            table: "Recipies");
    }
}
