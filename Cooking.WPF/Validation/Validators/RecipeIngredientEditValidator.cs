using Cooking.ServiceLayer;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// FluentValidation Validator for <see cref="RecipeIngredientEdit"/>.
    /// </summary>
    public class RecipeIngredientEditValidator : AbstractValidator<RecipeIngredientEdit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeIngredientEditValidator"/> class.
        /// </summary>
        /// <param name="localization">Localization provider for eror messages.</param>
        public RecipeIngredientEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Ingredient)
                .NotNull()
                .WithMessage(localization["SpecifyIngredient"]);

            RuleFor(x => x.Amount)
                .NotEmpty()
                .When(x => x.MeasureUnit != null)
                .WithMessage(localization["SpecifyAmountIfMeasureUnit"]);

            RuleFor(x => x.MeasureUnit)
                .NotNull()
                .When(x => !string.IsNullOrEmpty(x.Amount))
                .WithMessage(localization["SpecifyMeasureUnitIfAmount"]);
        }
    }
}
