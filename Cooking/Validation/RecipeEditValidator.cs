using Cooking.WPF.Helpers;
using FluentValidation;

namespace Cooking.DTO
{
    public class RecipeEditValidator : AbstractValidator<RecipeEdit>
    {
        public RecipeEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("SpecifyName"));
        }
    }
}
