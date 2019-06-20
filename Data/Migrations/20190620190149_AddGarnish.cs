using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class AddGarnish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipies_Name",
                table: "Recipies");

            migrationBuilder.CreateTable(
                name: "Garnishes",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garnishes", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Garnishes");

            migrationBuilder.CreateIndex(
                name: "IX_Recipies_Name",
                table: "Recipies",
                column: "Name",
                unique: true);
        }
    }
}
