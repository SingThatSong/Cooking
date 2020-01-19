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

        public DelegateCommand<SelectDay> SelectDayCommand { get; }
        public ObservableCollection<SelectDay> DaysOfWeek { get; }

        public MoveRecipeViewModel(DialogService dialogService, ILocalization localization) : base(dialogService)
        {
            SelectDayCommand = new DelegateCommand<SelectDay>(recipe =>
            {
                foreach (SelectDay d in DaysOfWeek)
                {
                    d.IsSelected = false;
                }

                recipe.IsSelected = true;
            });

            WhereMoveRecipe = localization.GetLocalizedString("WhereMoveRecipe");
            DaysOfWeek = new ObservableCollection<SelectDay>()
            {
                new SelectDay { Name = localization.GetLocalizedString("DayOfWeek_Monday"), WeekDay = DayOfWeek.Monday, IsSelected = true },
                new SelectDay { Name = localization.GetLocalizedString("DayOfWeek_Tuesday"), WeekDay = DayOfWeek.Tuesday },
                new SelectDay { Name = localization.GetLocalizedString("DayOfWeek_Wednesday"), WeekDay = DayOfWeek.Wednesday },
                new SelectDay { Name = localization.GetLocalizedString("DayOfWeek_Thursday"), WeekDay = DayOfWeek.Thursday },
                new SelectDay { Name = localization.GetLocalizedString("DayOfWeek_Friday"), WeekDay = DayOfWeek.Friday },
                new SelectDay { Name = localization.GetLocalizedString("DayOfWeek_Saturday"), WeekDay = DayOfWeek.Saturday },
                new SelectDay { Name = localization.GetLocalizedString("DayOfWeek_Sunday"), WeekDay = DayOfWeek.Sunday }
            };
        }

        protected override bool CanOk() => DaysOfWeek.Any(x => x.IsSelected);

    }
}