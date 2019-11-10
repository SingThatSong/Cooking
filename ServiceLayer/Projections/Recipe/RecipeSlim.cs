using Data.Model;

namespace Cooking.ServiceLayer.Projections
{
    public class RecipeSlim : RecipeProjection
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public CalorieType CalorieType { get; set; }
    }
}
