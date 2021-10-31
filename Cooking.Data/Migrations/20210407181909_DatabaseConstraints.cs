using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations;

/// <summary>
/// Enforce database constrains (not-null columns).
/// </summary>
public partial class DatabaseConstraints : Migration
{
    /// <inheritdoc/>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Tags",
            type: "TEXT",
            nullable: false,
            defaultValue: string.Empty,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<Guid>(
            name: "IngredientID",
            table: "RecipeIngredients",
            type: "TEXT",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
            oldClrType: typeof(Guid),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "IngredientsGroup",
            type: "TEXT",
            nullable: false,
            defaultValue: string.Empty,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Ingredients",
            type: "TEXT",
            nullable: false,
            defaultValue: string.Empty,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);
    }

    /// <inheritdoc/>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Tags",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<Guid>(
            name: "IngredientID",
            table: "RecipeIngredients",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "IngredientsGroup",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Ingredients",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");
    }
}
