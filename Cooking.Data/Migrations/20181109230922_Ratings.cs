using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Ratings added for recipies.
    /// </summary>
    public partial class Ratings : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Recipies",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Recipies",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Recipies");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Recipies");
        }
    }
}
