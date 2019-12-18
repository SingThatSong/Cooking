using System.Collections.Generic;

namespace Data.Model
{
    public class IngredientsGroup : Entity
    {
        public string Name { get; set; }
        public virtual List<RecipeIngredient> Ingredients { get; set; }
    }
}