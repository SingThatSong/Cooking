using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// Converter which converts nullable values to Visibility.
    /// </summary>
    public class IsNullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets visibility which will be used when converted value is null.
        /// </summary>
        public Visibility NullVisibility { get; set; }

        /// <summary>
        /// Gets or sets visibility which will be used when converted value is not null.
        /// </summary>
        public Visibility NotNullVisibility { get; set; }

        /// <inheritdoc/>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value == null ? NullVisibility : NotNullVisibility;

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException($"{nameof(IsNullToVisibilityConverter)} can only be used OneWay.");
    }
}
