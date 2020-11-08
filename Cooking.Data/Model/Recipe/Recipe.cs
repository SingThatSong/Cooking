using Cooking.Data.Model.Plan;
using System;
using System.Collections.Generic;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Recipe database entity.
    /// </summary>
    public class Recipe : Entity
    {
        /// <summary>
        /// Gets or sets recipe name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets source of recipe. E.g. some website or granny's hidden cookbook.
        /// </summary>
        public Uri? SourceUrl { get; set; }

        /// <summary>
        /// Gets or sets path to recipie's image.
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets or sets recipe description. Recipe per se. RTF format.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets recipe difficulty.
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// Gets or sets recipe rating.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets portions count for one cook.
        /// </summary>
        public int PortionsCount { get; set; }

        /// <summary>
        /// Gets or sets calorie type for recipe.
        /// </summary>
        public CalorieType CalorieType { get; set; }

        /// <summary>
        /// Gets or sets ingredient groups in recipe.
        /// </summary>
        public virtual List<IngredientsGroup> IngredientGroups { get; set; } = new List<IngredientsGroup>();

        /// <summary>
        /// Gets or sets ingredients in recipe itself.
        /// </summary>
        public virtual List<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();

        /// <summary>
        /// Gets or sets recipe tags.
        /// </summary>
        public virtual List<Tag> Tags { get; set; } = new List<Tag>();

        /// <summary>
        /// Gets or sets garnishes suitable for this recipe.
        /// </summary>
        public virtual List<Recipe>? Garnishes { get; set; }
    }
}
