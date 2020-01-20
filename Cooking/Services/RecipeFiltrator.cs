using AutoMapper;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.Projections;
using Cooking.WPF.DTO;
using Cooking.WPF.Events;
using Plafi;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cooking.WPF.Helpers
{
    public class RecipeFiltrator
    {
        private readonly RecipeService recipeService;
        private readonly IMapper mapper;

        private Dictionary<Guid, RecipeFull>? recipeCache;
        private FilterContext<RecipeListViewDto> FilterContext { get; set; }

        public RecipeFiltrator(RecipeService recipeService,
                               IEventAggregator eventAggregator,
                               IMapper mapper)
        {
            this.recipeService = recipeService;
            this.mapper = mapper;

            FilterContext = new FilterContext<RecipeListViewDto>().AddFilter("name", CombinedFilter, isDefault: true)
                                                                .AddFilter(Consts.IngredientSymbol, HasIngredient)
                                                                .AddFilter(Consts.TagSymbol, HasTag);

            eventAggregator.GetEvent<RecipeCreatedEvent>().Subscribe(OnRecipeCreated, ThreadOption.UIThread);
            eventAggregator.GetEvent<RecipeUpdatedEvent>().Subscribe(OnRecipeUpdated, ThreadOption.UIThread);
            eventAggregator.GetEvent<RecipeDeletedEvent>().Subscribe(OnRecipeDeleted, ThreadOption.UIThread);
        }

        private void OnRecipeDeleted(Guid id)
        {
            if (recipeCache == null)
            {
                return;
            }

            if (recipeCache.ContainsKey(id))
            {
                recipeCache.Remove(id);
            }
        }

        private void OnRecipeUpdated(RecipeEdit obj)
        {
            if (recipeCache != null)
            {
                recipeCache[obj.ID] = recipeService.GetProjected<RecipeFull>(obj.ID);
            }
        }

        private void OnRecipeCreated(RecipeEdit obj)
        {
            if (recipeCache == null)
            {
                return;
            }

            recipeCache!.Add(obj.ID, mapper.Map<RecipeFull>(obj));
        }

        public bool FilterObject(RecipeListViewDto recipe) => FilterContext.IsExpressionBuilt ? FilterContext.Filter(recipe) : false;

        public void OnFilterTextChanged(string? newText)
        {
            if (!string.IsNullOrEmpty(newText))
            {
                FilterContext.BuildExpression(newText);
                if (recipeCache == null)
                {
                    recipeCache = recipeService.GetProjected<RecipeFull>().ToDictionary(x => x.ID, x => x);
                }
            }
        }

        private bool CombinedFilter(RecipeListViewDto recipe, string text) => HasName(recipe, text) || HasTag(recipe, text) || HasIngredient(recipe, text);
        private bool HasName(RecipeListViewDto recipe, string name) => recipe.Name != null && recipe.Name.ToUpperInvariant().Contains(name.ToUpperInvariant(), StringComparison.Ordinal);
        private bool HasTag(RecipeListViewDto recipe, string category)
        {
            RecipeFull recipeDb = recipeCache![recipe.ID];
            return recipeDb.Tags != null && recipeDb.Tags
                                                    .Where(x => x.Name != null)
                                                    .Any(x => x.Name!.ToUpperInvariant() == category.ToUpperInvariant());
        }

        private bool HasIngredient(RecipeListViewDto recipe, string category)
        {
            RecipeFull recipeDb = recipeCache![recipe.ID];

            // Ищем среди ингредиентов
            if (recipeDb.Ingredients != null
                && recipeDb.Ingredients.Where(x => x.Ingredient?.Name != null)
                                       .Any(x => x.Ingredient!.Name!.ToUpperInvariant() == category.ToUpperInvariant()))
            {
                return true;
            }

            // Ищем среди групп ингредиентов
            if (recipeDb.IngredientGroups != null)
            {
                foreach (IngredientGroupData group in recipeDb.IngredientGroups)
                {
                    if (group.Ingredients.Where(x => x.Ingredient?.Name != null)
                                         .Any(x => x.Ingredient!.Name!.ToUpperInvariant() == category.ToUpperInvariant()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
