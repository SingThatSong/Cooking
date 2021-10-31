using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations;

/// <summary>
/// Remove old MeasureUnitID and set new one with reference to the table.
/// </summary>
public partial class UpdateRecipeIngredients : Migration
{
    /// <inheritdoc/>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Disable foreign key checks
        migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

        // Rename old Day table to temp
        migrationBuilder.Sql($"ALTER TABLE RecipeIngredients RENAME TO _RecipeIngredients;");

        // Recreate table with new types for Day
        migrationBuilder.Sql(@"
                CREATE TABLE [RecipeIngredients] (
                  [ID] text NOT NULL
                , [IngredientId] text NULL
                , [Amount] real NULL
                , [IngredientsGroupID] text NULL
                , [RecipeID] text NULL
                , [Order] bigint DEFAULT (0) NOT NULL
                , [Culture] text DEFAULT ('ru-RU') NOT NULL
                , [MeasureUnitID] text NULL
                , CONSTRAINT [sqlite_autoindex_RecipeIngredients_1] PRIMARY KEY ([ID])
                , CONSTRAINT [FK_RecipeIngredients_0_0] FOREIGN KEY ([RecipeID]) REFERENCES [Recipies] ([ID]) ON DELETE RESTRICT ON UPDATE NO ACTION
                , CONSTRAINT [FK_RecipeIngredients_1_0] FOREIGN KEY ([IngredientsGroupID]) REFERENCES [IngredientsGroup] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION
                , CONSTRAINT [FK_RecipeIngredients_2_0] FOREIGN KEY ([IngredientId]) REFERENCES [Ingredients] ([ID]) ON DELETE SET NULL ON UPDATE NO ACTION
                , CONSTRAINT [FK_RecipeIngredients_3_0] FOREIGN KEY ([MeasureUnitID]) REFERENCES [MeasureUnit] ([ID]) ON DELETE SET NULL ON UPDATE NO ACTION
                );

                CREATE INDEX [RecipeIngredients_IX_RecipeIngredients_MeasureUnitID] ON [RecipeIngredients] ([MeasureUnitID] ASC);
                CREATE INDEX [RecipeIngredients_RecipeIngredients_RecipeIngredients_IX_RecipeIngredients_RecipeID] ON [RecipeIngredients] ([RecipeID] ASC);
                CREATE INDEX [RecipeIngredients_RecipeIngredients_RecipeIngredients_IX_RecipeIngredients_IngredientsGroupID] ON [RecipeIngredients] ([IngredientsGroupID] ASC);
                CREATE INDEX [RecipeIngredients_RecipeIngredients_RecipeIngredients_IX_RecipeIngredients_IngredientId] ON [RecipeIngredients] ([IngredientId] ASC);");

        // Copy data from backup
        migrationBuilder.Sql(@$"
                INSERT INTO RecipeIngredients SELECT [ID]
                     ,[IngredientId]
                     ,[Amount]
                     ,[IngredientsGroupID]
                     ,[RecipeID]
                     ,[Order]
                     ,[Culture]
                     ,[MeasureUnitGuid]
                 FROM [_RecipeIngredients];");

        // Drop backup
        migrationBuilder.Sql($"DROP TABLE [_RecipeIngredients];");

        // Re-enable foreign key checks
        migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
    }

    /// <inheritdoc/>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
