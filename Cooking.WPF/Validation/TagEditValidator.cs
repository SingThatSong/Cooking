using Cooking.Data.Model;
using Cooking.ServiceLayer;
using FluentValidation;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// FluentValidation Validator for <see cref="TagEdit"/>.
    /// </summary>
    public class TagEditValidator : AbstractValidator<TagEdit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagEditValidator"/> class.
        /// </summary>
        /// <param name="localization">Localization provider for eror messages.</param>
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
