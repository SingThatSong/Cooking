using System;
using System.Collections.Generic;

namespace Data.Model
{
    public class IngredientsGroup
    {
        public Guid? ID { get; set; }

        public string Name { get; set; }
        public List<RecipeIngredient> Ingredients { get; set; }
    }
}