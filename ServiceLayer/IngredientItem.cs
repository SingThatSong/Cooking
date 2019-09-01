using System.Collections.Generic;

namespace ServiceLayer
{
    public class IngredientItem
    {
        public string Name { get; set; }

        public List<string> RecipiesSources { get; set; } = new List<string>();

        public List<IngredientAmount> IngredientAmounts { get; set; } = new List<IngredientAmount>();
    }
}