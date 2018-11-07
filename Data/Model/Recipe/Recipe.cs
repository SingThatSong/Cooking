﻿using System;
using System.Collections.Generic;

namespace Data.Model
{
    public class Recipe
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public int PortionsCount { get; set; }
        public CalorieType CalorieType { get; set; }
        public List<IngredientsGroup> IngredientGroups { get; set; }
        public List<RecipeIngredient> Ingredients { get; set; }
        public List<RecipeTag> Tags { get; set; }
    }
}
