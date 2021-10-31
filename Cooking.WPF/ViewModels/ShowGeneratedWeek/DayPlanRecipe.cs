using Cooking.Data.Model;
using Cooking.WPF.DTO;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// Dto for a recipe in a day in week generation.
/// </summary>
public class DayPlanRecipe : EntityNotify
{
    /// <summary>
    /// Gets or sets recipe name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets calorie type.
    /// </summary>
    public CalorieType CalorieType { get; set; }

    /// <summary>
    /// Gets or sets garnishes for a recipe.
    /// </summary>
    public List<DayPlanRecipe> Garnishes { get; set; } = new List<DayPlanRecipe>();
}
