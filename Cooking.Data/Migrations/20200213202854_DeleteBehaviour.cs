using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Set null to Ingredient's FK when ingredient is deleted.
    /// </summary>
    public partial class DeleteBehaviour : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Disable foreign key checks
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

            ChangeTableNames(@"CREATE TABLE [RecipeIngredients] (
                          [ID] text NOT NULL
                        , [IngredientId] text NULL
                        , [Amount] real NULL
                        , [MeasureUnitID] bigint NULL
                        , [IngredientsGroupID] text NULL
                        , [RecipeID] text NULL
                        , [Order] bigint DEFAULT (0) NOT NULL
                        , [Culture] text DEFAULT ('ru-RU') NOT NULL
                        , CONSTRAINT [sqlite_autoindex_RecipeIngredients_1] PRIMARY KEY ([ID])
                        , CONSTRAINT [FK_RecipeIngredients_0_0] FOREIGN KEY ([IngredientId]) REFERENCES [Ingredients] ([ID]) ON DELETE SET NULL ON UPDATE NO ACTION
                        , CONSTRAINT [FK_RecipeIngredients_1_0] FOREIGN KEY ([IngredientsGroupID]) REFERENCES [IngredientsGroup] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION
                        , CONSTRAINT [FK_RecipeIngredients_2_0] FOREIGN KEY ([RecipeID]) REFERENCES [Recipies] ([ID]) ON DELETE RESTRICT ON UPDATE NO ACTION
                        );",
                        "RecipeIngredients",
                        migrationBuilder);

            migrationBuilder.Sql("CREATE INDEX [RecipeIngredients_RecipeIngredients_IX_RecipeIngredients_IngredientId] ON [RecipeIngredients] ([IngredientId] ASC);");
            migrationBuilder.Sql("CREATE INDEX[RecipeIngredients_RecipeIngredients_IX_RecipeIngredients_IngredientsGroupID] ON[RecipeIngredients]([IngredientsGroupID] ASC);");
            migrationBuilder.Sql("CREATE INDEX[RecipeIngredients_RecipeIngredients_IX_RecipeIngredients_RecipeID] ON[RecipeIngredients]([RecipeID] ASC);");

            // Re-enable foreign key checks
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

        private void ChangeTableNames(string sql, string tableName, MigrationBuilder migrationBuilder)
        {
            // Rename old Day table to temp
            migrationBuilder.Sql($"ALTER TABLE {tableName} RENAME TO _{tableName};", suppressTransaction: true);

            // Recreate table with new types for Day
            migrationBuilder.Sql(sql, suppressTransaction: true);

            // Copy data from backup
            migrationBuilder.Sql(@$"INSERT INTO {tableName} SELECT * FROM [_{tableName}];", suppressTransaction: true);

            // Drop backup
            migrationBuilder.Sql($"DROP TABLE [_{tableName}];", suppressTransaction: true);
        }
    }
}
