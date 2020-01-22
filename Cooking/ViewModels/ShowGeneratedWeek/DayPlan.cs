using Cooking.WPF.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cooking.WPF.Views
{
    public class DayPlan : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Рецепт, указанный вручную
        public RecipeListViewDto? SpecificRecipe { get; set; }
        public RecipeListViewDto? Recipe { get; set; }

        public List<RecipeListViewDto>? RecipeAlternatives { get; set; }

        public int? MinRating { get; set; }
        public int? MaxComplexity { get; set; }
        public bool OnlyNewRecipies { get; set; }
        public bool IsSelected { get; set; } = true;
        public string? DayName { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public ObservableCollection<TagEdit> NeededMainIngredients { get; set; } = new ObservableCollection<TagEdit>() { TagEdit.Any };
        public ObservableCollection<TagEdit> NeededDishTypes { get; set; } = new ObservableCollection<TagEdit>() { TagEdit.Any };
        public ObservableCollection<CalorieTypeSelection> CalorieTypes { get; set; }
            = new ObservableCollection<CalorieTypeSelection>()
            {
                CalorieTypeSelection.Any
            };
    }
}
