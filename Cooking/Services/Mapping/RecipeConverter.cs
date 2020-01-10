using AutoMapper;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Data.Model;

namespace Cooking.Services
{
    public class RecipeConverter : IMappingAction<Recipe, RecipeEdit>
    {
        private readonly RecipeService recipeService;

        public RecipeConverter(RecipeService recipeService)
        {
            this.recipeService = recipeService;
        }

        public void Process(Recipe source, RecipeEdit destination, ResolutionContext context)
        {
            destination.LastCooked = recipeService.DaysFromLasCook(destination.ID);
        }
    }
}
