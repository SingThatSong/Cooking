﻿using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Events;
using Plafi;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cooking.WPF.Services
{
    /// <summary>
    /// Filtration logic for list of recipies.
    /// </summary>
    public class RecipeFiltrator
    {
        private readonly RecipeService recipeService;
        private readonly IMapper mapper;

        private Dictionary<Guid, Recipe>? recipeCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeFiltrator"/> class.
        /// </summary>
        /// <param name="recipeService">Recipe service dependency.</param>
        /// <param name="eventAggregator">Event aggregator dependency for reacting to events.</param>
        /// <param name="mapper">Mapper dependency.</param>
        public RecipeFiltrator(RecipeService recipeService,
                               IEventAggregator eventAggregator,
                               IMapper mapper)
        {
            this.recipeService = recipeService;
            this.mapper = mapper;

            FilterContext = new FilterContext<RecipeListViewDto>().AddFilter(Consts.NameSymbol, CombinedFilter, isDefault: true)
                                                                  .AddFilter(Consts.IngredientSymbol, HasIngredient)
                                                                  .AddFilter(Consts.TagSymbol, HasTag);

            eventAggregator.GetEvent<RecipeCreatedEvent>().Subscribe(OnRecipeCreated, ThreadOption.UIThread);
            eventAggregator.GetEvent<RecipeUpdatedEvent>().Subscribe(OnRecipeUpdated, ThreadOption.UIThread);
            eventAggregator.GetEvent<RecipeDeletedEvent>().Subscribe(OnRecipeDeleted, ThreadOption.UIThread);
        }

        private FilterContext<RecipeListViewDto> FilterContext { get; }

        /// <summary>
        /// Filter single object.
        /// </summary>
        /// <param name="recipe">Recipe to filter.</param>
        /// <returns>Whether recipe fits fitler.</returns>
        public bool FilterObject(RecipeListViewDto recipe) => FilterContext.IsExpressionBuilt && FilterContext.Filter(recipe);

        /// <summary>
        /// Callback called when filter text is changed.
        /// </summary>
        /// <param name="newText">New value for Filter Text.</param>
        public void OnFilterTextChanged(string? newText)
        {
            if (!string.IsNullOrEmpty(newText))
            {
                FilterContext.BuildExpression(newText);
                if (recipeCache == null)
                {
                    recipeCache = recipeService.GetAll().ToDictionary(x => x.ID, x => x);
                }
            }
        }

        /// <summary>
        /// Callback for event when recipe is deleted. Remove it from cache.
        /// </summary>
        /// <param name="id">ID of deleted recipe.</param>
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

        /// <summary>
        /// Callback for event when recipe is updated. Update it in cache.
        /// </summary>
        /// <param name="obj">Updated recipe.</param>
        private void OnRecipeUpdated(RecipeEdit obj)
        {
            if (recipeCache != null)
            {
                recipeCache[obj.ID] = recipeService.Get(obj.ID);
            }
        }

        /// <summary>
        /// Callback for event when recipe is deleted. Remove it from cache.
        /// </summary>
        /// <param name="obj">Created recipe.</param>
        private void OnRecipeCreated(RecipeEdit obj)
        {
            if (recipeCache == null)
            {
                return;
            }

            if (!recipeCache.ContainsKey(obj.ID))
            {
                recipeCache.Add(obj.ID, mapper.Map<Recipe>(obj));
            }
        }

        private bool CombinedFilter(RecipeListViewDto recipe, string text) => HasName(recipe, text) || HasTag(recipe, text) || HasIngredient(recipe, text);
        private bool HasName(RecipeListViewDto recipe, string name) => recipe.Name?.ToUpperInvariant().Contains(name.ToUpperInvariant(), StringComparison.Ordinal) == true;

        private bool HasTag(RecipeListViewDto recipe, string category)
        {
            Recipe recipeDb;
            if (recipeCache!.ContainsKey(recipe.ID))
            {
                recipeDb = recipeCache[recipe.ID];
            }
            else
            {
                recipeDb = recipeService.Get(recipe.ID);
                recipeCache.Add(recipe.ID, recipeDb);
            }

            return recipeDb.Tags?.Any(x => x.Tag?.Name != null && string.Equals(x.Tag!.Name!, category, StringComparison.InvariantCultureIgnoreCase)) == true;
        }

        private bool HasIngredient(RecipeListViewDto recipe, string category)
        {
            Recipe recipeDb = recipeCache![recipe.ID];

            // Ищем среди ингредиентов
            if (recipeDb.Ingredients?.Any(x => x.Ingredient?.Name != null && string.Equals(x.Ingredient!.Name!, category, StringComparison.InvariantCultureIgnoreCase)) == true)
            {
                return true;
            }

            // Ищем среди групп ингредиентов
            if (recipeDb.IngredientGroups != null)
            {
                foreach (IngredientsGroup group in recipeDb.IngredientGroups)
                {
                    if (group.Ingredients.Any(x => x.Ingredient?.Name != null && string.Equals(x.Ingredient!.Name!, category, StringComparison.InvariantCultureIgnoreCase))
)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
