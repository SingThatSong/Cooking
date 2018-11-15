using Cooking.DTO;
using Cooking.Pages.MainPage.Dialogs.Model.CalorieTypeSelect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooking.Pages.MainPage.Dialogs.Model
{
    public class DayPlan : INotifyPropertyChanged
    {
        // Рецепт, указанный вручную
        public RecipeDTO SpecificRecipe { get; set; }
        public RecipeDTO Recipe { get; set; }
        public List<RecipeDTO> RecipeAlternatives { get; set; }

        public int? MinRating { get; set; }
        public int? MaxComplexity { get; set; }
        public bool OnlyNewRecipies { get; set; }
        public bool IsSelected { get; set; } = true;
        public string DayName { get; set; }
        public ObservableCollection<TagDTO> NeededMainIngredients { get; set; } = new ObservableCollection<TagDTO>() { TagDTO.Any };
        public ObservableCollection<TagDTO> NeededDishTypes { get; set; } = new ObservableCollection<TagDTO>() { TagDTO.Any };
        public ObservableCollection<CalorieTypeSelection> CalorieTypes { get; set; } 
            = new ObservableCollection<CalorieTypeSelection>()
            {
                CalorieTypeSelection.Any
            };

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
