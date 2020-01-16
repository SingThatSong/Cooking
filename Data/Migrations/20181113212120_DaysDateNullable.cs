using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cooking.Data.Migrations
{
    public partial class DaysDateNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Day",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Day");
        }
    }
}
