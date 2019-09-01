using Data.Model;
using System.Collections.Generic;

namespace Cooking.ServiceLayer
{
    public class IngredientGroupData : Entity
    {
        public string Name { get; set; }
        public IEnumerable<RecipeIngredientData> Ingredients { get; set; }
    }
}
