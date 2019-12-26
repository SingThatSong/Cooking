using System.Collections.ObjectModel;

namespace Data.Model
{
    /// <summary>
    /// Typesafe enum pattern
    /// See https://www.infoworld.com/article/3198453/how-to-implement-a-type-safe-enum-pattern-in-c.html
    /// </summary>
    public class MeasureUnit
    {
        public static readonly MeasureUnit Gram = new MeasureUnit(id: 1, name: "г", fullName: "грамм");
        public static readonly MeasureUnit Ml = new MeasureUnit(id: 2, name: "мл", fullName: "миллилитр");
        public static readonly MeasureUnit Unit = new MeasureUnit(id: 3, name: "шт", fullName: "штука");
        public static readonly MeasureUnit TableSpoon = new MeasureUnit(id: 4, name: "ст.л.", fullName: "столовая ложка");
        public static readonly MeasureUnit TeaSpoon = new MeasureUnit(id: 5, name: "ч.л.", fullName: "чайная ложка");
        public static readonly MeasureUnit Cup = new MeasureUnit(id: 6, name: "ст.", fullName: "стакан");
        public static readonly MeasureUnit Pinch = new MeasureUnit(id: 7, name: "щепотка", fullName: "щепотка");
        public static readonly MeasureUnit Sprig = new MeasureUnit(id: 8, name: "веточка", fullName: "веточка");
        public static readonly MeasureUnit Clove = new MeasureUnit(id: 9, name: "зубчик", fullName: "зубчик");
        public static readonly MeasureUnit Bundle = new MeasureUnit(id: 10, name: "пучок", fullName: "пучок");

        private MeasureUnit(int id, string name, string fullName)
        {
            ID = id;
            Name = name;
            FullName = fullName;
        }

        public int ID { get; }
        public string Name { get; }
        public string FullName { get; }

        public static ReadOnlyCollection<MeasureUnit> AllValues { get; } = new ReadOnlyCollection<MeasureUnit>(new[]
        {
            Gram,
            Ml,
            Unit,
            TableSpoon,
            TeaSpoon,
            Cup,
            Pinch,
            Sprig,
            Clove,
            Bundle
        });
    }
}