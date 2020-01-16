using Cooking.WPF.Services;
using Cooking.Data.Model;

namespace Cooking.WPF.Views
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
        public string? Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
