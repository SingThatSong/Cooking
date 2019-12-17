using Cooking.Commands;
using Cooking.Pages.Dialogs;
using Cooking.ServiceLayer.Projections;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public class ShowGeneratedWeekViewModel : INavigationAware
    {
        private readonly DialogService dialogUtils;
        private readonly IRegionManager regionManager;
        private NavigationContext? navigationContext;

        public IEnumerable<DayPlan>? Days { get; private set; }
        public DateTime WeekStart { get; private set; }

        public DelegateCommand<DayPlan> ShowRecipeCommand { get; }
        public DelegateCommand<DayPlan> GetAlternativeRecipeCommand { get; }
        public DelegateCommand<DayPlan> DeleteRecipeManuallyCommand { get; }
        public DelegateCommand<DayPlan> SetRecipeManuallyCommand { get; }
        public DelegateCommand ReturnCommand { get; }
        public AsyncDelegateCommand OkCommand { get; }
        public DelegateCommand CloseCommand { get; }

        public ShowGeneratedWeekViewModel(DialogService dialogUtils, IRegionManager regionManager) : base()
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(regionManager != null);

            this.dialogUtils            = dialogUtils;
            this.regionManager          = regionManager;
            ReturnCommand               = new DelegateCommand(Return);
            DeleteRecipeManuallyCommand = new DelegateCommand<DayPlan>(DeleteRecipeManually);
            SetRecipeManuallyCommand    = new DelegateCommand<DayPlan>(SetRecipeManually);
            GetAlternativeRecipeCommand = new DelegateCommand<DayPlan>(GetAlternativeRecipe, canExecute: (day) => day?.RecipeAlternatives?.Count > 1);
            ShowRecipeCommand           = new DelegateCommand<DayPlan>(ShowRecipe);
            CloseCommand                = new DelegateCommand(Close);
            OkCommand                   = new AsyncDelegateCommand(Ok, freezeWhenBusy: true);
        }

        private void Close()
        {
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(MainPage));
        }

        private async Task Ok()
        {            
            var daysDictionary = Days.ToDictionary(x => x.DayOfWeek, x => x.SpecificRecipe?.ID ?? x.Recipe?.ID);
            await WeekService.CreateWeekAsync(WeekStart, daysDictionary).ConfigureAwait(false);

            var parameters = new NavigationParameters
            {
                { Consts.ReloadWeekParameter, true }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(MainPage), parameters);
        }

        private async void ShowRecipe(DayPlan day)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecipeViewModel.Recipe), day.Recipe?.ID }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(RecipeView), parameters);
        }

        private static void GetAlternativeRecipe(DayPlan day)
        {
            if (day.Recipe!.Name == day.RecipeAlternatives.Last().Name)
            {
                day.Recipe = day.RecipeAlternatives.First();
            }
            else
            {
                day.Recipe = day.RecipeAlternatives.SkipWhile(x => x.Name != day.Recipe.Name).Skip(1).First();
            }
        }

        private async void SetRecipeManually(DayPlan day)
        {
            var viewModel = new RecipeSelectViewModel(dialogUtils, day);

            await dialogUtils.ShowCustomMessageAsync<RecipeSelect, RecipeSelectViewModel>(content: viewModel).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var recipeId = viewModel.SelectedRecipeID;
                day.SpecificRecipe = RecipeService.GetProjection<RecipeSlim>(recipeId);
            }
        }

        private static void DeleteRecipeManually(DayPlan day)
        {
            day.SpecificRecipe = null;
        }

        private void Return()
        {
            navigationContext?.NavigationService.Journal.GoBack();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.navigationContext = navigationContext;
            Days = (IEnumerable<DayPlan>)navigationContext.Parameters[nameof(Days)];
            WeekStart = (DateTime)navigationContext.Parameters[nameof(WeekStart)];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
