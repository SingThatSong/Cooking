using System.Collections.Generic;

namespace Cooking.ServiceLayer
{
    /// <summary>
    /// Ingredients in shopping list grouped by type.
    /// </summary>
    public class ShoppingListIngredientsGroup
    {
        /// <summary>
        /// Gets or sets name of a group.
        /// </summary>
        public string? IngredientGroupName { get; set; }

        /// <summary>
        /// Gets ingredients in a group.
        /// </summary>
        public List<ShoppingListIngredient> Ingredients { get; internal set; } = new List<ShoppingListIngredient>();
    }
}