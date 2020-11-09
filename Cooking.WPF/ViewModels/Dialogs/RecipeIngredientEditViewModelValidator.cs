using Cooking.WPF.Validation;
using FluentValidation;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// Validator type for <see cref="RecipeIngredientEditViewModel"/> class.
    /// </summary>
    public class RecipeIngredientEditViewModelValidator : AbstractValidator<RecipeIngredientEditViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeIngredientEditViewModelValidator"/> class.
        /// </summary>
        public RecipeIngredientEditViewModelValidator()
        {
            RuleFor(x => x.Ingredient).Must(x => x.IsValid());
        }
    }
}