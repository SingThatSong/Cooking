using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;

namespace Cooking.WPF.Services;

/// <summary>
/// Converter from db Recipe entity and RecipeListViewDto dto, setting LastCooked using RecipeService.
/// </summary>
public class RecipeDtoConverter : IMappingAction<Recipe, RecipeListViewDto>
{
    private readonly RecipeService recipeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecipeDtoConverter"/> class.
    /// </summary>
    /// <param name="recipeService">Dependency to <see cref="RecipeService"/>.</param>
    public RecipeDtoConverter(RecipeService recipeService)
    {
        this.recipeService = recipeService;
    }

    /// <inheritdoc/>
    public void Process(Recipe source, RecipeListViewDto destination, ResolutionContext context)
    {
        destination.LastCooked = recipeService.DaysFromLasCook(destination.ID);
    }
}
