using Cooking.Data.Model;
using Plafi.DynamicLinq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Cooking.WPF.Services
{
    // Class contains expressions for database querying, null-checks is not applicable.
    #pragma warning disable CS8602, CS8604
    // Same for ToLower and Equals methods
    #pragma warning disable CA1304, CA1307, RCS1155

    /// <summary>
    /// Filtration logic for list of recipies.
    /// </summary>
    public static class RecipeFiltrator
    {
        /// <summary>
        /// Gets instance of recipe filtrator generator.
        /// </summary>
        public static Lazy<FilterContext<Recipe>> Instance { get; } = new Lazy<FilterContext<Recipe>>(() => new FilterContext<Recipe>().AddFilter(Consts.NameSymbol, CombinedFilter(), isDefault: true)
                                                                                                                                       .AddFilter(Consts.IngredientSymbol, HasIngredient())
                                                                                                                                       .AddFilter(Consts.TagSymbol.ToString(), HasTag()));

        private static Expression<Func<Recipe, string, bool>> CombinedFilter()
        {
            return (x, str) => x.Name.ToLower().Contains(str.ToLower())
                            || (x.Ingredients != null && x.Ingredients.Any(x => x.Ingredient.Name.ToLower().Contains(str.ToLower())))
                            || (x.IngredientGroups != null && x.IngredientGroups.Any(x => x.Ingredients.Any(x => x.Ingredient.Name.ToLower().Contains(str.ToLower()))));
        }

        private static Expression<Func<Recipe, string, bool>> HasTag()
        {
            return (x, str) => x.Tags != null && x.Tags.Any(x => x.Name.ToLower().Contains(str.ToLower()));
        }

        private static Expression<Func<Recipe, string, bool>> HasIngredient()
        {
            return (x, str) => (x.Ingredients != null && x.Ingredients.Any(x => x.Ingredient.Name.ToLower().Contains(str.ToLower())))
                            || (x.IngredientGroups != null && x.IngredientGroups.Any(x => x.Ingredients.Any(x => x.Ingredient.Name.ToLower().Contains(str.ToLower()))));
        }
    }
    #pragma warning restore CS8602, CS8604, CA1304, CA1307, RCS1155
}
