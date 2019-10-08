using System;
using System.Collections.Generic;

namespace TEST
{
    public class IngredientsGroup : Entity
    {
        public string Name { get; set; }
        public virtual List<RecipeIngredient> Ingredients { get; set; }
    }
}