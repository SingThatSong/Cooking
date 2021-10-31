using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations;

/// <summary>
/// Removing DayOfWeek column, cause it's already part of Date.
/// </summary>
public partial class RemoveDayOfWeek : Migration
{
    /// <inheritdoc/>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DayOfWeek",
            table: "Day");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Recipies",
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
            table: "Recipies",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AddColumn<int>(
            name: "DayOfWeek",
            table: "Day",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);
    }
}
