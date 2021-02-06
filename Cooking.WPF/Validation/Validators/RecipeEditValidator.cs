using Cooking.ServiceLayer;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// FluentValidation Validator for <see cref="RecipeEdit"/>.
    /// </summary>
    public class RecipeEditValidator : AbstractValidator<RecipeEdit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeEditValidator"/> class.
        /// </summary>
        /// <param name="localization">Localization provider for eror messages.</param>
        public RecipeEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization["SpecifyName"]);

            RuleFor(x => x.PortionsCount)
                .Matches(@"^\d+$")
                .When(x => !string.IsNullOrEmpty(x.PortionsCount))
                .WithMessage(localization["ShouldBeNumber"]);
        }
    }
}
