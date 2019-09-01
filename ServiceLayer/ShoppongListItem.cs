using System.Collections.Generic;

namespace ServiceLayer
{
    public class ShoppongListItem
    {
        public string IngredientGroupName { get; internal set; }

        public List<IngredientItem> Ingredients { get; internal set; } = new List<IngredientItem>();
    }
}