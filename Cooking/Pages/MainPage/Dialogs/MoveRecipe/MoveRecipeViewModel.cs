using Cooking.Commands;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class MoveRecipeViewModel : INotifyPropertyChanged
    {
        public MoveRecipeViewModel()
        {
            SelectDayCommand = new Lazy<DelegateCommand<SelectDay>>(
            () => new DelegateCommand<SelectDay>(recipe => {

                foreach (var d in DaysOfWeek)
                {
                    d.IsSelected = false;
                }

                recipe.IsSelected = true;
            }));

            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {

                    if (DaysOfWeek.Any(x => x.IsSelected))
                    {
                        DialogResultOk = true;
                        var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                        await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                    }

                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));
        }

        public Lazy<DelegateCommand<SelectDay>> SelectDayCommand { get; }
        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public bool DialogResultOk { get; set; }
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

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [AddINotifyPropertyChangedInterface]
    public class SelectDay
    {
        public DayOfWeek WeekDay { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}