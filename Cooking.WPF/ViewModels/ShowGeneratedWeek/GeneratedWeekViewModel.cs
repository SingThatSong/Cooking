﻿using AutoMapper;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cooking.WPF.Views
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
            SetRecipeManuallyCommand = new DelegateCommand<DayPlan>(SetRecipeManually);
            GetAlternativeRecipeCommand = new DelegateCommand<DayPlan>(GetAlternativeRecipe, canExecute: (day) => day?.RecipeAlternatives?.Count > 1);
            ShowRecipeCommand = new DelegateCommand<Guid>(ShowRecipe);
            CloseCommand = new DelegateCommand(Close);
            OkCommand = new DelegateCommand(Ok);
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
        /// Gets command to remove manually selected recipe.
        /// </summary>
        public DelegateCommand<DayPlan> DeleteRecipeManuallyCommand { get; }

        /// <summary>
        /// Gets command to set recipe manually.
        /// </summary>
        public DelegateCommand<DayPlan> SetRecipeManuallyCommand { get; }

        /// <summary>
        /// Gets command to return to previous window.
        /// </summary>
        public DelegateCommand ReturnCommand { get; }

        /// <summary>
        /// Gets command to accept generated week.
        /// </summary>
        public DelegateCommand OkCommand { get; }

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

        private void Close() => regionManager.RequestNavigate(Consts.MainContentRegion, nameof(WeekView));

        private async void Ok()
        {
            var daysDictionary = Days.ToDictionary(x => x.DayOfWeek, x => x.SpecificRecipe?.ID ?? x.Recipe?.ID);
            await dayService.CreateWeekAsync(WeekStart, daysDictionary);

            var parameters = new NavigationParameters
            {
                { Consts.ReloadWeekParameter, true }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(WeekView), parameters);
        }

        private void ShowRecipe(Guid recipeId)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecipeViewModel.Recipe), recipeId }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(RecipeView), parameters);
        }

        private void GetAlternativeRecipe(DayPlan day) => day.Recipe = day.Recipe!.Name == day.RecipeAlternatives.Last().Name
                                                                            ? day.RecipeAlternatives?[0]
                                                                            : day.RecipeAlternatives?.SkipWhile(x => x.Name != day.Recipe.Name).Skip(1).First();

        private async void SetRecipeManually(DayPlan day)
        {
            IMapper mapper = container.Resolve<IMapper>();
            var viewModel = new RecipeSelectViewModel(dialogService,
                                                      recipeService,
                                                      mapper,
                                                      container.Resolve<RecipeFiltrator>(),
                                                      container.Resolve<ILocalization>(),
                                                      day);

            await dialogService.ShowCustomMessageAsync<RecipeSelect, RecipeSelectViewModel>(content: viewModel);

            if (viewModel.DialogResultOk)
            {
                day.SpecificRecipe = recipeService.GetMapped<RecipeListViewDto>(viewModel.SelectedRecipe!.ID, mapper);
            }
        }

        private void DeleteRecipeManually(DayPlan day) => day.SpecificRecipe = null;

        private void Return() => navigationContext?.NavigationService.Journal.GoBack();
    }
}
