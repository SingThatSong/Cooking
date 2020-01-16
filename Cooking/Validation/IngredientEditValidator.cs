using Cooking.WPF.Helpers;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    public class IngredientEditValidator : AbstractValidator<IngredientEdit>
    {
        public IngredientEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("SpecifyName"));

            RuleFor(x => x.Type)
                .NotNull()
                .WithMessage(localization.GetLocalizedString("SpecifyIngredientType"));
        }
    }
}
