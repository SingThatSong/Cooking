using System.Collections.ObjectModel;
using Validar;

namespace Cooking.WPF.DTO;

/// <summary>
/// Dto for ingredient group editing.
/// </summary>
[InjectValidation]
public class IngredientGroupEdit : EntityNotify
{
    /// <summary>
    /// Gets or sets ingredient group name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets ingredients that belongs to this group.
    /// </summary>
    public ObservableCollection<RecipeIngredientEdit>? Ingredients { get; set; }
}
