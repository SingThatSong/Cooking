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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets ingredient's Type.
        /// </summary>
        public IngredientType? Type { get; set; }
    }
}
