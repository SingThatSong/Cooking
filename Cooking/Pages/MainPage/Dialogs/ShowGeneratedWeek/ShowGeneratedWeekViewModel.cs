using Cooking.Commands;
using Cooking.Pages.Dialogs;
using Cooking.ServiceLayer.Projections;
using MahApps.Metro.Controls.Dialogs;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cooking.Pages
{
    public class ShowGeneratedWeekViewModel : OkCancelViewModel
    {
        private readonly DialogUtils dialogUtils;

        public ShowGeneratedWeekViewModel(DateTime weekStart, 
                                          DateTime weekEnd, 
                                          IEnumerable<DayPlan> days,
                                          DialogUtils dialogUtils) : base()
        {
            Debug.Assert(dialogUtils != null);

            WeekStart = weekStart;
            WeekEnd = weekEnd;

            Days = days;
            this.dialogUtils = dialogUtils;
            ReturnCommand = new DelegateCommand(async () => {
                    ReturnBack = true;
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this).ConfigureAwait(false);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current).ConfigureAwait(false);
                });

            DeleteRecipeManuallyCommand = new DelegateCommand<DayPlan>(day =>
            {
                day.SpecificRecipe = null;
            });

            SetRecipeManuallyCommand = new DelegateCommand<DayPlan>(async (day) =>
            {
                var viewModel = new RecipeSelectViewModel(dialogUtils, day);

                await dialogUtils.ShowCustomMessageAsync<RecipeSelect, RecipeSelectViewModel>(content: viewModel).ConfigureAwait(false);

                if (viewModel.DialogResultOk)
                {
                    var recipeId = viewModel.SelectedRecipeID;
                    day.SpecificRecipe = RecipeService.GetProjection<RecipeSlim>(recipeId);
                }
            });

            GetAlternativeRecipe = new DelegateCommand<DayPlan>((day) =>
            {
                if (day.Recipe.Name == day.RecipeAlternatives.Last().Name)
                {
                    day.Recipe = day.RecipeAlternatives.First();
                }
                else
                {
                    day.Recipe = day.RecipeAlternatives.SkipWhile(x => x.Name != day.Recipe.Name).Skip(1).First();
                }
            },
                canExecute: (day) =>
                 day?.RecipeAlternatives?.Count > 1);

            ShowRecipe = new DelegateCommand<DayPlan>(async (day) => {
                var viewModel = new RecipeViewModel(day.Recipe.ID, dialogUtils);
                await dialogUtils.ShowCustomMessageAsync<RecipeView, RecipeViewModel>(content: viewModel).ConfigureAwait(false);
            });
        }

        public bool ReturnBack { get; set; }

        public DateTime WeekStart { get; }
        public DateTime WeekEnd { get; }

        public IEnumerable<DayPlan> Days { get; }

        public DelegateCommand<DayPlan> ShowRecipe { get; }
        public DelegateCommand<DayPlan> GetAlternativeRecipe { get; }
        public DelegateCommand<DayPlan> DeleteRecipeManuallyCommand { get; }
        public DelegateCommand<DayPlan> SetRecipeManuallyCommand { get; }
        public DelegateCommand ReturnCommand { get; }
    }
}
