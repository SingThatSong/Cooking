using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;

namespace Cooking.WPF.Services
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
