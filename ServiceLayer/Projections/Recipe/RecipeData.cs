using System;
using System.Collections.Generic;

namespace Cooking.ServiceLayer.Projections
{
    public class RecipeFull : RecipeSlim
    {
        public Uri SourceUrl { get; set; }
        public string Description { get; set; }
        public int PortionsCount { get; set; }
        public int? Difficulty { get; set; }
        public int? Rating { get; set; }

        public IEnumerable<IngredientGroupData> IngredientGroups { get; set; }
        public IEnumerable<RecipeIngredientData> Ingredients { get; set; }
        public IEnumerable<TagData> Tags { get; set; }
    }
}
