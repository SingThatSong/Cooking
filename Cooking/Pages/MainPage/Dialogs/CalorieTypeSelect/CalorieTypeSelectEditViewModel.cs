using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.MainPage.Dialogs.Model.CalorieTypeSelect;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class CalorieTypeSelectEditViewModel : INotifyPropertyChanged
    {
        public bool DialogResultOk { get; set; }

        public CalorieTypeSelectEditViewModel(IEnumerable<CalorieTypeSelection> selectedTypes)
        {
            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            AllValues = new ObservableCollection<CalorieTypeSelection>(Enum.GetValues(typeof(CalorieType)).Cast<CalorieType>().Select(x => new CalorieTypeSelection() { CalorieType = x }));
            AllValues.Insert(0, CalorieTypeSelection.Any);

            if (selectedTypes != null)
            {
                foreach (var tag in selectedTypes)
                {
                    AllValues.Single(x => x.CalorieType == tag.CalorieType).IsSelected = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public ObservableCollection<CalorieTypeSelection> AllValues { get; set; }

    }
}