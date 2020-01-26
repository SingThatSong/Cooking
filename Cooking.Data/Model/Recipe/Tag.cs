using System.Collections.Generic;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Database entity for tag.
    /// </summary>
    public class Tag : Entity
    {
        /// <summary>
        /// Gets or sets tag name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets tag type.
        /// </summary>
        public TagType Type { get; set; }

        /// <summary>
        /// Gets or sets tag color.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets recipies with this tag.
        /// </summary>
        public virtual List<RecipeTag>? Recipies { get; set; }
    }
}
