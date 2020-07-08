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
        // TODO: Rename to Type when EF Core 5 will be fixed (EF Core 5.0.0-preview.2.20159.4 tested)
        public IngredientType? Type { get; set; }
    }
}
