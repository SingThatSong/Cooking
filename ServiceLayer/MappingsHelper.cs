using AutoMapper;
using Data.Context;
using Data.Model;
namespace ServiceLayer
{
    internal class MappingsHelper
    {
        public static Recipe MapToRecipe(Recipe recipe, CookingContext context)
        {
            if (recipe.IngredientGroups != null)
            {
                for (int i = 0; i < recipe.IngredientGroups.Count; i++)
                {
                    if (recipe.IngredientGroups[i].Ingredients != null)
                    {
                        for (int j = 0; j < recipe.IngredientGroups[i].Ingredients.Count; j++)
                        {
                            var dbValue = context.RecipeIngredients.Find(recipe.IngredientGroups[i].Ingredients[j].ID);
                            if (dbValue != null)
                            {
                                recipe.IngredientGroups[i].Ingredients[j] = dbValue;
                            }
                        }
                    }
                }
            }

            if (recipe.Ingredients != null)
            {
                for (int i = 0; i < recipe.Ingredients.Count; i++)
                {
                    var dbValue = context.RecipeIngredients.Find(recipe.Ingredients[i].ID);
                    if (dbValue != null)
                    {
                        recipe.Ingredients[i] = dbValue;
                    }
                }
            }

            if (recipe.Tags != null)
            {
                for (int i = 0; i < recipe.Tags.Count; i++)
                {
                    var dbValue = context.Tags.Find(recipe.Tags[i].Tag.ID);
                    if (dbValue != null)
                    {
                        recipe.Tags[i].Tag = dbValue;
                    }
                }
            }

            return recipe;
        }
    }
}
