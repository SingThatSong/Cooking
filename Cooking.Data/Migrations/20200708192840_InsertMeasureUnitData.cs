using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    /// <summary>
    /// Replacement of enum MeasureUnit to database table MeasureUnit.
    /// </summary>
    public partial class InsertMeasureUnitData : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO [MeasureUnit] ([ID], [Culture], [Name], [FullName])
                     VALUES ('35ACBB4B-B650-422A-808A-79415B03A80A', 'ru-RU', 'г', 'грамм');

                INSERT INTO [MeasureUnit] ([ID], [Culture], [Name], [FullName])
                     VALUES ('718451E2-E081-4869-B4A0-79090DFFF694', 'ru-RU', 'мл', 'миллилитр');

                INSERT INTO [MeasureUnit] ([ID], [Culture], [Name], [FullName])
                     VALUES ('1E3BB1C9-5851-403C-87BE-15052F3B179A', 'ru-RU', 'шт', 'штука');

                INSERT INTO [MeasureUnit] ([ID], [Culture], [Name], [FullName])
                     VALUES ('27620DB8-ECFD-49F1-AA12-7CBAC6038021', 'ru-RU', 'ст.л.', 'столовая ложка');

                INSERT INTO [MeasureUnit] ([ID], [Culture], [Name], [FullName])
                     VALUES ('682828C4-91B1-46A0-9C15-869BCDA83CE5', 'ru-RU', 'ч.л.', 'чайная ложка');

                INSERT INTO [MeasureUnit] ([ID], [Culture], [Name], [FullName])
                     VALUES ('8133EC33-9977-4E7E-A68C-10D47FCA0D1E', 'ru-RU', 'ст.', 'стакан');

                INSERT INTO [MeasureUnit] ([ID], [Culture], [Name], [FullName])
                     VALUES ('82AD6E97-CD08-4EE8-B962-F6121E165BDD', 'ru-RU', 'щепотка', 'щепотка');

                INSERT INTO [MeasureUnit] ([ID], [Culture], [Name], [FullName])
                     VALUES ('2EF9C659-3525-4F92-8231-D1677918EE7C', 'ru-RU', 'веточка', 'веточка');

                INSERT INTO [MeasureUnit] ([ID], [Culture], [Name], [FullName])
                     VALUES ('A7EB760E-8F99-44BD-A990-2A14AD6D9573', 'ru-RU', 'зубчик', 'зубчик');

                INSERT INTO [MeasureUnit] ([ID], [Culture], [Name], [FullName])
                     VALUES ('A072CB35-8FF7-4ACD-BCD6-23C7E3F64D8A', 'ru-RU', 'пучок', 'пучок');

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = '35ACBB4B-B650-422A-808A-79415B03A80A'
                 WHERE [MeasureUnitID] = '1';

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = '718451E2-E081-4869-B4A0-79090DFFF694'
                 WHERE [MeasureUnitID] = '2';

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = '1E3BB1C9-5851-403C-87BE-15052F3B179A'
                 WHERE [MeasureUnitID] = '3';

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = '27620DB8-ECFD-49F1-AA12-7CBAC6038021'
                 WHERE [MeasureUnitID] = '4';

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = '682828C4-91B1-46A0-9C15-869BCDA83CE5'
                 WHERE [MeasureUnitID] = '5';

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = '8133EC33-9977-4E7E-A68C-10D47FCA0D1E'
                 WHERE [MeasureUnitID] = '6';

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = '82AD6E97-CD08-4EE8-B962-F6121E165BDD'
                 WHERE [MeasureUnitID] = '7';

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = '2EF9C659-3525-4F92-8231-D1677918EE7C'
                 WHERE [MeasureUnitID] = '8';

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = 'A7EB760E-8F99-44BD-A990-2A14AD6D9573'
                 WHERE [MeasureUnitID] = '9';

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = 'A072CB35-8FF7-4ACD-BCD6-23C7E3F64D8A'
                 WHERE [MeasureUnitID] = '10';

                UPDATE [RecipeIngredients] 
                   SET [MeasureUnitGuid] = upper(MeasureUnitGuid);
            ");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeasureUnitID",
                table: "RecipeIngredients",
                type: "INTEGER",
                nullable: true);
        }
    }
}
