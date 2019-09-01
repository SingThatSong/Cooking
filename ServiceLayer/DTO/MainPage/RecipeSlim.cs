using Data.Model;
using System.Collections.Generic;

namespace Cooking.ServiceLayer
{
    public class RecipeSlim : Entity
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public CalorieType CalorieType { get; set; }
    }
}
