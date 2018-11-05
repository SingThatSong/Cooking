using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class WeeksStart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Day",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DinnerID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Day", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Day_Recipies_DinnerID",
                        column: x => x.DinnerID,
                        principalTable: "Recipies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weeks",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    MondayID = table.Column<Guid>(nullable: true),
                    TuesdayID = table.Column<Guid>(nullable: true),
                    WednesdayID = table.Column<Guid>(nullable: true),
                    ThursdayID = table.Column<Guid>(nullable: true),
                    FridayID = table.Column<Guid>(nullable: true),
                    SaturdayID = table.Column<Guid>(nullable: true),
                    SundayID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weeks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Weeks_Day_FridayID",
                        column: x => x.FridayID,
                        principalTable: "Day",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weeks_Day_MondayID",
                        column: x => x.MondayID,
                        principalTable: "Day",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weeks_Day_SaturdayID",
                        column: x => x.SaturdayID,
                        principalTable: "Day",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weeks_Day_SundayID",
                        column: x => x.SundayID,
                        principalTable: "Day",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weeks_Day_ThursdayID",
                        column: x => x.ThursdayID,
                        principalTable: "Day",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weeks_Day_TuesdayID",
                        column: x => x.TuesdayID,
                        principalTable: "Day",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weeks_Day_WednesdayID",
                        column: x => x.WednesdayID,
                        principalTable: "Day",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Day_DinnerID",
                table: "Day",
                column: "DinnerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_FridayID",
                table: "Weeks",
                column: "FridayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_MondayID",
                table: "Weeks",
                column: "MondayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_SaturdayID",
                table: "Weeks",
                column: "SaturdayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_SundayID",
                table: "Weeks",
                column: "SundayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_ThursdayID",
                table: "Weeks",
                column: "ThursdayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_TuesdayID",
                table: "Weeks",
                column: "TuesdayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_WednesdayID",
                table: "Weeks",
                column: "WednesdayID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weeks");

            migrationBuilder.DropTable(
                name: "Day");
        }
    }
}
