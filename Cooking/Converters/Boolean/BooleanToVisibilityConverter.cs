using System.Windows;

namespace Cooking.WPF.Converters
{
    // https://stackoverflow.com/a/5182660
    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }
}
