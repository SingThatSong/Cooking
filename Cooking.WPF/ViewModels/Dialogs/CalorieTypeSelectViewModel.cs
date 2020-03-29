using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for selecting calorie types.
    /// </summary>
    public partial class CalorieTypeSelectViewModel : OkCancelViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalorieTypeSelectViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency to close dialog.</param>
        /// <param name="localization">Localization provider for calorie type's names.</param>
        /// <param name="selectedTypes">Already selected types to show in interface.</param>
        public CalorieTypeSelectViewModel(DialogService dialogService, ILocalization localization, IEnumerable<CalorieTypeSelection>? selectedTypes)
            : base(dialogService)
        {
            AllValues = new ObservableCollection<CalorieTypeSelection>(
                Enum.GetValues(typeof(CalorieType))
                .Cast<CalorieType>()
                .Where(x => x != CalorieType.None)
                .Select(x => new CalorieTypeSelection()
                {
                    CalorieType = x,
                    Name = localization.GetLocalizedString(x)
                })
            );

            AllValues.Insert(0, CalorieTypeSelection.Any);

            if (selectedTypes != null)
            {
                foreach (CalorieTypeSelection tag in selectedTypes)
                {
                    AllValues.Single(x => x.CalorieType == tag.CalorieType).IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Gets all calorie types to select from.
        /// </summary>
        public ObservableCollection<CalorieTypeSelection> AllValues { get; }
    }
}