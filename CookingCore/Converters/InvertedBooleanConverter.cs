using System.Windows;

namespace Cooking.Converters
{
    public sealed class InvertedBooleanConverter : BooleanConverter<bool>
    {
        public InvertedBooleanConverter() :
            base(false, true)
        { }
    }
}
