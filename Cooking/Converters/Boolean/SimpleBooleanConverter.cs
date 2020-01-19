namespace Cooking.WPF.Converters
{
    public class SimpleBooleanConverter : BooleanConverter<bool>
    {
        public SimpleBooleanConverter()
            : base(true, false)
        {
        }

        public SimpleBooleanConverter(bool trueValue, bool falseValue)
            : base(trueValue, falseValue)
        {
        }
    }
}
