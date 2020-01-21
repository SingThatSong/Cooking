using System;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Base class for database entities.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Gets or sets identificator for entity.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets entity's culture. Used for localization.
        /// </summary>
        public string? Culture { get; set; }
    }
}
