using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.WPF.Views
{
    public partial class MoveRecipeViewModel : OkCancelViewModel
    {
        public string? WhereMoveRecipe { get; }

        public DayOfWeek? SelectedDay { get; set; }
        public ObservableCollection<DayOfWeek> DaysOfWeek { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveRecipeViewModel"/> class.
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="localization"></param>
        public MoveRecipeViewModel(DialogService dialogService, ILocalization localization)
            : base(dialogService)
        {
            WhereMoveRecipe = localization.GetLocalizedString("WhereMoveRecipe");
        }

        protected override bool CanOk() => SelectedDay.HasValue;
    }
}