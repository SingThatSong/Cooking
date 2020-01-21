using System;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Database representation for many-many relationship between tag and recipe.
    /// </summary>
    public class RecipeTag
    {
        /// <summary>
        /// Gets or sets recipe entity.
        /// </summary>
        public virtual Recipe? Recipe { get; set; }

        /// <summary>
        /// Gets or sets tag entity.
        /// </summary>
        public virtual Tag? Tag { get; set; }

        /// <summary>
        /// Gets or sets foreign key for <see cref="Recipe"/>.
        /// </summary>
        public Guid RecipeId { get; set; }

        /// <summary>
        /// Gets or sets foreign key for <see cref="Tag"/>.
        /// </summary>
        public Guid TagId { get; set; }
    }
}
