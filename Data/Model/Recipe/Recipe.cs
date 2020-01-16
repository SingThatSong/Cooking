using System;
using System.Collections.Generic;

namespace Cooking.Data.Model
{
    public class Recipe : Entity
    {
        public string? Name { get; set; }
        public Uri? SourceUrl { get; set; }
        public string? ImagePath { get; set; }
        public string? Description { get; set; }

        public int Difficulty { get; set; }
        public int Rating { get; set; }

        public int PortionsCount { get; set; }
        public CalorieType CalorieType { get; set; }
#pragma warning disable CA2227 // Свойства коллекций должны быть доступны только для чтения
        public virtual List<IngredientsGroup>? IngredientGroups { get; set; }
        public virtual List<RecipeIngredient>? Ingredients { get; set; }
        public virtual List<RecipeTag>? Tags { get; set; }
#pragma warning restore CA2227
    }
}
