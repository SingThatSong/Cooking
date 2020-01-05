using Data.Model;

namespace Cooking.ServiceLayer.Projections
{
    public class RecipeSlim : Entity
    {
        public string? Name { get; set; }
        public string? ImagePath { get; set; }
        public CalorieType CalorieType { get; set; }
    }
}
