using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Cooking.WPF.Events;
using Cooking.WPF.Helpers;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class RecepiesViewModel : INavigationAware
    {
        public CollectionViewSource RecipiesSource { get; set; }
        public ObservableCollection<RecipeSelectDto>? Recipies { get; private set; }

        public DelegateCommand AddRecipeCommand { get; }
        public DelegateCommand<Guid> ViewRecipeCommand { get; }

        public DelegateCommand LoadedCommand { get; }

        public bool IsListView { get; set; }
        public bool IsTilesView { get; set; } = true;

        public void OnIsListViewChanged()
        {
            IsTilesView = !IsListView;
        }

        public void OnIsTilesViewChanged()
        {
            IsListView = !IsTilesView;
        }

        public RecepiesViewModel(DialogService dialogUtils, 
                                 IContainerExtension container, 
                                 IRegionManager regionManager, 
                                 RecipeService recipeService,
                                 IEventAggregator eventAggregator,
                                 IMapper mapper,
                                 RecipeFiltrator recipeFiltrator)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(container != null);
            Debug.Assert(regionManager != null);
            Debug.Assert(recipeService != null);
            Debug.Assert(eventAggregator != null);
            Debug.Assert(mapper != null);
            Debug.Assert(recipeFiltrator != null);

            this.dialogUtils = dialogUtils;
            this.container = container;
            this.regionManager = regionManager;
            this.recipeService = recipeService;
            this.mapper = mapper;
            this.recipeFiltrator = recipeFiltrator;

            // Subscribe to events
            eventAggregator.GetEvent<RecipeCreatedEvent>().Subscribe(OnRecipeCreated, ThreadOption.UIThread);
            eventAggregator.GetEvent<RecipeUpdatedEvent>().Subscribe(OnRecipeUpdated, ThreadOption.UIThread);
            eventAggregator.GetEvent<RecipeDeletedEvent>().Subscribe(OnRecipeDeleted, ThreadOption.UIThread);

            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);

            ViewRecipeCommand = new DelegateCommand<Guid>(ViewRecipe);
            AddRecipeCommand = new DelegateCommand(AddRecipe);

            RecipiesSource = new CollectionViewSource();
            RecipiesSource.Filter += RecipiesSource_Filter;
            RecipiesSource.SortDescriptions.Add(new SortDescription() { PropertyName = nameof(RecipeSelectDto.Name) });
        }

        private void OnRecipeDeleted(Guid id)
        {
            var existingRecipe = Recipies!.First(x => x.ID == id);
            Recipies!.Remove(existingRecipe);
        }

        private void OnRecipeUpdated(RecipeEdit obj)
        {
            var existingRecipe = Recipies!.First(x => x.ID == obj.ID);
            mapper.Map(obj, existingRecipe);
        }

        private void OnRecipeCreated(RecipeEdit obj)
        {
            Recipies!.Add(mapper.Map<RecipeSelectDto>(obj));
        }

        private void OnLoaded()
        {
            Debug.WriteLine("RecepiesViewModel.OnLoaded");
            var recipies = recipeService.GetProjected<RecipeSelectDto>(container.Resolve<IMapper>());
            Recipies = new ObservableCollection<RecipeSelectDto>(recipies);

            RecipiesSource.Source = Recipies;
        }

        private void RecipiesSource_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(filterText))
                return;

            if (e.Item is RecipeSelectDto recipe)
            {
                e.Accepted = recipeFiltrator.FilterObject(recipe);
            }
        }

        private string? filterText;
        private readonly DialogService dialogUtils;
        private readonly IContainerExtension container;
        private readonly IRegionManager regionManager;
        private readonly RecipeService recipeService;
        private readonly IMapper mapper;
        private readonly RecipeFiltrator recipeFiltrator;

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
                }
            }
        }

        public void ViewRecipe(Guid recipeId)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecipeViewModel.Recipe), recipeId }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(RecipeView), parameters);
        }

        public void AddRecipe()
        {
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(RecipeView));
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[nameof(FilterText)] != null)
            {
                FilterText = (string)navigationContext.Parameters[nameof(FilterText)];
            }

            var mainVM = container.Resolve<MainWindowViewModel>();
            mainVM.SelectMenuItemByViewType(typeof(Recepies));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}