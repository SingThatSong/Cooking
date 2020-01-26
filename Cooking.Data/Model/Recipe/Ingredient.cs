using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Ingredient database entity.
    /// </summary>
    public class Ingredient : Entity
    {
        /// <summary>
        /// Gets or sets ingredient's name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets ingredient's Type. Typesafe enum, stored in database as int. Ignored in mapping.
        /// </summary>
        public IngredientType? Type { get; set; }

        /// <summary>
        /// Gets or sets value to store Typesafe enum.
        /// </summary>
        public int? TypeID
        {
            get => Type?.ID;
            set => Type = IngredientType.AllValues.SingleOrDefault(x => x.ID == value);
        }
    }
}
