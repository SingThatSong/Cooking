using System.ComponentModel;

namespace Cooking.Data.Model
{
    public enum CalorieType
    {
        [Description("Фитнес рецепт, подходит для похудения")]
        Fitness = 1,
        [Description("Хороший рецепт, достаточно белка")]
        Protein = 2,
        [Description("Либо калорийный, либо много углеводов")]
        Bad = 3,
        [Description("Калорийная вкусняшка")]
        Sweets = 4
    }
}