using Cooking.WPF.Services;
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
                .WithMessage(localization.GetLocalizedString("SpecifyIngredient"));

            RuleFor(x => x.Amount)
                .NotNull()
                .When(x => x.MeasureUnit != null)
                .WithMessage(localization.GetLocalizedString("SpecifyAmountIfMeasureUnit"));

            RuleFor(x => x.MeasureUnit)
                .NotNull()
                .When(x => x.Amount != null)
                .WithMessage(localization.GetLocalizedString("SpecifyMeasureUnitIfAmount"));
        }
    }
}
