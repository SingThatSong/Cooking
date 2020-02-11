using System.Collections.ObjectModel;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Measurement units hardcoded values.
    /// </summary>
    // TODO: Localization
    public class MeasureUnit : TypesafeEnum
    {
        /// <summary>
        /// грамм.
        /// </summary>
        public static readonly MeasureUnit Gram = new MeasureUnit(id: 1, name: "г", fullName: "грамм");

        /// <summary>
        /// миллили.
        /// </summary>р"
        public static readonly MeasureUnit Ml = new MeasureUnit(id: 2, name: "мл", fullName: "миллилитр");

        /// <summary>
        /// штука.
        /// </summary>
        public static readonly MeasureUnit Unit = new MeasureUnit(id: 3, name: "шт", fullName: "штука");

        /// <summary>
        /// столова.
        /// </summary> ложка"
        public static readonly MeasureUnit TableSpoon = new MeasureUnit(id: 4, name: "ст.л.", fullName: "столовая ложка");

        /// <summary>
        /// чайная.
        /// </summary>ожка"
        public static readonly MeasureUnit TeaSpoon = new MeasureUnit(id: 5, name: "ч.л.", fullName: "чайная ложка");

        /// <summary>
        /// стакан.
        /// </summary>
        public static readonly MeasureUnit Cup = new MeasureUnit(id: 6, name: "ст.", fullName: "стакан");

        /// <summary>
        /// щепотка.
        /// </summary>
        public static readonly MeasureUnit Pinch = new MeasureUnit(id: 7, name: "щепотка", fullName: "щепотка");

        /// <summary>
        /// веточка.
        /// </summary>
        public static readonly MeasureUnit Sprig = new MeasureUnit(id: 8, name: "веточка", fullName: "веточка");

        /// <summary>
        /// зубчик.
        /// </summary>
        public static readonly MeasureUnit Clove = new MeasureUnit(id: 9, name: "зубчик", fullName: "зубчик");

        /// <summary>
        /// пучок.
        /// </summary>
        public static readonly MeasureUnit Bundle = new MeasureUnit(id: 10, name: "пучок", fullName: "пучок");

        private MeasureUnit(int id, string name, string fullName)
            : base(id)
        {
            Name = name;
            FullName = fullName;
        }

        /// <summary>
        /// Gets all values for <see cref="MeasureUnit"/>.
        /// </summary>
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

        /// <summary>
        /// Gets measurement unit short name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets measurement unit full name.
        /// </summary>
        public string FullName { get; }
    }
}