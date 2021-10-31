using SmartFormat;

namespace Cooking.ServiceLayer;

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
    /// Gets or sets name of ingredient measurement unit plural forms.
    /// </summary>
    public string? MeasurementUnitPluralization { get; set; }

    /// <summary>
    /// Gets name of ingredient measurement unit.
    /// </summary>
    public string? MeasurementUnit => MeasurementUnitPluralization != null ? Smart.Format($"{{0:{MeasurementUnitPluralization}}}", Amount) : null;
}
