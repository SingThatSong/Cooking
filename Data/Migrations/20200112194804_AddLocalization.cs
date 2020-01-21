using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Migration to add localization to all tables.
    /// </summary>
    public partial class AddLocalization : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "Weeks",
                defaultValue: "ru-RU",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "Tags",
                defaultValue: "ru-RU",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "Recipies",
                defaultValue: "ru-RU",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "RecipeIngredients",
                defaultValue: "ru-RU",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "IngredientsGroup",
                defaultValue: "ru-RU",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "Ingredients",
                defaultValue: "ru-RU",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "Garnishes",
                defaultValue: "ru-RU",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "Day",
                defaultValue: "ru-RU",
                nullable: false);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException();
        }
    }
}
