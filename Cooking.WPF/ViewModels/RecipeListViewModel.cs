using AutoMapper;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Events;
using Cooking.WPF.Services;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// View model for a list of recipies.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class RecipeListViewModel : INavigationAware
    {
        private readonly DialogService dialogService;
        private readonly IContainerExtension container;
        private readonly IRegionManager regionManager;
        private readonly RecipeService recipeService;
        private readonly IMapper mapper;
        private readonly RecipeFiltrator recipeFiltrator;
        private readonly ILocalization localization;

        private string? filterText;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeListViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="container">IoC container.</param>
        /// <param name="regionManager">Region manager for Prism navigation.</param>
        /// <param name="recipeService">Recipe service dependency.</param>
        /// <param name="eventAggregator">Dependency on Prism event aggregator.</param>
        /// <param name="mapper">Mapper dependency.</param>
        /// <param name="recipeFiltrator">Dependency on recipe filtrator.</param>
        /// <param name="localization">Localization provider dependency.</param>
        public RecipeListViewModel(DialogService dialogService,
                                 IContainerExtension container,
                                 IRegionManager regionManager,
                                 RecipeService recipeService,
                                 IEventAggregator eventAggregator,
                                 IMapper mapper,
                                 RecipeFiltrator recipeFiltrator,
                                 ILocalization localization)
        {
            this.dialogService = dialogService;
            this.container = container;
            this.regionManager = regionManager;
            this.recipeService = recipeService;
            this.mapper = mapper;
            this.recipeFiltrator = recipeFiltrator;
            this.localization = localization;

            // Subscribe to events
            eventAggregator.GetEvent<RecipeCreatedEvent>().Subscribe(OnRecipeCreated, ThreadOption.UIThread);
            eventAggregator.GetEvent<RecipeUpdatedEvent>().Subscribe(OnRecipeUpdated, ThreadOption.UIThread);
            eventAggregator.GetEvent<RecipeDeletedEvent>().Subscribe(OnRecipeDeleted, ThreadOption.UIThread);

            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);

            ViewRecipeCommand = new DelegateCommand<Guid>(ViewRecipe);
            AddRecipeCommand = new DelegateCommand(AddRecipe);

            RecipiesSource = new CollectionViewSource();
            RecipiesSource.Filter += RecipiesSource_Filter;
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
        /// Gets caption for search help placeholder.
        /// </summary>
        public string? SearchHelpTextCaption => localization.GetLocalizedString("SearchHelpText", Consts.IngredientSymbol, Consts.TagSymbol);

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
        public DelegateCommand LoadedCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether current view is tiles view.
        /// </summary>
        public bool IsTilesView { get; set; } = true;

        /// <summary>
        /// Gets or sets filter text value.
        /// </summary>
        public string? FilterText
        {
            get => filterText;
            set
            {
                if (filterText != value)
                {
                    filterText = value;
                    recipeFiltrator.OnFilterTextChanged(value);

                    RecipiesSource.View?.Refresh();
                    if (RecipiesSource.View is ListCollectionView listCollectionView)
                    {
                        RecipiesNotFound = listCollectionView.Count == 0;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[nameof(FilterText)] != null)
            {
                FilterText = (string)navigationContext.Parameters[nameof(FilterText)];
            }

            MainWindowViewModel mainVM = container.Resolve<MainWindowViewModel>();
            mainVM.SelectMenuItemByViewType(typeof(RecipeListView));
        }

        /// <inheritdoc/>
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        /// <inheritdoc/>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void OnRecipeDeleted(Guid id)
        {
            RecipeListViewDto existingRecipe = Recipies!.First(x => x.ID == id);
            Recipies!.Remove(existingRecipe);
        }

        private void OnRecipeUpdated(RecipeEdit obj)
        {
            RecipeListViewDto existingRecipe = Recipies!.First(x => x.ID == obj.ID);
            mapper.Map(obj, existingRecipe);
        }

        private void OnRecipeCreated(RecipeEdit obj) => Recipies!.Add(mapper.Map<RecipeListViewDto>(obj));

        private void OnLoaded()
        {
            Debug.WriteLine("RecepiesViewModel.OnLoaded");
            List<RecipeListViewDto> recipies = recipeService.GetAllMapped<RecipeListViewDto>(container.Resolve<IMapper>());
            Recipies = new ObservableCollection<RecipeListViewDto>(recipies);

            RecipiesSource.Source = Recipies;
        }

        private void RecipiesSource_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(filterText))
            {
                return;
            }

            if (e.Item is RecipeListViewDto recipe)
            {
                e.Accepted = recipeFiltrator.FilterObject(recipe);
            }
        }

        private void ViewRecipe(Guid recipeId)
        {
            regionManager.NavigateMain(
                view: nameof(RecipeView),
                parameters: (nameof(RecipeViewModel.Recipe), recipeId));
        }

        private void AddRecipe() => regionManager.NavigateMain(nameof(RecipeView));
    }
}