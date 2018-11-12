using System.Collections.ObjectModel;
using System.Linq;

namespace Data.Model
{
    public class IngredientType
    {
        private IngredientType() { }

        public static IngredientType Alcohol = new IngredientType(id: 1, name: "Алкоголь");
        public static IngredientType Cereals = new IngredientType(id: 2, name: "Крупы, бобовые и мука");
        public static IngredientType Seafood = new IngredientType(id: 3, name: "Рыба и морепродукты");
        public static IngredientType Grocery = new IngredientType(id: 4, name: "Бакалея");
        public static IngredientType Spice = new IngredientType(id: 5, name: "Специи и приправы");
        public static IngredientType Dairy = new IngredientType(id: 6, name: "Молочные продукты и яйца");
        public static IngredientType Cheese = new IngredientType(id: 7, name: "Сыры");
        public static IngredientType Vegetables = new IngredientType(id: 8, name: "Овощи и корнеплоды");
        public static IngredientType Fruits = new IngredientType(id: 9, name: "Фрукты и ягоды");
        public static IngredientType Mushrooms = new IngredientType(id: 10, name: "Грибы");
        public static IngredientType Herbs = new IngredientType(id: 11, name: "Зелень и травы");
        public static IngredientType Meat = new IngredientType(id: 12, name: "Мясо и птица");
        public static IngredientType Nuts = new IngredientType(id: 13, name: "Орехи");
        public static IngredientType Ready = new IngredientType(id: 14, name: "Готовые продукты");

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