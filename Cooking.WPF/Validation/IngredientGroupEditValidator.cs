using Cooking.WPF.Services;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// FluentValidation Validator for <see cref="IngredientGroupEdit"/>.
    /// </summary>
    public class IngredientGroupEditValidator : AbstractValidator<IngredientGroupEdit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientGroupEditValidator"/> class.
        /// </summary>
        /// <param name="localization">Localization provider for eror messages.</param>
        public IngredientGroupEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("SpecifyName"));
        }
    }
}
