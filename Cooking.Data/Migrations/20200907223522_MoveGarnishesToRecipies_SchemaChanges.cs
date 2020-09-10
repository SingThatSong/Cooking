using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Update schema to remove Garnishes table.
    /// </summary>
    public partial class MoveGarnishesToRecipies_SchemaChanges : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarnishRecipe");

            migrationBuilder.DropTable(
                name: "Garnishes");

            migrationBuilder.AddColumn<Guid>(
                name: "RecipeID",
                table: "Recipies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipies_RecipeID",
                table: "Recipies",
                column: "RecipeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipies_Recipies_RecipeID",
                table: "Recipies",
                column: "RecipeID",
                principalTable: "Recipies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipies_Recipies_RecipeID",
                table: "Recipies");

            migrationBuilder.DropIndex(
                name: "IX_Recipies_RecipeID",
                table: "Recipies");

            migrationBuilder.DropColumn(
                name: "RecipeID",
                table: "Recipies");

            migrationBuilder.CreateTable(
                name: "Garnishes",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Culture = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garnishes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GarnishRecipe",
                columns: table => new
                {
                    GarnishesID = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecipiesID = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarnishRecipe", x => new { x.GarnishesID, x.RecipiesID });
                    table.ForeignKey(
                        name: "FK_GarnishRecipe_Garnishes_GarnishesID",
                        column: x => x.GarnishesID,
                        principalTable: "Garnishes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GarnishRecipe_Recipies_RecipiesID",
                        column: x => x.RecipiesID,
                        principalTable: "Recipies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarnishRecipe_RecipiesID",
                table: "GarnishRecipe",
                column: "RecipiesID");
        }
    }
}
