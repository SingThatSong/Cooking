using Cooking.WPF.DTO;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// View model for creating/editing ingredient group.
/// </summary>
public class IngredientGroupEditViewModel : OkCancelViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IngredientGroupEditViewModel"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service dependency.</param>
    /// <param name="ingredientGroup">Ingredient group for editin. Null means group creation.</param>
    public IngredientGroupEditViewModel(DialogService dialogService, IngredientGroupEdit? ingredientGroup = null)
        : base(dialogService)
    {
        IngredientGroup = ingredientGroup ?? new IngredientGroupEdit();
    }

    /// <summary>
    /// Gets ingredient group to edit.
    /// </summary>
    public IngredientGroupEdit IngredientGroup { get; }
}
