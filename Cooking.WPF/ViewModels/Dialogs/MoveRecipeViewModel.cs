using Cooking.WPF.Services;
using System;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// View model for dialog of moving recipe to another week.
    /// </summary>
    public partial class MoveRecipeViewModel : OkCancelViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveRecipeViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        public MoveRecipeViewModel(DialogService dialogService, ILocalization localization)
            : base(dialogService)
        {
            WhereMoveRecipeCaption = localization.GetLocalizedString("WhereMoveRecipe");
        }

        /// <summary>
        /// Gets caption for WhereToMove.
        /// </summary>
        public string? WhereMoveRecipeCaption { get; }

        /// <summary>
        /// Gets or sets selected day of week on next week to move recipe to.
        /// </summary>
        public DayOfWeek? SelectedDay { get; set; }

        /// <inheritdoc/>
        protected override bool CanOk() => SelectedDay.HasValue;
    }
}