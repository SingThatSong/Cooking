using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    public partial class MeasureUnitPluralization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullNamePluralization",
                table: "MeasureUnit",
                type: "TEXT",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.Sql(
              @"UPDATE [MeasureUnit] 
                SET [FullNamePluralization] = ('грамм|грамма|грамм')
                     WHERE ID = '35ACBB4B-B650-422A-808A-79415B03A80A';

                UPDATE [MeasureUnit] 
                  SET [FullNamePluralization] = ('миллилитр|миллилитра|миллилитров')
                     WHERE ID = '718451E2-E081-4869-B4A0-79090DFFF694';

                UPDATE [MeasureUnit] 
                  SET [FullNamePluralization] = ('штука|штуки|штук')
                     WHERE ID = '1E3BB1C9-5851-403C-87BE-15052F3B179A';

                UPDATE [MeasureUnit] 
                  SET [FullNamePluralization] = ('столовая ложка|столовых ложки|столовых ложек')
                     WHERE ID = '27620DB8-ECFD-49F1-AA12-7CBAC6038021';

                UPDATE [MeasureUnit] 
                  SET [FullNamePluralization] = ('чайная ложка|чайных ложки|чайных ложек')
                     WHERE ID = '682828C4-91B1-46A0-9C15-869BCDA83CE5';

                UPDATE [MeasureUnit] 
                  SET [FullNamePluralization] = ('стакан|стакана|стаканов')
                     WHERE ID = '8133EC33-9977-4E7E-A68C-10D47FCA0D1E';

                UPDATE [MeasureUnit] 
                  SET [FullNamePluralization] = ('щепотка|щепотки|щепоток')
                     WHERE ID = '82AD6E97-CD08-4EE8-B962-F6121E165BDD';

                UPDATE [MeasureUnit] 
                  SET [FullNamePluralization] = ('веточка|веточки|веточек')
                     WHERE ID = '2EF9C659-3525-4F92-8231-D1677918EE7C';

                UPDATE [MeasureUnit] 
                  SET [FullNamePluralization] = ('зубчик|зубчика|зубчиков')
                     WHERE ID = 'A7EB760E-8F99-44BD-A990-2A14AD6D9573';

                UPDATE [MeasureUnit] 
                  SET [FullNamePluralization] = ('пучок|пучка|пучков')
                     WHERE ID = 'A072CB35-8FF7-4ACD-BCD6-23C7E3F64D8A';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullNamePluralization",
                table: "MeasureUnit");
        }
    }
}
