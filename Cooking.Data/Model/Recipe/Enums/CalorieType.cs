namespace Cooking.Data.Model;

/// <summary>
/// Type of calories for recipe.
/// </summary>
public enum CalorieType
{
    /// <summary>
    /// CalorieType not selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// Low carbs recipe.
    /// </summary>
    Fitness = 1,

    /// <summary>
    /// Protein rich recipe.
    /// </summary>
    Protein = 2,

    /// <summary>
    /// Hight carbs recipe.
    /// </summary>
    Bad = 3,

    /// <summary>
    /// Sweets.
    /// </summary>
    Sweets = 4
}
