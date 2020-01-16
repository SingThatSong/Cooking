using AutoMapper;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Cooking.Data.Model;

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
            int daysFromLastCook = recipeService.DaysFromLasCook(destination.ID);
            destination.LastCooked = daysFromLastCook;
        }
    }
}
