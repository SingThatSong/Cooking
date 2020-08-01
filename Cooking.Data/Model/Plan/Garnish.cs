using System.Collections.Generic;

namespace Cooking.Data.Model.Plan
{
    /// <summary>
    /// Garnish database entity.
    /// </summary>
    public class Garnish : Entity
    {
        /// <summary>
        /// Gets or sets name for garnish.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets recipies with this garnish.
        /// </summary>
        public virtual List<Recipe>? Recipies { get; set; }
    }
}
