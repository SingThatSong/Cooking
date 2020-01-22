﻿using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;

namespace Cooking.WPF.Services
{
    public class RecipeDtoConverter : IMappingAction<Recipe, RecipeListViewDto>
    {
        private readonly RecipeService recipeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeDtoConverter"/> class.
        /// </summary>
        /// <param name="recipeService"></param>
        public RecipeDtoConverter(RecipeService recipeService)
        {
            this.recipeService = recipeService;
        }

        public void Process(Recipe source, RecipeListViewDto destination, ResolutionContext context)
        {
            int daysFromLastCook = recipeService.DaysFromLasCook(destination.ID);
            destination.LastCooked = daysFromLastCook;
        }
    }
}
