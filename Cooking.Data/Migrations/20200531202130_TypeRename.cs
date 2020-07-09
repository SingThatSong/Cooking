using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Rename TypeID column to Type.
    /// </summary>
    public partial class TypeRename : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeID",
                table: "Ingredients",
                newName: "Type");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Ingredients",
                newName: "TypeID");
        }
    }
}
