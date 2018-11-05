using System.Collections.Generic;

namespace Data.Model
{
    public class MeasureUnit
    {
        private MeasureUnit() { }

        public static MeasureUnit Gram = new MeasureUnit() { ID = 1, Name = "г", FullName = "грамм" };
        public static MeasureUnit Ml = new MeasureUnit() { ID = 2, Name = "мл", FullName = "миллилитр" };
        public static MeasureUnit Unit = new MeasureUnit() { ID = 3, Name = "шт", FullName = "штука" };
        public static MeasureUnit TableSpoon = new MeasureUnit() { ID = 4, Name = "ст.л.", FullName = "столовая ложка" };
        public static MeasureUnit TeaSpoon = new MeasureUnit() { ID = 5, Name = "ч.л.", FullName = "чайная ложка" };
        public static MeasureUnit Cup = new MeasureUnit() { ID = 6, Name = "ст.", FullName = "стакан" };
        public static MeasureUnit Pinch = new MeasureUnit() { ID = 7, Name = "щепотка", FullName = "щепотка" };
        public static MeasureUnit Sprig = new MeasureUnit() { ID = 8, Name = "веточка", FullName = "веточка" };

        public int? ID { get; internal set; }
        public string Name { get; set; }
        public string FullName { get; set; }

        public static List<MeasureUnit> AllValues { get; } = new List<MeasureUnit>
        {
            Gram,
            Ml,
            Unit,
            TableSpoon,
            TeaSpoon,
            Cup,
            Pinch,
            Sprig
        };
    }
}