using AutoMapper;
using Cooking.Commands;
using Cooking.Pages.Dialogs;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.Projections;
using Cooking.WPF.Helpers;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public class ShowGeneratedWeekViewModel : INavigationAware
    {
        private readonly DialogService dialogUtils;
        private readonly IRegionManager regionManager;
        private readonly RecipeService recipeService;
        private readonly IContainerExtension container;
        private readonly WeekService weekService;
        private NavigationContext? navigationContext;

        public IEnumerable<DayPlan>? Days { get; private set; }
        public DateTime WeekStart { get; private set; }

        public DelegateCommand<Guid> ShowRecipeCommand { get; }
        public DelegateCommand<DayPlan> GetAlternativeRecipeCommand { get; }
        public DelegateCommand<DayPlan> DeleteRecipeManuallyCommand { get; }
        public DelegateCommand<DayPlan> SetRecipeManuallyCommand { get; }
        public DelegateCommand ReturnCommand { get; }
        public DelegateCommand OkCommand { get; }
        public DelegateCommand CloseCommand { get; }

        public ShowGeneratedWeekViewModel(DialogService dialogUtils, 
                                          IRegionManager regionManager, 
                                          RecipeService recipeService, 
                                          IContainerExtension container,
                                          WeekService weekService) : base()
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(regionManager != null);
            Debug.Assert(recipeService != null);
            Debug.Assert(container != null);
            Debug.Assert(weekService != null);

            this.dialogUtils            = dialogUtils;
            this.regionManager          = regionManager;
            this.recipeService          = recipeService;
            this.container              = container;
            this.weekService            = weekService;

            ReturnCommand               = new DelegateCommand(Return);
            DeleteRecipeManuallyCommand = new DelegateCommand<DayPlan>(DeleteRecipeManually);
            SetRecipeManuallyCommand    = new DelegateCommand<DayPlan>(SetRecipeManually);
            GetAlternativeRecipeCommand = new DelegateCommand<DayPlan>(GetAlternativeRecipe, canExecute: (day) => day?.RecipeAlternatives?.Count > 1);
            ShowRecipeCommand           = new DelegateCommand<Guid>(ShowRecipe);
            CloseCommand                = new DelegateCommand(Close);
            OkCommand                   = new DelegateCommand(Ok);
        }

        private void Close()
        {
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(MainView));
        }

        private async void Ok()
        {            
            var daysDictionary = Days.ToDictionary(x => x.DayOfWeek, x => x.SpecificRecipe?.ID ?? x.Recipe?.ID);
            await weekService.CreateWeekAsync(WeekStart, daysDictionary).ConfigureAwait(false);

            var parameters = new NavigationParameters
            {
                { Consts.ReloadWeekParameter, true }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(MainView), parameters);
        }

        private void ShowRecipe(Guid recipeId)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecipeViewModel.Recipe), recipeId }
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
            var viewModel = new RecipeSelectViewModel(dialogUtils, 
                                                      recipeService, 
                                                      container.Resolve<IMapper>(), 
                                                      container.Resolve<RecipeFiltrator>(),
                                                      container.Resolve<ILocalization>(),
                                                      day);

            await dialogUtils.ShowCustomMessageAsync<RecipeSelect, RecipeSelectViewModel>(content: viewModel).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                day.SpecificRecipe = recipeService.GetProjected<RecipeSlim>(viewModel.SelectedRecipeID!.Value);
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
