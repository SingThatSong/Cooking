using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Ingredient's type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Names are obvious.")]
    public enum IngredientType : int
    {
        Alcohol = 1,
        Cereals = 2,
        Seafood = 3,
        Grocery = 4,
        Spice = 5,
        Dairy = 6,
        Cheese = 7,
        Vegetables = 8,
        Fruits = 9,
        Mushrooms = 10,
        Herbs = 11,
        Meat = 12,
        Nuts = 13,
        Ready = 14,
    }
}