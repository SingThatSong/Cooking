using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cooking.WPF.Converters
{
    public class IsNullToVisibilityConverter : IValueConverter
    {
        public Visibility NullVisibility { get; set; }
        public Visibility NotNullVisibility { get; set; }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value == null ? NullVisibility : NotNullVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("IsNullConverter can only be used OneWay.");
        }
    }
}
