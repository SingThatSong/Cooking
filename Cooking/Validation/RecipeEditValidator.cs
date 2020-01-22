using Cooking.WPF.Helpers;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    public class RecipeEditValidator : AbstractValidator<RecipeEdit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeEditValidator"/> class.
        /// </summary>
        /// <param name="localization"></param>
        public RecipeEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("SpecifyName"));
        }
    }
}
