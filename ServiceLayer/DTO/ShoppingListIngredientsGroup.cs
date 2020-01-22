using System.Collections.Generic;

namespace ServiceLayer
{
    /// <summary>
    /// Ingredients in shopping list grouped by type.
    /// </summary>
    public class ShoppingListIngredientsGroup
    {
        /// <summary>
        /// Gets name of a group.
        /// </summary>
        public string? IngredientGroupName { get; internal set; }

        /// <summary>
        /// Gets ingredients in a group.
        /// </summary>
        public List<ShoppingListIngredient> Ingredients { get; internal set; } = new List<ShoppingListIngredient>();
    }
}