using Cooking.Data.Model;
using System.Collections.Generic;

namespace Cooking.ServiceLayer
{
    public sealed class IngredientGroupData : Entity
    {
        public string? Name { get; set; }
        public IEnumerable<RecipeIngredientData>? Ingredients { get; set; }
    }
}
