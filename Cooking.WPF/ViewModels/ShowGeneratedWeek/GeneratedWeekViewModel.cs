using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.Views;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using WPF.Commands;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for generated week.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class GeneratedWeekViewModel : INavigationAware
    {
        private readonly DialogService dialogService;
        private readonly IRegionManager regionManager;
        private readonly RecipeService recipeService;
        private readonly IContainerExtension container;
        private readonly DayService dayService;
        private NavigationContext? navigationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedWeekViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="regionManager">Region manager for Prism navigation.</param>
        /// <param name="recipeService">Recipe service dependency.</param>
        /// <param name="container">IoC container.</param>
        /// <param name="dayService">Day service dependency.</param>
        public GeneratedWeekViewModel(DialogService dialogService,
                                      IRegionManager regionManager,
                                      RecipeService recipeService,
                                      IContainerExtension container,
                                      DayService dayService)
        {
            this.dialogService = dialogService;
            this.regionManager = regionManager;
            this.recipeService = recipeService;
            this.container = container;
            this.dayService = dayService;

            ReturnCommand = new DelegateCommand(Return);
            DeleteRecipeManuallyCommand = new DelegateCommand<DayPlan>(DeleteRecipeManually);
            SetRecipeManuallyCommand = new AsyncDelegateCommand<DayPlan>(SetRecipeManuallyAsync);
            SetGarnishManuallyCommand = new AsyncDelegateCommand<DayPlan>(SetGarnishManuallyAsync, canExecute: (day) => day?.Recipe?.Garnishes.Count > 1);
            GetAlternativeRecipeCommand = new DelegateCommand<DayPlan>(GetAlternativeRecipe, canExecute: (day) => day?.RecipeAlternatives?.Count > 1);
            GetAlternativeGarnishCommand = new DelegateCommand<DayPlan>(GetAlternativeGarnish, canExecute: (day) => day?.Recipe?.Garnishes.Count > 1);
            ShowRecipeCommand = new DelegateCommand<Guid>(ShowRecipe);
            CloseCommand = new DelegateCommand(Close);
            OkCommand = new AsyncDelegateCommand(OkAsync);
        }

        /// <summary>
        /// Gets days in generated week.
        /// </summary>
        public IEnumerable<DayPlan>? Days { get; private set; }

        /// <summary>
        /// Gets first day of week.
        /// </summary>
        public DateTime WeekStart { get; private set; }

        /// <summary>
        /// Gets command to show recipe details.
        /// </summary>
        public DelegateCommand<Guid> ShowRecipeCommand { get; }

        /// <summary>
        /// Gets command to change current recipe to alternative.
        /// </summary>
        public DelegateCommand<DayPlan> GetAlternativeRecipeCommand { get; }

        /// <summary>
        /// Gets command to change current garnish to alternative.
        /// </summary>
        public DelegateCommand<DayPlan> GetAlternativeGarnishCommand { get; }

        /// <summary>
        /// Gets command to remove manually selected recipe.
        /// </summary>
        public DelegateCommand<DayPlan> DeleteRecipeManuallyCommand { get; }

        /// <summary>
        /// Gets command to set recipe manually.
        /// </summary>
        public AsyncDelegateCommand<DayPlan> SetRecipeManuallyCommand { get; }

        /// <summary>
        /// Gets command to set garnish manually.
        /// </summary>
        public AsyncDelegateCommand<DayPlan> SetGarnishManuallyCommand { get; }

        /// <summary>
        /// Gets command to return to previous window.
        /// </summary>
        public DelegateCommand ReturnCommand { get; }

        /// <summary>
        /// Gets command to accept generated week.
        /// </summary>
        public AsyncDelegateCommand OkCommand { get; }

        /// <summary>
        /// Gets command to close current dialog.
        /// </summary>
        public DelegateCommand CloseCommand { get; }

        /// <inheritdoc/>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.navigationContext = navigationContext;
            Days = (IEnumerable<DayPlan>)navigationContext.Parameters[nameof(Days)];
            WeekStart = (DateTime)navigationContext.Parameters[nameof(WeekStart)];
        }

        /// <inheritdoc/>
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        /// <inheritdoc/>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void Close() => regionManager.NavigateMain(nameof(WeekView));

        private async Task OkAsync()
        {
            var daysDictionary = Days!.ToDictionary(x => x.DayOfWeek, x => (x.SpecificRecipe?.ID ?? x.Recipe?.ID, x.Garnish?.ID));
            await dayService.CreateWeekAsync(WeekStart, daysDictionary);

            regionManager.NavigateMain(view: nameof(WeekView),
                                       parameters: (Consts.ReloadWeekParameter, true));
        }

        private void ShowRecipe(Guid recipeID)
        {
            regionManager.NavigateMain(
                view: nameof(RecipeView),
                parameters: (nameof(RecipeViewModel.Recipe), recipeID));
        }

        private void GetAlternativeRecipe(DayPlan day)
        {
            day.Recipe = day.Recipe!.Name == day.RecipeAlternatives?.Last().Name
                                                                            ? day.RecipeAlternatives?[0]
                                                                            : day.RecipeAlternatives?.SkipWhile(x => x.Name != day.Recipe.Name).Skip(1).First();

            day.Garnish = day.Recipe!.Garnishes.RandomElement();
        }

        private void GetAlternativeGarnish(DayPlan day)
        {
            DayPlanRecipe? lastGarnish = day.Garnish;

            do
            {
                day.Garnish = day.Recipe!.Garnishes.RandomElement();
            }
            while (day.Garnish == lastGarnish);
        }

        private async Task SetRecipeManuallyAsync(DayPlan day)
        {
            RecipeSelectViewModel? viewModel = container.Resolve<RecipeSelectViewModel>((typeof(DayPlan), day));
            await dialogService.ShowCustomMessageAsync<RecipeSelectView, RecipeSelectViewModel>(content: viewModel);

            if (viewModel.DialogResultOk)
            {
                day.SpecificRecipe = recipeService.GetMapped<DayPlanRecipe>(viewModel.SelectedRecipe!.ID);
            }
        }

        private async Task SetGarnishManuallyAsync(DayPlan day)
        {
            RecipeSelectViewModel? viewModel = container.Resolve<RecipeSelectViewModel>(
                                                            (typeof(DayPlan), day),
                                                            (typeof(bool), true)); // garnishSelect

            await dialogService.ShowCustomMessageAsync<RecipeSelectView, RecipeSelectViewModel>(content: viewModel);

            if (viewModel.DialogResultOk)
            {
                day.Garnish = day.Recipe!.Garnishes.First(x => x.ID == viewModel.SelectedRecipe!.ID);
            }
        }

        private void DeleteRecipeManually(DayPlan day) => day.SpecificRecipe = null;

        private void Return() => navigationContext?.NavigationService.Journal.GoBack();
    }
}
