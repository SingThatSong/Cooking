using Cooking.ServiceLayer;
using PropertyChanged;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class RecipeIngredientMain : RecipeIngredientData
    {
        public new IngredientMain Ingredient { get; set; }
    }
}