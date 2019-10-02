using Cooking.Helpers;
using Data.Model;

namespace Cooking.Pages.MainPage.Dialogs
{
    public class CalorieTypeSelection
    {
        public static readonly CalorieTypeSelection Any = new CalorieTypeSelection()
        {
            Name = "Любой"
        };

        private CalorieType calorieType;
        public CalorieType CalorieType
        {
            get => calorieType;
            set
            {
                calorieType = value;
                Name = value.Description();
            }
        }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
