using Cooking.WPF.Services;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// FluentValidation Validator for <see cref="IngredientEdit"/>.
    /// </summary>
    public class IngredientEditValidator : AbstractValidator<IngredientEdit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientEditValidator"/> class.
        /// </summary>
        /// <param name="localization">Localization provider for eror messages.</param>
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
