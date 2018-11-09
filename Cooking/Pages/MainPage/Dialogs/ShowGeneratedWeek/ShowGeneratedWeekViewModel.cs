using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.MainPage.Dialogs.Model;
using Cooking.Pages.Recepies;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.MainPage.Dialogs
{
    public class ShowGeneratedWeekViewModel : INotifyPropertyChanged
    {
        public ShowGeneratedWeekViewModel(DateTime weekStart, DateTime weekEnd, IEnumerable<DayPlan> days)
        {
            WeekStart = weekStart;
            WeekEnd = weekEnd;

            Days = days;

            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    IsDialogResultOK = true;
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));
            DeleteRecipeManuallyCommand = new DelegateCommand<DayPlan>(async (day) =>
            {
                day.SpecificRecipe = null;
            });

            SetRecipeManuallyCommand = new DelegateCommand<DayPlan>(async (day) =>
            {
                var viewModel = new RecipeSelectViewModel();

                var dialog = new CustomDialog()
                {
                    Content = new RecipeSelectView()
                    {
                        DataContext = viewModel
                    }
                };

                var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

                await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

                do
                {
                    await dialog.WaitUntilUnloadedAsync();
                }
                while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

                if (viewModel.DialogResultOk)
                {
                    day.SpecificRecipe = viewModel.Recipies.Value.Single(x => x.IsSelected);
                }
            });

            GetAlternativeRecipe = new DelegateCommand<DayPlan>(async (day) =>
            {
                RecipeDTO newRecipe;
                do
                {
                    newRecipe = day.RecipeAlternatives[Random.Next(0, day.RecipeAlternatives.Count)];
                }
                while (day.Recipe == newRecipe);

                day.Recipe = newRecipe;
            },
                canExecute: (day) =>
                 day?.RecipeAlternatives?.Count > 1);

            ShowRecipe = new Lazy<DelegateCommand<DayPlan>>(
                () => new DelegateCommand<DayPlan>(async (day) => {
                    var viewModel = new RecipeViewModel(day.Recipe);

                    var dialog = new CustomDialog()
                    {
                        Content = new RecipeView()
                        {
                            DataContext = viewModel
                        }
                    };

                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();
                }));
        }

        private Random Random = new Random();

        public bool IsDialogResultOK { get; set; }

        public DateTime WeekStart { get; }
        public DateTime WeekEnd { get; }

        public IEnumerable<DayPlan> Days { get; }

        public Lazy<DelegateCommand<DayPlan>> ShowRecipe { get; }
        public DelegateCommand<DayPlan> GetAlternativeRecipe { get; }
        public DelegateCommand<DayPlan> DeleteRecipeManuallyCommand { get; }
        public DelegateCommand<DayPlan> SetRecipeManuallyCommand { get; }
        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
