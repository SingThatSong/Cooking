using Cooking.WPF.Helpers;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    public class GarnishEditValidator : AbstractValidator<GarnishEdit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GarnishEditValidator"/> class.
        /// </summary>
        /// <param name="localization"></param>
        public GarnishEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("SpecifyName"));
        }
    }
}
