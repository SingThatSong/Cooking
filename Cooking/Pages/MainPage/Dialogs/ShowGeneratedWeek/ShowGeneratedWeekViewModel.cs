using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.MainPage.Dialogs.Model;
using Cooking.Pages.Recepies;
using Data.Context;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
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

            ReturnCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    ReturnBack = true;
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

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


            DeleteRecipeManuallyCommand = new DelegateCommand<DayPlan>(day =>
            {
                day.SpecificRecipe = null;
            });

            SetRecipeManuallyCommand = new DelegateCommand<DayPlan>(async (day) =>
            {
                var viewModel = new RecipeSelectViewModel(day);

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

            GetAlternativeRecipe = new DelegateCommand<DayPlan>((day) =>
            {
                if (day.Recipe.Name == day.RecipeAlternatives.Last().Name)
                {
                    day.Recipe = day.RecipeAlternatives.First();
                }

                day.Recipe = day.RecipeAlternatives.SkipWhile(x => x.Name != day.Recipe.Name).Skip(1).First();
            },
                canExecute: (day) =>
                 day?.RecipeAlternatives?.Count > 1);

            ShowRecipe = new Lazy<DelegateCommand<DayPlan>>(
                () => new DelegateCommand<DayPlan>(async (day) => {

                    if (day.Recipe.Ingredients.Count == 0 && day.Recipe.IngredientGroups.Count == 0)
                    {
                        using (var context = new CookingContext())
                        {
                            var recipe = context.Recipies.Include(x => x.IngredientGroups)
                                                        .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.Ingredient)
                                                    .Include(x => x.Ingredients)
                                                        .ThenInclude(x => x.Ingredient)
                                                    .Single(x => x.ID == day.Recipe.ID);

                            AutoMapper.Mapper.Map(recipe, day.Recipe);
                        }
                    }

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

        private readonly Random Random = new Random();

        public bool IsDialogResultOK { get; set; }
        public bool ReturnBack { get; set; }

        public DateTime WeekStart { get; }
        public DateTime WeekEnd { get; }

        public IEnumerable<DayPlan> Days { get; }

        public Lazy<DelegateCommand<DayPlan>> ShowRecipe { get; }
        public DelegateCommand<DayPlan> GetAlternativeRecipe { get; }
        public DelegateCommand<DayPlan> DeleteRecipeManuallyCommand { get; }
        public DelegateCommand<DayPlan> SetRecipeManuallyCommand { get; }
        public Lazy<DelegateCommand> ReturnCommand { get; }
        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
