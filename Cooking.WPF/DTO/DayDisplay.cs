using Cooking.Data.Model;
using System;
using System.ComponentModel;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// Dto for day displaying.
    /// </summary>
    public class DayDisplay : Entity, INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether dinner on a given day was cooked.
        /// </summary>
        public bool DinnerWasCooked { get; set; }

        /// <summary>
        /// Gets or sets day of week for given day.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Gets or sets dinner that selected to be cooked on given day.
        /// </summary>
        public RecipeListViewDto? Dinner { get; set; }
    }
}