using Cooking.Data.Model;
using Cooking.WPF.DTO;
using Cooking.WPF.Helpers;
using Cooking.WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.WPF.ViewModels
{
    public partial class CalorieTypeSelectViewModel : OkCancelViewModel
    {
        public ObservableCollection<CalorieTypeSelection> AllValues { get; }

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
    }
}