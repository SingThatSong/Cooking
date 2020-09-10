using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;

namespace Cooking.WPF.Services
{
    /// <summary>
    /// Converter from db Recipe entity and RecipeEdit dto, setting LastCooked using RecipeService.
    /// </summary>
    public class RecipeConverter : IMappingAction<Recipe, RecipeEdit>
    {
        private readonly RecipeService recipeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeConverter"/> class.
        /// </summary>
        /// <param name="recipeService">Dependency to <see cref="RecipeService"/>.</param>
        public RecipeConverter(RecipeService recipeService)
        {
            this.recipeService = recipeService;
        }

        /// <inheritdoc/>
        public void Process(Recipe source, RecipeEdit destination, ResolutionContext context)
        {
            destination.LastCooked = recipeService.DaysFromLasCook(destination.ID);
        }
    }
}
