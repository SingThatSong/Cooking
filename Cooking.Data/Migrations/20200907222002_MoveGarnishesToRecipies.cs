using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Move garnishes to recipies.
    /// </summary>
    public partial class MoveGarnishesToRecipies : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MeasureUnit",
                type: "TEXT",
                nullable: false,
                defaultValue: string.Empty,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            string garnishTagID = "0DDA01F5-E6FF-4E2D-B29C-424CB235723F";

            migrationBuilder.Sql($@"
                INSERT INTO [Tags]
                           ([ID]
                           ,[Name]
                           ,[Type]
                           ,[Culture]
                           ,[IsInMenu]
                           ,[MenuIcon])
                     VALUES
                           ('{garnishTagID}'
                           ,'Гарниры'
                           ,'ru-RU'
                           ,'ru-RU'
                           ,true
                           ,'FoodCupcake');
           
                INSERT INTO [Recipies]
                           ([ID]
                           ,[Name]
                           ,[Culture]
                           ,[PortionsCount]
                           ,[CalorieType])
                SELECT [ID]
                      ,[Name]
                      ,[Culture]
                      , 0
                      , 0
                  FROM [Garnishes];


                INSERT INTO [RecipeTag]
                           ([RecipiesID]
                           ,[TagsID])
                SELECT [ID]
	                  , '{garnishTagID}' as TagID
                FROM [Garnishes];"
            );
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new InvalidOperationException("Downgrade not supported");
        }
    }
}
