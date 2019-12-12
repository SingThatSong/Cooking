using Cooking.Pages.Dialogs;

using Data.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.Pages.ViewModel
{
    public partial class CalorieTypeSelectEditViewModel : OkCancelViewModel
    {
        public ObservableCollection<CalorieTypeSelection> AllValues { get; }
        public CalorieTypeSelectEditViewModel() : this(null) { }

        public CalorieTypeSelectEditViewModel(IEnumerable<CalorieTypeSelection>? selectedTypes) : base()
        {
            AllValues = new ObservableCollection<CalorieTypeSelection>(
                Enum.GetValues(typeof(CalorieType))
                .Cast<CalorieType>()
                .Select(x => new CalorieTypeSelection() { CalorieType = x })
            );

            AllValues.Insert(0, CalorieTypeSelection.Any);

            if (selectedTypes != null)
            {
                foreach (var tag in selectedTypes)
                {
                    AllValues.Single(x => x.CalorieType == tag.CalorieType).IsSelected = true;
                }
            }
        }
    }
}