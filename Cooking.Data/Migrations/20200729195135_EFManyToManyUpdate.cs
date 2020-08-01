using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    public partial class EFManyToManyUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                newName: "TagID");

            migrationBuilder.RenameColumn(
                name: "Recipe_ID",
                table: "RecipeTag",
                newName: "RecipeID");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeTag_Tag_ID",
                table: "RecipeTag",
                newName: "IX_RecipeTag_TagID");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeTag_Recipies_RecipeID",
                table: "RecipeTag");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeTag_Tags_TagID",
                table: "RecipeTag");

            migrationBuilder.RenameColumn(
                name: "TagID",
                table: "RecipeTag",
                newName: "Tag_ID");

            migrationBuilder.RenameColumn(
                name: "RecipeID",
                table: "RecipeTag",
                newName: "Recipe_ID");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeTag_TagID",
                table: "RecipeTag",
                newName: "IX_RecipeTag_Tag_ID");

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
        }
    }
}
