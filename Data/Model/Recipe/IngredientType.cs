using System.Collections.ObjectModel;
using System.Linq;

namespace Data.Model
{
    /// <summary>
    /// Typesafe enum pattern
    /// See https://www.infoworld.com/article/3198453/how-to-implement-a-type-safe-enum-pattern-in-c.html
    /// </summary>
    public class IngredientType
    {
        public static readonly IngredientType Alcohol = new IngredientType(id: 1, name: "Алкоголь");
        public static readonly IngredientType Cereals = new IngredientType(id: 2, name: "Крупы, бобовые и мука");
        public static readonly IngredientType Seafood = new IngredientType(id: 3, name: "Рыба и морепродукты");
        public static readonly IngredientType Grocery = new IngredientType(id: 4, name: "Бакалея");
        public static readonly IngredientType Spice = new IngredientType(id: 5, name: "Специи и приправы");
        public static readonly IngredientType Dairy = new IngredientType(id: 6, name: "Молочные продукты и яйца");
        public static readonly IngredientType Cheese = new IngredientType(id: 7, name: "Сыры");
        public static readonly IngredientType Vegetables = new IngredientType(id: 8, name: "Овощи и корнеплоды");
        public static readonly IngredientType Fruits = new IngredientType(id: 9, name: "Фрукты и ягоды");
        public static readonly IngredientType Mushrooms = new IngredientType(id: 10, name: "Грибы");
        public static readonly IngredientType Herbs = new IngredientType(id: 11, name: "Зелень и травы");
        public static readonly IngredientType Meat = new IngredientType(id: 12, name: "Мясо и птица");
        public static readonly IngredientType Nuts = new IngredientType(id: 13, name: "Орехи");
        public static readonly IngredientType Ready = new IngredientType(id: 14, name: "Готовые продукты");

        private IngredientType(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public int ID { get; }
        public string Name { get; }

        public static ReadOnlyCollection<IngredientType> AllValues { get; } = new ReadOnlyCollection<IngredientType>(new[]
        {
            Alcohol,
            Cereals,
            Seafood,
            Grocery,
            Spice,
            Dairy,
            Cheese,
            Vegetables,
            Fruits,
            Mushrooms,
            Herbs,
            Meat,
            Nuts,
            Ready
        }.OrderBy(x => x.Name).ToList());
    }
}