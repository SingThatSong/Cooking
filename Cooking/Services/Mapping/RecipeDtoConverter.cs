﻿using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;

namespace Cooking.WPF.Services
{
    public class RecipeDtoConverter : IMappingAction<Recipe, RecipeSelectDto>
    {
        private readonly RecipeService recipeService;

        public RecipeDtoConverter(RecipeService recipeService)
        {
            this.recipeService = recipeService;
        }

        public void Process(Recipe source, RecipeSelectDto destination, ResolutionContext context)
        {
            int daysFromLastCook = recipeService.DaysFromLasCook(destination.ID);
            destination.LastCooked = daysFromLastCook;
        }
    }
}
