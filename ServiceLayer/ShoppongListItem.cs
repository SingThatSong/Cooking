using System.Collections.Generic;

namespace ServiceLayer
{
    public class ShoppingListItem
    {
        public string IngredientGroupName { get; internal set; }

        public List<IngredientItem> Ingredients { get; internal set; } = new List<IngredientItem>();
    }
}