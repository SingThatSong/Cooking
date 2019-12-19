using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.Projections;
using Plafi;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
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

        public RecepiesViewModel(DialogService dialogUtils, 
                                 IContainerExtension container, 
                                 IRegionManager regionManager, 
                                 RecipeService recipeService)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(container != null);
            Debug.Assert(regionManager != null);
            Debug.Assert(recipeService != null);

            FilterContext = new FilterContext<RecipeSelectDto>().AddFilter("name", HasName, isDefault: true)
                                                             .AddFilter(Consts.IngredientSymbol, HasIngredient)
                                                             .AddFilter(Consts.TagSymbol, HasTag);

            this.dialogUtils = dialogUtils;
            this.container = container;
            this.regionManager = regionManager;
            this.recipeService = recipeService;

            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);


            ViewRecipeCommand = new DelegateCommand<Guid>(ViewRecipe);
            AddRecipeCommand = new DelegateCommand(AddRecipe);

            RecipiesSource = new CollectionViewSource();
            RecipiesSource.Filter += RecipiesSource_Filter;
            RecipiesSource.IsLiveSortingRequested = true;
            RecipiesSource.SortDescriptions.Add(new SortDescription() { PropertyName = nameof(RecipeSelectDto.Name) });
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
            if (FilterContext == null || !FilterContext.IsExpressionBuilt)
                return;

            if (e.Item is RecipeSelectDto recipe)
            {
                e.Accepted = FilterContext.Filter(recipe);
            }
        }

        private string? filterText;
        private readonly DialogService dialogUtils;
        private readonly IContainerExtension container;
        private readonly IRegionManager regionManager;
        private readonly RecipeService recipeService;

        private FilterContext<RecipeSelectDto> FilterContext { get; set; }
        public string? FilterText
        {
            get => filterText;
            set
            {
                if (filterText != value)
                {
                    filterText = value;
                    FilterContext.BuildExpression(value);
                    RecipiesSource.View?.Refresh();
                }
            }
        }
        private bool HasName(RecipeSelectDto recipe, string name)
        {
            return recipe.Name != null && recipe.Name.ToUpperInvariant().Contains(name.ToUpperInvariant(), StringComparison.Ordinal);
        }


        private readonly Dictionary<Guid, RecipeFull> recipeCache = new Dictionary<Guid, RecipeFull>();
        private bool HasTag(RecipeSelectDto recipe, string category)
        {
            RecipeFull recipeDb;

            if (recipeCache.ContainsKey(recipe.ID))
            {
                recipeDb = recipeCache[recipe.ID];
            }
            else
            {
                recipeDb = recipeService.GetProjected<RecipeFull>(recipe.ID);
                recipeCache.Add(recipe.ID, recipeDb);
            }

            return recipeDb.Tags != null && recipeDb.Tags.Any(x => x.Name.ToUpperInvariant() == category.ToUpperInvariant());
        }

        private bool HasIngredient(RecipeSelectDto recipe, string category)
        {
            RecipeFull recipeDb;

            if (recipeCache.ContainsKey(recipe.ID))
            {
                recipeDb = recipeCache[recipe.ID];
            }
            else
            {
                recipeDb = recipeService.GetProjected<RecipeFull>(recipe.ID);
                recipeCache.Add(recipe.ID, recipeDb);
            }

            // Ищем среди ингредиентов
            if (recipeDb.Ingredients != null
                && recipeDb.Ingredients.Any(x => x.Ingredient.Name.ToUpperInvariant() == category.ToUpperInvariant()))
            {
                return true;
            }

            // Ищем среди групп ингредиентов
            if (recipeDb.IngredientGroups != null)
            {
                foreach (var group in recipeDb.IngredientGroups)
                {
                    if (group.Ingredients.Any(x => x.Ingredient.Name.ToUpperInvariant() == category.ToUpperInvariant()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async void ViewRecipe(Guid recipeId)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecipeViewModel.Recipe), recipeId }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(RecipeView), parameters);
        }

        public async void AddRecipe()
        {
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(RecipeView), Callback);

            //var viewModel = await dialogUtils.ShowCustomMessageAsync<RecipeView, RecipeViewModel>("Новый рецепт", content: new RecipeViewModel(dialogUtils) { IsEditing = true }).ConfigureAwait(false); ;

            //if (viewModel.DialogResultOk)
            //{
            //    var id = await RecipeService.CreateAsync(viewModel.Recipe.MapTo<Recipe>()).ConfigureAwait(false);
            //    viewModel.Recipe.ID = id;
            //    Recipies!.Add(viewModel.Recipe.MapTo<RecipeSelectDto>());
            //}
        }

        private void Callback(NavigationResult navigationResult)
        {

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