using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    public partial class AddMeasureUnitTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MeasureUnitGuid",
                table: "RecipeIngredients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MeasureUnit",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Culture = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    FullName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureUnit", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_MeasureUnitGuid",
                table: "RecipeIngredients",
                column: "MeasureUnitGuid");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_MeasureUnit_MeasureUnitGuid",
                table: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "MeasureUnit");

            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_MeasureUnitGuid",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "MeasureUnitGuid",
                table: "RecipeIngredients");
        }
    }
}
