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
    }
}
