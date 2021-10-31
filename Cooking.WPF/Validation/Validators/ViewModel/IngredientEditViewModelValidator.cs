using Cooking.WPF.Validation;
using FluentValidation;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// Validator for <see cref="IngredientEditViewModel"/> class.
/// </summary>
public class IngredientEditViewModelValidator : AbstractValidator<IngredientEditViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IngredientEditViewModelValidator"/> class.
    /// </summary>
    public IngredientEditViewModelValidator()
    {
        RuleFor(x => x.Ingredient).Must(x => x.IsValid());
    }
}
