using Cooking.WPF.Helpers;
using Data.Model;
using FluentValidation;

namespace Cooking.DTO
{
    public class TagEditValidator : AbstractValidator<TagEdit>
    {
        public TagEditValidator(ILocalization localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("SpecifyName"));

            RuleFor(x => x.Type)
                .NotEqual((TagType)0)
                .WithMessage(localization.GetLocalizedString("SpecifyTagType"));
        }
    }
}
