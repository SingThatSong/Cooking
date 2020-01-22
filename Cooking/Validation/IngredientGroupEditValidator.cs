using Cooking.WPF.Helpers;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    public class IngredientGroupEditValidator : AbstractValidator<IngredientGroupEdit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientGroupEditValidator"/> class.
        /// </summary>
        /// <param name="localization"></param>
        public IngredientGroupEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("SpecifyName"));
        }
    }
}
