using System;
using System.Collections.Generic;

namespace TEST
{
    public class Recipe : Entity
    {
        public string Name { get; set; }
        public string SourceUrl { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }

        public int Difficulty { get; set; }
        public int Rating { get; set; }

        public int PortionsCount { get; set; }
        public virtual IEnumerable<RecipeIngredient> Ingredients { get; set; }
        public virtual IEnumerable<RecipeTag> Tags { get; set; }
    }
}
