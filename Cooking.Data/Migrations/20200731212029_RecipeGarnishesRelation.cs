using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    public partial class RecipeGarnishesRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarnishRecipe",
                columns: table => new
                {
                    GarnishID = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecipeID = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarnishRecipe", x => new { x.GarnishID, x.RecipeID });
                    table.ForeignKey(
                        name: "FK_GarnishRecipe_Garnishes_GarnishID",
                        column: x => x.GarnishID,
                        principalTable: "Garnishes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GarnishRecipe_Recipies_RecipeID",
                        column: x => x.RecipeID,
                        principalTable: "Recipies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarnishRecipe_RecipeID",
                table: "GarnishRecipe",
                column: "RecipeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarnishRecipe");
        }
    }
}
