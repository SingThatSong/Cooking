using System.ComponentModel;

namespace Data.Model
{
    public enum TagType
    {
        [Description("Dish type")]
        DishType = 1,
        [Description("Main Ingredient")]
        MainIngredient = 2,
        Occasion = 3
    }
}