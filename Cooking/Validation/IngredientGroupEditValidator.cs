using Cooking.DTO;
using FluentValidation;

namespace Cooking.DTO
{
    public class IngredientGroupEditValidator : AbstractValidator<IngredientGroupEdit>
    {
        public IngredientGroupEditValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Нужно указать название");
        }
    }
}
