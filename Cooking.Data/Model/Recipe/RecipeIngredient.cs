using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Entity for ingredient in recipe.
    /// </summary>
    public class RecipeIngredient : Entity
    {
        /// <summary>
        /// Gets or sets order in resipe's ingredients list.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets foreign key for <see cref="Ingredient"/>.
        /// </summary>
        public Guid IngredientID { get; set; }

        /// <summary>
        /// Gets or sets related ingredient.
        /// </summary>
        public virtual Ingredient? Ingredient { get; set; }

        /// <summary>
        /// Gets or sets amount of ingredient in recipe.
        /// </summary>
        public double? Amount { get; set; }

        /// <summary>
        /// Gets or sets measurement unit for <see cref="Amount"/>. Ignored in mapping.
        /// </summary>
        public MeasureUnit? MeasureUnit { get; set; }

        /// <summary>
        /// Gets or sets store value for <see cref="MeasureUnit"/>.
        /// </summary>
        public Guid? MeasureUnitID { get; set; }
    }
}