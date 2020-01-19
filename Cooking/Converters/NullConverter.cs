using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// https://stackoverflow.com/a/5628347.
    /// </summary>
    public class NullImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value ?? DependencyProperty.UnsetValue;

        // According to https://msdn.microsoft.com/en-us/library/system.windows.data.ivalueconverter.convertback(v=vs.110).aspx#Anchor_1
        // (kudos Scott Chamberlain), if you do not support a conversion
        // back you should return a Binding.DoNothing or a
        // DependencyProperty.UnsetValue
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
