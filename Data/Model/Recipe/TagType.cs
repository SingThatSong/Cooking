using System.ComponentModel;

namespace Cooking.Data.Model
{
    public enum TagType
    {
        [Description("Dish type")]
        DishType = 1,
        [Description("Main Ingredient")]
        MainIngredient = 2,
        Occasion = 3,
        [Description("Источник")]
        Source = 4
    }
}