using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Migration to add property to keep selected icon for a tag.
    /// </summary>
    public partial class AddTagMenuIcon : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MenuIcon",
                table: "Tags",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuIcon",
                table: "Tags");
        }
    }
}
