using Cooking.DTO;
using FluentValidation;

namespace Cooking.DTO
{
    public class RecipeEditValidator : AbstractValidator<RecipeEdit>
    {
        public RecipeEditValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Нужно указать название");
        }
    }
}
