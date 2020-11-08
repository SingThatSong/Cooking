using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Adds garnish to day.
    /// </summary>
    public partial class AddDayGarnish : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DinnerGarnishID",
                table: "Day",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Day_DinnerGarnishID",
                table: "Day",
                column: "DinnerGarnishID");

            migrationBuilder.AddForeignKey(
                name: "FK_Day_Recipies_DinnerGarnishID",
                table: "Day",
                column: "DinnerGarnishID",
                principalTable: "Recipies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Day_Recipies_DinnerGarnishID",
                table: "Day");

            migrationBuilder.DropIndex(
                name: "IX_Day_DinnerGarnishID",
                table: "Day");

            migrationBuilder.DropColumn(
                name: "DinnerGarnishID",
                table: "Day");
        }
    }
}
