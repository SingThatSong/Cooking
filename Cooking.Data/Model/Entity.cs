using System;
using System.ComponentModel.DataAnnotations.Schema;

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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets entity's culture. Used for localization.
        /// </summary>
        public string? Culture { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj switch
                                                   {
                                                       Entity entity => ID.Equals(entity.ID),
                                                       _ => false
                                                   };

        /// <inheritdoc/>
        public override int GetHashCode() => ID.GetHashCode();
    }
}
