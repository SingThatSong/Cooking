using Cooking.WPF.Views;

using Cooking.Data.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.WPF.ViewModels
{
    public partial class CalorieTypeSelectViewModel : OkCancelViewModel
    {
        public ObservableCollection<CalorieTypeSelection> AllValues { get; }

        public CalorieTypeSelectViewModel(DialogService dialogService, IEnumerable<CalorieTypeSelection>? selectedTypes) : base(dialogService)
        {
            AllValues = new ObservableCollection<CalorieTypeSelection>(
                Enum.GetValues(typeof(CalorieType))
                .Cast<CalorieType>()
                .Select(x => new CalorieTypeSelection() { CalorieType = x })
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