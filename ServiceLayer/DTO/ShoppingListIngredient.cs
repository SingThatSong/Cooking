using System.Collections.Generic;

namespace ServiceLayer
{
    /// <summary>
    /// Ingredient in shopping list. Read-only.
    /// </summary>
    public sealed class ShoppingListIngredient
    {
        /// <summary>
        /// Gets or sets ingredient name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets recipies where this item is present.
        /// </summary>
        public List<string> RecipiesSources { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets amounts of this ingredient.
        /// </summary>
        public List<ShoppingListAmount> IngredientAmounts { get; set; } = new List<ShoppingListAmount>();
    }
}