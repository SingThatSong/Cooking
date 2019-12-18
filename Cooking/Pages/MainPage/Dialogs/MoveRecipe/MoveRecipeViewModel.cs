using Cooking.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.Pages
{
    public partial class MoveRecipeViewModel : OkCancelViewModel
    {
        public DelegateCommand<SelectDay> SelectDayCommand { get; }
        public ObservableCollection<SelectDay> DaysOfWeek { get; } = new ObservableCollection<SelectDay>()
        {
            new SelectDay { Name = "Понедельник", WeekDay = DayOfWeek.Monday, IsSelected = true },
            new SelectDay { Name = "Вторник", WeekDay = DayOfWeek.Tuesday },
            new SelectDay { Name = "Среда", WeekDay = DayOfWeek.Wednesday },
            new SelectDay { Name = "Четверг", WeekDay = DayOfWeek.Thursday },
            new SelectDay { Name = "Пятница", WeekDay = DayOfWeek.Friday },
            new SelectDay { Name = "Суббота", WeekDay = DayOfWeek.Saturday },
            new SelectDay { Name = "Воскресенье", WeekDay = DayOfWeek.Sunday }
        };

        public MoveRecipeViewModel()
        {
            SelectDayCommand = new DelegateCommand<SelectDay>(recipe =>
            {
                foreach (var d in DaysOfWeek)
                {
                    d.IsSelected = false;
                }

                recipe.IsSelected = true;
            });
        }

        protected override bool CanOk() => DaysOfWeek.Any(x => x.IsSelected);

    }
}