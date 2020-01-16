using Microsoft.EntityFrameworkCore.Migrations;

namespace Cooking.Data.Migrations
{
    public partial class RemoveDayNotRemovesWeek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA writable_schema=1;");
            migrationBuilder.Sql(@"UPDATE sqlite_master SET sql='CREATE TABLE ""Weeks"" 
(
    ""ID""    BLOB NOT NULL,
    ""Start"" TEXT NOT NULL,
    ""End""   TEXT NOT NULL,
    ""MondayID""  BLOB,
    ""TuesdayID"" BLOB,
    ""WednesdayID""   BLOB,
    ""ThursdayID""    BLOB,
    ""FridayID""  BLOB,
    ""SaturdayID""    BLOB,
    ""SundayID""  BLOB,
    CONSTRAINT ""PK_Weeks"" PRIMARY KEY(""ID""),
    CONSTRAINT ""FK_Weeks_Day_MondayID"" FOREIGN KEY(""MondayID"") REFERENCES ""Day""(""ID"") ON DELETE SET NULL,
    CONSTRAINT ""FK_Weeks_Day_SundayID"" FOREIGN KEY(""SundayID"") REFERENCES ""Day""(""ID"") ON DELETE SET NULL,
    CONSTRAINT ""FK_Weeks_Day_FridayID"" FOREIGN KEY(""FridayID"") REFERENCES ""Day""(""ID"") ON DELETE SET NULL,
    CONSTRAINT ""FK_Weeks_Day_SaturdayID"" FOREIGN KEY(""SaturdayID"") REFERENCES ""Day""(""ID"") ON DELETE SET NULL,
    CONSTRAINT ""FK_Weeks_Day_ThursdayID"" FOREIGN KEY(""ThursdayID"") REFERENCES ""Day""(""ID"") ON DELETE SET NULL,
    CONSTRAINT ""FK_Weeks_Day_TuesdayID"" FOREIGN KEY(""TuesdayID"") REFERENCES ""Day""(""ID"") ON DELETE SET NULL,
    CONSTRAINT ""FK_Weeks_Day_WednesdayID"" FOREIGN KEY(""WednesdayID"") REFERENCES ""Day""(""ID"") ON DELETE SET NULL
)' 
    WHERE type='table' AND name='Weeks';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
