using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Events;
using Cooking.WPF.Services;
using Cooking.WPF.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using WPF.Commands;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for a list of recipies.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class RecipeListViewModel : INavigationAware
    {
        private readonly DialogService dialogService;
        private readonly RecipeFiltrator recipeFiltrator;
        private readonly IContainerExtension container;
        private readonly IRegionManager regionManager;
        private readonly RecipeService recipeService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeListViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="recipeFiltrator">RecipeFiltrator dependency.</param>
        /// <param name="container">IoC container.</param>
        /// <param name="regionManager">Region manager for Prism navigation.</param>
        /// <param name="recipeService">Recipe service dependency.</param>
        /// <param name="eventAggregator">Dependency on Prism event aggregator.</param>
        /// <param name="mapper">Mapper dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        public RecipeListViewModel(DialogService dialogService,
                                   RecipeFiltrator recipeFiltrator,
                                   IContainerExtension container,
                                   IRegionManager regionManager,
                                   RecipeService recipeService,
                                   IEventAggregator eventAggregator,
                                   IMapper mapper,
                                   ILocalization localization)
        {
            this.dialogService = dialogService;
            this.recipeFiltrator = recipeFiltrator;
            this.container = container;
            this.regionManager = regionManager;
            this.recipeService = recipeService;
            this.mapper = mapper;
            this.localization = localization;

            // Subscribe to events
            eventAggregator.GetEvent<RecipeCreatedEvent>().Subscribe(OnRecipeCreated, ThreadOption.UIThread);
            eventAggregator.GetEvent<RecipeUpdatedEvent>().Subscribe(OnRecipeUpdated, ThreadOption.UIThread);
            eventAggregator.GetEvent<RecipeDeletedEvent>().Subscribe(OnRecipeDeleted, ThreadOption.UIThread);

            LoadedCommand = new AsyncDelegateCommand(OnLoaded, executeOnce: true);

            ViewRecipeCommand = new DelegateCommand<Guid>(ViewRecipe);
            AddRecipeCommand = new DelegateCommand(AddRecipe);

            RecipiesSource = new CollectionViewSource();
            RecipiesSource.SortDescriptions.Add(new SortDescription() { PropertyName = nameof(RecipeListViewDto.Name) });
        }

        /// <summary>
        /// Gets a value indicating whether no recipies is present.
        /// </summary>
        public bool RecipiesNotFound { get; private set; }

        /// <summary>
        /// Gets or sets source for filtering and ordering source collection of recipies.
        /// </summary>
        public CollectionViewSource RecipiesSource { get; set; }

        /// <summary>
        /// Gets source collection of recipies.
        /// </summary>
        public ObservableCollection<RecipeListViewDto>? Recipies { get; private set; }

        /// <summary>
        /// Gets command to add recipe.
        /// </summary>
        public DelegateCommand AddRecipeCommand { get; }

        /// <summary>
        /// Gets command to view recipe.
        /// </summary>
        public DelegateCommand<Guid> ViewRecipeCommand { get; }

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public AsyncDelegateCommand LoadedCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether current view is tiles view.
        /// </summary>
        public bool IsTilesView { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether view is filterable.
        /// </summary>
        public bool IsFilterable { get; set; } = true;

        /// <summary>
        /// Gets or sets filter text value.
        /// </summary>
        [PropertyChanged.OnChangedMethod(nameof(UpdateRecipiesSource))]
        public string? FilterText { get; set; }

        /// <inheritdoc/>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[nameof(FilterText)] != null)
            {
                FilterText = (string)navigationContext.Parameters[nameof(FilterText)];
            }

            // Tag to filter recipies
            if (navigationContext.Parameters[Consts.TagNameParameter] != null)
            {
                IsFilterable = false;
                FilterText = $"{Consts.TagSymbol}\"{navigationContext.Parameters[Consts.TagNameParameter]}\"";
            }
        }

        /// <inheritdoc/>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            object tagNameParameter = navigationContext.Parameters[Consts.TagNameParameter];

            if (tagNameParameter == null && IsFilterable)
            {
                return true;
            }
            else
            {
                return FilterText == $"{Consts.TagSymbol}\"{navigationContext.Parameters[Consts.TagNameParameter]}\"";
            }
        }

        /// <inheritdoc/>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void OnRecipeDeleted(Guid id)
        {
            RecipeListViewDto? existingRecipe = Recipies!.FirstOrDefault(x => x.ID == id);
            if (existingRecipe != null)
            {
                Recipies?.Remove(existingRecipe);
            }
        }

        private void OnRecipeUpdated(RecipeEdit obj)
        {
            RecipeListViewDto? existingRecipe = Recipies!.FirstOrDefault(x => x.ID == obj.ID);
            if (existingRecipe != null)
            {
                mapper.Map(obj, existingRecipe);
            }
        }

        private void OnRecipeCreated(RecipeEdit obj) => UpdateRecipiesSource();

        private Task OnLoaded()
        {
            List<RecipeListViewDto> recipies = recipeService.GetProjected<RecipeListViewDto>(x => !x.Tags!.Any(t => t.IsInMenu));
            Recipies = new ObservableCollection<RecipeListViewDto>(recipies);

            RecipiesSource.Source = Recipies;
            if (FilterText != null)
            {
                UpdateRecipiesSource();
            }

            return Task.CompletedTask;
        }

        private void UpdateRecipiesSource()
        {
            if (Recipies != null)
            {
                Recipies.Clear();
            }
            else
            {
                Recipies = new ObservableCollection<RecipeListViewDto>();
            }

            List<RecipeListViewDto> newEntries;
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                Expression<Func<Recipe, bool>> filterExpression = recipeFiltrator.Instance.Value.GetExpression(FilterText);
                newEntries = recipeService.GetMapped<RecipeListViewDto>(clientsidePredicate: filterExpression.Compile());
            }
            else
            {
                newEntries = recipeService.GetProjected<RecipeListViewDto>();
            }

            Recipies.AddRange(newEntries);

            if (RecipiesSource.View is ListCollectionView listCollectionView)
            {
                RecipiesNotFound = listCollectionView.IsEmpty;
            }
        }

        private void ViewRecipe(Guid recipeID)
        {
            regionManager.NavigateMain(
                view: nameof(RecipeView),
                parameters: (nameof(RecipeViewModel.Recipe), recipeID));
        }

        private void AddRecipe() => regionManager.NavigateMain(view: nameof(RecipeView));
    }
}