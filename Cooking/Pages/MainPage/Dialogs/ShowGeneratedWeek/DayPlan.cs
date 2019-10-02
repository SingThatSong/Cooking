using Cooking.DTO;
using Cooking.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cooking.Pages.MainPage.Dialogs
{
    public class DayPlan : INotifyPropertyChanged
    {
        // Рецепт, указанный вручную
        public RecipeSlim? SpecificRecipe { get; set; }
        public RecipeSlim Recipe { get; set; }
        public List<RecipeSlim> RecipeAlternatives { get; set; }

        public int? MinRating { get; set; }
        public int? MaxComplexity { get; set; }
        public bool OnlyNewRecipies { get; set; }
        public bool IsSelected { get; set; } = true;
        public string DayName { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
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
