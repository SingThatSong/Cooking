using Cooking.WPF.Helpers;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    public class IngredientGroupEditValidator : AbstractValidator<IngredientGroupEdit>
    {
        public IngredientGroupEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("SpecifyName"));
        }
    }
}
