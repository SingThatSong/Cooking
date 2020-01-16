using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cooking.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Recipies",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ImagePath = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PortionsCount = table.Column<int>(nullable: false),
                    CalorieType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Color = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Day",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DinnerID = table.Column<Guid>(nullable: true),
                    DinnerWasCooked = table.Column<bool>(nullable: false)
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
                name: "IngredientsGroup",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RecipeID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientsGroup", x => x.ID);
                    table.ForeignKey(
                        name: "FK_IngredientsGroup_Recipies_RecipeID",
                        column: x => x.RecipeID,
                        principalTable: "Recipies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RecipeTag",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(nullable: false),
                    TagId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeTag", x => new { x.RecipeId, x.TagId });
                    table.ForeignKey(
                        name: "FK_RecipeTag_Recipies_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
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

            migrationBuilder.CreateTable(
                name: "RecipeIngredients",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    IngredientId = table.Column<Guid>(nullable: true),
                    Amount = table.Column<double>(nullable: true),
                    MeasureUnitID = table.Column<int>(nullable: true),
                    IngredientsGroupID = table.Column<Guid>(nullable: true),
                    RecipeID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredients", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_IngredientsGroup_IngredientsGroupID",
                        column: x => x.IngredientsGroupID,
                        principalTable: "IngredientsGroup",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Recipies_RecipeID",
                        column: x => x.RecipeID,
                        principalTable: "Recipies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Day_DinnerID",
                table: "Day",
                column: "DinnerID");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientsGroup_RecipeID",
                table: "IngredientsGroup",
                column: "RecipeID");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientId",
                table: "RecipeIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientsGroupID",
                table: "RecipeIngredients",
                column: "IngredientsGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_RecipeID",
                table: "RecipeIngredients",
                column: "RecipeID");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTag_TagId",
                table: "RecipeTag",
                column: "TagId");

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
                name: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "RecipeTag");

            migrationBuilder.DropTable(
                name: "Weeks");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "IngredientsGroup");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Day");

            migrationBuilder.DropTable(
                name: "Recipies");
        }
    }
}
