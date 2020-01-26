using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Typesafe enum pattern
    /// See https://www.infoworld.com/article/3198453/how-to-implement-a-type-safe-enum-pattern-in-c.html.
    /// </summary>
    public class IngredientType : TypesafeEnum
    {
        /// <summary>
        /// Алкоголь.
        /// </summary>
        public static readonly IngredientType Alcohol = new IngredientType(id: 1, name: "Алкоголь");

        /// <summary>
        /// Крупы, бобовые и мука.
        /// </summary>
        public static readonly IngredientType Cereals = new IngredientType(id: 2, name: "Крупы, бобовые и мука");

        /// <summary>
        /// Рыба и морепродукты.
        /// </summary>
        public static readonly IngredientType Seafood = new IngredientType(id: 3, name: "Рыба и морепродукты");

        /// <summary>
        /// Бакалея.
        /// </summary>
        public static readonly IngredientType Grocery = new IngredientType(id: 4, name: "Бакалея");

        /// <summary>
        /// Специи и приправы.
        /// </summary>
        public static readonly IngredientType Spice = new IngredientType(id: 5, name: "Специи и приправы");

        /// <summary>
        /// Молочные продукты и яйца.
        /// </summary>
        public static readonly IngredientType Dairy = new IngredientType(id: 6, name: "Молочные продукты и яйца");

        /// <summary>
        /// Сыры.
        /// </summary>
        public static readonly IngredientType Cheese = new IngredientType(id: 7, name: "Сыры");

        /// <summary>
        /// Овощи и корнеплоды.
        /// </summary>
        public static readonly IngredientType Vegetables = new IngredientType(id: 8, name: "Овощи и корнеплоды");

        /// <summary>
        /// Фрукты и ягоды.
        /// </summary>
        public static readonly IngredientType Fruits = new IngredientType(id: 9, name: "Фрукты и ягоды");

        /// <summary>
        /// Грибы.
        /// </summary>
        public static readonly IngredientType Mushrooms = new IngredientType(id: 10, name: "Грибы");

        /// <summary>
        /// Зелень и травы.
        /// </summary>
        public static readonly IngredientType Herbs = new IngredientType(id: 11, name: "Зелень и травы");

        /// <summary>
        /// Мясо и птица.
        /// </summary>
        public static readonly IngredientType Meat = new IngredientType(id: 12, name: "Мясо и птица");

        /// <summary>
        /// Орехи.
        /// </summary>
        public static readonly IngredientType Nuts = new IngredientType(id: 13, name: "Орехи");

        /// <summary>
        /// Готовые продукты.
        /// </summary>
        public static readonly IngredientType Ready = new IngredientType(id: 14, name: "Готовые продукты");

        private IngredientType(int id, string name)
            : base(id)
        {
            Name = name;
        }

        /// <summary>
        /// Gets all values for <see cref="IngredientType"/>.
        /// </summary>
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

        /// <summary>
        /// Gets ingredient type name.
        /// </summary>
        public string Name { get; }
    }
}