using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Change type of IDs to text due to https://docs.microsoft.com/ru-ru/ef/core/what-is-new/ef-core-3.0/breaking-changes#guid.
    /// </summary>
    public partial class Change_Id_Types : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Disable foreign key checks
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

            // Change ID types for Day
            migrationBuilder.Sql(GetColumnUpdateToText("Day", "DinnerID"));
            migrationBuilder.Sql(GetColumnUpdateToText("Day", "ID"));

            ChangeIDTypes(@"CREATE TABLE [Day] (
                              [ID] image NOT NULL
                            , [DinnerID] image NULL
                            , [DinnerWasCooked] bigint NOT NULL
                            , [Date] text NULL
                            , [DayOfWeek] bigint DEFAULT (0) NOT NULL
                            , [Culture] text DEFAULT ('ru-RU') NOT NULL
                            , CONSTRAINT [sqlite_autoindex_Day_1] PRIMARY KEY ([ID])
                            , CONSTRAINT [FK_Day_0_0] FOREIGN KEY ([DinnerID]) REFERENCES [Recipies] ([ID]) ON DELETE SET NULL ON UPDATE NO ACTION
                            );",
                            "Day",
                            migrationBuilder);

            migrationBuilder.Sql(GetColumnUpdateToText("Garnishes", "ID"));

            ChangeIDTypes(@"CREATE TABLE [Garnishes] (
                              [ID] image NOT NULL
                            , [Name] text NULL
                            , [Culture] text DEFAULT ('ru-RU') NOT NULL
                            , CONSTRAINT [sqlite_autoindex_Garnishes_1] PRIMARY KEY ([ID])
                            );
                            ",
                            "Garnishes",
                            migrationBuilder);

            migrationBuilder.Sql(GetColumnUpdateToText("Ingredients", "ID"));

            ChangeIDTypes(@"CREATE TABLE [Ingredients] (
                              [ID] image NOT NULL
                            , [Name] text NULL
                            , [TypeID] bigint NULL
                            , [Culture] text DEFAULT ('ru-RU') NOT NULL
                            , CONSTRAINT [sqlite_autoindex_Ingredients_1] PRIMARY KEY ([ID])
                            );",
                            "Ingredients",
                            migrationBuilder);

            migrationBuilder.Sql(GetColumnUpdateToText("IngredientsGroup", "ID"));
            migrationBuilder.Sql(GetColumnUpdateToText("IngredientsGroup", "RecipeID"));

            ChangeIDTypes(@"CREATE TABLE [IngredientsGroup] (
                              [ID] image NOT NULL
                            , [Name] text NULL
                            , [RecipeID] image NULL
                            , [Culture] text DEFAULT ('ru-RU') NOT NULL
                            , CONSTRAINT [sqlite_autoindex_IngredientsGroup_1] PRIMARY KEY ([ID])
                            , CONSTRAINT [FK_IngredientsGroup_0_0] FOREIGN KEY ([RecipeID]) REFERENCES [Recipies] ([ID]) ON DELETE SET NULL ON UPDATE NO ACTION
                            );",
                            "IngredientsGroup",
                            migrationBuilder);

            migrationBuilder.Sql("CREATE INDEX[IngredientsGroup_IX_IngredientsGroup_RecipeID] ON[IngredientsGroup]([RecipeID] ASC);");

            migrationBuilder.Sql(GetColumnUpdateToText("RecipeIngredients", "ID"));
            migrationBuilder.Sql(GetColumnUpdateToText("RecipeIngredients", "IngredientId"));
            migrationBuilder.Sql(GetColumnUpdateToText("RecipeIngredients", "IngredientsGroupID"));
            migrationBuilder.Sql(GetColumnUpdateToText("RecipeIngredients", "RecipeID"));

            ChangeIDTypes(@"CREATE TABLE [RecipeIngredients] (
                              [ID] image NOT NULL
                            , [IngredientId] image NULL
                            , [Amount] real NULL
                            , [MeasureUnitID] bigint NULL
                            , [IngredientsGroupID] image NULL
                            , [RecipeID] image NULL
                            , [Order] bigint DEFAULT (0) NOT NULL
                            , [Culture] text DEFAULT ('ru-RU') NOT NULL
                            , CONSTRAINT [sqlite_autoindex_RecipeIngredients_1] PRIMARY KEY ([ID])
                            , CONSTRAINT [FK_RecipeIngredients_0_0] FOREIGN KEY ([RecipeID]) REFERENCES [Recipies] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION
                            , CONSTRAINT [FK_RecipeIngredients_1_0] FOREIGN KEY ([IngredientsGroupID]) REFERENCES [IngredientsGroup] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION
                            , CONSTRAINT [FK_RecipeIngredients_2_0] FOREIGN KEY ([IngredientId]) REFERENCES [Ingredients] ([ID]) ON DELETE RESTRICT ON UPDATE NO ACTION
                            );",
                            "RecipeIngredients",
                            migrationBuilder);

            migrationBuilder.Sql("CREATE INDEX[RecipeIngredients_IX_RecipeIngredients_RecipeID] ON[RecipeIngredients]([RecipeID] ASC);");
            migrationBuilder.Sql("CREATE INDEX[RecipeIngredients_IX_RecipeIngredients_IngredientsGroupID] ON[RecipeIngredients]([IngredientsGroupID] ASC);");
            migrationBuilder.Sql("CREATE INDEX[RecipeIngredients_IX_RecipeIngredients_IngredientId] ON[RecipeIngredients]([IngredientId] ASC);");

            migrationBuilder.Sql(GetColumnUpdateToText("RecipeTag", "RecipeId"));
            migrationBuilder.Sql(GetColumnUpdateToText("RecipeTag", "TagId"));

            ChangeIDTypes(@"CREATE TABLE [RecipeTag] (
                              [RecipeId] image NOT NULL
                            , [TagId] image NOT NULL
                            , CONSTRAINT [sqlite_autoindex_RecipeTag_1] PRIMARY KEY ([RecipeId],[TagId])
                            , CONSTRAINT [FK_RecipeTag_0_0] FOREIGN KEY ([TagId]) REFERENCES [Tags] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION
                            , CONSTRAINT [FK_RecipeTag_1_0] FOREIGN KEY ([RecipeId]) REFERENCES [Recipies] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION
                            );",
                            "RecipeTag",
                            migrationBuilder);

            migrationBuilder.Sql("CREATE INDEX[RecipeTag_IX_RecipeTag_TagId] ON[RecipeTag]([TagId] ASC);");

            migrationBuilder.Sql(GetColumnUpdateToText("Recipies", "ID"));

            ChangeIDTypes(@"CREATE TABLE [Recipies] (
                              [ID] image NOT NULL
                            , [Name] text NULL
                            , [ImagePath] text NULL
                            , [Description] text NULL
                            , [PortionsCount] bigint NOT NULL
                            , [CalorieType] bigint NOT NULL
                            , [SourceUrl] text NULL
                            , [Difficulty] bigint DEFAULT (0) NOT NULL
                            , [Rating] bigint DEFAULT (0) NOT NULL
                            , [Culture] text DEFAULT ('ru-RU') NOT NULL
                            , CONSTRAINT [sqlite_autoindex_Recipies_1] PRIMARY KEY ([ID])
                            );",
                            "Recipies",
                            migrationBuilder);

            migrationBuilder.Sql(GetColumnUpdateToText("Tags", "ID"));

            ChangeIDTypes(@"CREATE TABLE [Tags] (
                              [ID] image NOT NULL
                            , [Name] text NULL
                            , [Type] bigint NOT NULL
                            , [Color] text NULL
                            , [Culture] text DEFAULT ('ru-RU') NOT NULL
                            , CONSTRAINT [sqlite_autoindex_Tags_1] PRIMARY KEY ([ID])
                            );",
                            "Tags",
                            migrationBuilder);

            // Re-enable foreign key checks
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

        private static void ChangeIDTypes(string originalSql, string tableName, MigrationBuilder migrationBuilder)
        {
            // Rename old Day table to temp
            migrationBuilder.Sql($"ALTER TABLE {tableName} RENAME TO _{tableName};");

            // Recreate table with new types for Day
            migrationBuilder.Sql(originalSql.Replace("image", "text", StringComparison.Ordinal));

            // Copy data from backup
            migrationBuilder.Sql(@$"INSERT INTO {tableName} SELECT * FROM [_{tableName}];");

            // Drop backup
            migrationBuilder.Sql($"DROP TABLE [_{tableName}];");
        }

        private static string GetColumnUpdateToText(string tableName, string columnName)
        {
            string statement = $@"UPDATE {tableName}
SET {columnName} = hex(substr({columnName}, 4, 1)) ||
                   hex(substr({columnName}, 3, 1)) ||
                   hex(substr({columnName}, 2, 1)) ||
                   hex(substr({columnName}, 1, 1)) || '-' ||
                   hex(substr({columnName}, 6, 1)) ||
                   hex(substr({columnName}, 5, 1)) || '-' ||
                   hex(substr({columnName}, 8, 1)) ||
                   hex(substr({columnName}, 7, 1)) || '-' ||
                   hex(substr({columnName}, 9, 2)) || '-' ||
                   hex(substr({columnName}, 11, 6))
WHERE typeof({columnName}) == 'blob';";

            return statement;
        }
    }
}
