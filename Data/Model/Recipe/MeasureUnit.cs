using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Data.Model
{
    public class MeasureUnit
    {
        private MeasureUnit() { }

        public static MeasureUnit Gram = new MeasureUnit(id: 1, name: "г", fullName: "грамм" );
        public static MeasureUnit Ml = new MeasureUnit(id: 2, name: "мл", fullName: "миллилитр" );
        public static MeasureUnit Unit = new MeasureUnit(id: 3, name: "шт", fullName: "штука" );
        public static MeasureUnit TableSpoon = new MeasureUnit(id: 4, name: "ст.л.", fullName: "столовая ложка" );
        public static MeasureUnit TeaSpoon = new MeasureUnit(id: 5, name: "ч.л.", fullName: "чайная ложка" );
        public static MeasureUnit Cup = new MeasureUnit(id: 6, name: "ст.", fullName: "стакан" );
        public static MeasureUnit Pinch = new MeasureUnit(id: 7, name: "щепотка", fullName: "щепотка" );
        public static MeasureUnit Sprig = new MeasureUnit(id: 8, name: "веточка", fullName: "веточка" );
        public static MeasureUnit Clove = new MeasureUnit(id: 9, name: "зубчик", fullName: "зубчик");
        public static MeasureUnit Bundle = new MeasureUnit(id: 10, name: "пучок", fullName: "пучок");

        private MeasureUnit(int id, string name, string fullName)
        {
            ID = id;
            Name = name;
            FullName = fullName;
        }

        public int ID { get; }
        public string Name { get; }
        public string FullName { get; }

        public static ReadOnlyCollection<MeasureUnit> AllValues { get; } = new ReadOnlyCollection<MeasureUnit>(new []
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