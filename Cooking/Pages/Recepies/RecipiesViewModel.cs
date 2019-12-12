using Cooking.Commands;
using Cooking.DTO;
using Cooking.ServiceLayer.Projections;
using Data.Model;
using Plafi;
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
    public partial class RecepiesViewModel
    {
        public CollectionViewSource RecipiesSource { get; set; }
        public ObservableCollection<RecipeSelectDto>? Recipies { get; private set; }

        public DelegateCommand AddRecipeCommand { get; }
        public DelegateCommand<Guid> ViewRecipeCommand { get; }

        public DelegateCommand LoadedCommand { get; }

        public RecepiesViewModel()
        {
            FilterContext = new FilterContext<RecipeSelectDto>().AddFilter("name", HasName, isDefault:true)
                                                             .AddFilter(Consts.IngredientSymbol, HasIngredient)
                                                             .AddFilter(Consts.TagSymbol, HasTag);

            dialogUtils = new DialogUtils(this);
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

            Recipies = RecipeService.GetRecipies()
                                    .MapTo<ObservableCollection<RecipeSelectDto>>();

            RecipiesSource.Source = Recipies;
        }

        private void RecipiesSource_Filter(object sender, FilterEventArgs e)
        {
            if (FilterContext == null || !expressionBuilt)
                return;

            if (e.Item is RecipeSelectDto recipe)
            {
                e.Accepted = FilterContext.Filter(recipe);
            }
        }

        private string? filterText;
        private readonly DialogUtils dialogUtils;
        private bool expressionBuilt = false;
        private FilterContext<RecipeSelectDto> FilterContext { get; set; }
        public string? FilterText
        {
            get => filterText;
            set
            {
                if (filterText != value)
                {
                    filterText = value;
                    try
                    {
                        FilterContext.BuildExpression(value);
                        expressionBuilt = true;
                    }
                    catch 
                    {
                        expressionBuilt = false;
                    }

                    RecipiesSource.View?.Refresh();
                }
            }
        }
        private bool HasName(RecipeSelectDto recipe, string name)
        {
            return recipe.Name.ToUpperInvariant().Contains(name.ToUpperInvariant(), StringComparison.Ordinal);
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
                recipeDb = RecipeService.GetProjection<RecipeFull>(recipe.ID);
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
                recipeDb = RecipeService.GetProjection<RecipeFull>(recipe.ID);
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
            await dialogUtils.ShowCustomMessageAsync<RecipeView, RecipeViewModel>(content: new RecipeViewModel(recipeId)).ConfigureAwait(false);
        }

        public async void AddRecipe()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<RecipeView, RecipeViewModel>("Новый рецепт", content: new RecipeViewModel() { IsEditing = true }).ConfigureAwait(false); ;

            if (viewModel.DialogResultOk)
            {
                var id = await RecipeService.CreateAsync(viewModel.Recipe.MapTo<Recipe>()).ConfigureAwait(false);
                viewModel.Recipe.ID = id;
                Recipies!.Add(viewModel.Recipe.MapTo<RecipeSelectDto>());
            }
        }
    }
}