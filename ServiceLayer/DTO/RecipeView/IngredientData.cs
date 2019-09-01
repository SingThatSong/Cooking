using Data.Model;

namespace Cooking.ServiceLayer
{
    public class IngredientData : Entity
    {
        public string Name { get; set; }
        public IngredientType Type { get; set; }
    }
}
