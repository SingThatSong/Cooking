namespace ServiceLayer
{
    /// <summary>
    /// Amount of ingredient in shopping cart list. Read-only.
    /// </summary>
    public sealed class ShoppingListAmount
    {
        /// <summary>
        /// Gets or sets amount of ingredient.
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Gets or sets name of ingredient measurement unit.
        /// </summary>
        public string? MeasurementUnit { get; set; }
    }
}