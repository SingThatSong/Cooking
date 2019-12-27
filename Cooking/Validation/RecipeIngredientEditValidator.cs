using Cooking.DTO;
using FluentValidation;

namespace Cooking.DTO
{
    public class RecipeIngredientEditValidator : AbstractValidator<RecipeIngredientEdit>
    {
        public RecipeIngredientEditValidator()
        {
            RuleFor(x => x.Ingredient)
                .NotNull()
                .WithMessage("Ингредиент нужно указать");

            RuleFor(x => x.Amount)
                .NotNull()
                .When(x => x.MeasureUnit != null)
                .WithMessage("Если указана единица измерения, нужно количество");

            RuleFor(x => x.MeasureUnit)
                .NotNull()
                .When(x => x.Amount != null)
                .WithMessage("Если указано количество, нужна единица измерения");
        }
    }
}
