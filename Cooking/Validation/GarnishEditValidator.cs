using Cooking.WPF.Helpers;
using FluentValidation;

namespace Cooking.DTO
{
    public class GarnishEditValidator : AbstractValidator<GarnishEdit>
    {
        public GarnishEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("SpecifyName"));
        }
    }
}
