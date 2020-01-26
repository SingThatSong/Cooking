using Cooking.WPF.Services;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// FluentValidation Validator for <see cref="GarnishEdit"/>.
    /// </summary>
    public class GarnishEditValidator : AbstractValidator<GarnishEdit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GarnishEditValidator"/> class.
        /// </summary>
        /// <param name="localization">Localization provider for eror messages.</param>
        public GarnishEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("SpecifyName"));
        }
    }
}
