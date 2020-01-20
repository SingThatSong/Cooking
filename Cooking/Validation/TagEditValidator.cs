﻿using Cooking.Data.Model;
using Cooking.WPF.Helpers;
using FluentValidation;

namespace Cooking.WPF.DTO
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
