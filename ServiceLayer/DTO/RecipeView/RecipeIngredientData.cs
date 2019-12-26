using Data.Model;

namespace Cooking.ServiceLayer
{
    public sealed class RecipeIngredientData : Entity
    {
        public IngredientData? Ingredient { get; set; }
        public double? Amount { get; set; }
        public MeasureUnit? MeasureUnit { get; set; }
        public int Order { get; set; }
    }
}