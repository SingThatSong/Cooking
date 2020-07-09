using Cooking.WPF.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// Dto for a day in week generation.
    /// </summary>
    public class DayPlan : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets recipe, selected explicitly, despite of suggested.
        /// </summary>
        public RecipeListViewDto? SpecificRecipe { get; set; }

        /// <summary>
        /// Gets or sets suggested recipe for a day.
        /// </summary>
        public RecipeListViewDto? Recipe { get; set; }

        /// <summary>
        /// Gets or sets alternatives for a suggested recipe.
        /// </summary>
        public List<RecipeListViewDto>? RecipeAlternatives { get; set; }

        /// <summary>
        /// Gets or sets setting: Minimal rating for recipe.
        /// </summary>
        public int? MinRating { get; set; }

        /// <summary>
        /// Gets or sets setting: Maximum complexity for recipe.
        /// </summary>
        public int? MaxComplexity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether setting: Suggest only new recipies.
        /// </summary>
        public bool OnlyNewRecipies { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this day will be used in week creation.
        /// </summary>
        public bool IsSelected { get; set; } = true;

        /// <summary>
        /// Gets or sets day name.
        /// </summary>
        public string? DayName { get; set; }

        /// <summary>
        /// Gets or sets day of week.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Gets or sets required main ingredients for a day.
        /// </summary>
        public ObservableCollection<TagEdit> NeededMainIngredients { get; set; } = new ObservableCollection<TagEdit>() { TagEdit.Any };

        /// <summary>
        /// Gets or sets required dish types for a day.
        /// </summary>
        public ObservableCollection<TagEdit> NeededDishTypes { get; set; } = new ObservableCollection<TagEdit>() { TagEdit.Any };

        /// <summary>
        /// Gets or sets required calorie types for a day.
        /// </summary>
        public ObservableCollection<CalorieTypeSelection> CalorieTypes { get; set; } = new ObservableCollection<CalorieTypeSelection>() { CalorieTypeSelection.Any };
    }
}
