using Cooking.WPF.Helpers;
using FluentValidation;

namespace Cooking.DTO
{
    public class RecipeIngredientEditValidator : AbstractValidator<RecipeIngredientEdit>
    {
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
