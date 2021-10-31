using Cooking.WPF.Validation;
using FluentValidation;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// Validator for <see cref="TagEditViewModel"/> class.
/// </summary>
public class TagEditViewModelValidator : AbstractValidator<TagEditViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TagEditViewModelValidator"/> class.
    /// </summary>
    public TagEditViewModelValidator()
    {
        RuleFor(x => x.Tag).Must(x => x.IsValid());
    }
}
