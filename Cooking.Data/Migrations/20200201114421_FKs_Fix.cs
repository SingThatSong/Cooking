using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Somehow foreign keys still have _"Table" relationships. Fixing it.
    /// </summary>
    public partial class FKs_Fix : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Disable foreign key checks
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

            ChangeIDTypes(@"CREATE TABLE [IngredientsGroup] (
                              [ID] text NOT NULL
                            , [Name] text NULL
                            , [RecipeID] text NULL
                            , [Culture] text DEFAULT ('ru-RU') NOT NULL
                            , CONSTRAINT [sqlite_autoindex_IngredientsGroup_1] PRIMARY KEY ([ID])
                            , CONSTRAINT [FK_IngredientsGroup_0_0] FOREIGN KEY ([RecipeID]) REFERENCES [_Recipies] ([ID]) ON DELETE SET NULL ON UPDATE NO ACTION
                            );",
                            "IngredientsGroup",
                            migrationBuilder);

            ChangeIDTypes(@"CREATE TABLE [RecipeIngredients] (
                          [ID] text NOT NULL
                        , [IngredientId] text NULL
                        , [Amount] real NULL
                        , [MeasureUnitID] bigint NULL
                        , [IngredientsGroupID] text NULL
                        , [RecipeID] text NULL
                        , [Order] bigint DEFAULT (0) NOT NULL
                        , [Culture] text DEFAULT ('ru-RU') NOT NULL
                        , CONSTRAINT [sqlite_autoindex_RecipeIngredients_1] PRIMARY KEY ([ID])
                        , CONSTRAINT [FK_RecipeIngredients_0_0] FOREIGN KEY ([IngredientId]) REFERENCES [Ingredients] ([ID]) ON DELETE RESTRICT ON UPDATE NO ACTION
                        , CONSTRAINT [FK_RecipeIngredients_1_0] FOREIGN KEY ([IngredientsGroupID]) REFERENCES [IngredientsGroup] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION
                        , CONSTRAINT [FK_RecipeIngredients_2_0] FOREIGN KEY ([RecipeID]) REFERENCES [_Recipies] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION
                        );",
                        "RecipeIngredients",
                        migrationBuilder);

            ChangeIDTypes(@"CREATE TABLE [RecipeTag] (
                          [RecipeId] text NOT NULL
                        , [TagId] text NOT NULL
                        , CONSTRAINT [sqlite_autoindex_RecipeTag_1] PRIMARY KEY ([RecipeId],[TagId])
                        , CONSTRAINT [FK_RecipeTag_0_0] FOREIGN KEY ([RecipeId]) REFERENCES [_Recipies] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION
                        , CONSTRAINT [FK_RecipeTag_1_0] FOREIGN KEY ([TagId]) REFERENCES [_Tags] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION
                        );",
                        "RecipeTag",
                        migrationBuilder);

            // Re-enable foreign key checks
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

        private void ChangeIDTypes(string originalSql, string tableName, MigrationBuilder migrationBuilder)
        {
            // Rename old Day table to temp
            migrationBuilder.Sql($"ALTER TABLE {tableName} RENAME TO _{tableName};", suppressTransaction: true);

            string sql = originalSql.Replace("_Recipies", "Recipies", StringComparison.Ordinal)
                                    .Replace("_Tags", "Tags", StringComparison.Ordinal);

            // Recreate table with new types for Day
            migrationBuilder.Sql(sql, suppressTransaction: true);

            // Copy data from backup
            migrationBuilder.Sql(@$"INSERT INTO {tableName} SELECT * FROM [_{tableName}];", suppressTransaction: true);

            // Drop backup
            migrationBuilder.Sql($"DROP TABLE [_{tableName}];", suppressTransaction: true);
        }
    }
}
