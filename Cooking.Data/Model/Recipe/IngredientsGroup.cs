using System.Collections.Generic;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Database entity for group of ingredients. E.g. souce in recipe.
    /// </summary>
    public class IngredientsGroup : Entity
    {
        /// <summary>
        /// Gets or sets ingredient group name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets ingredients in group.
        /// </summary>
        public virtual List<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
    }
}