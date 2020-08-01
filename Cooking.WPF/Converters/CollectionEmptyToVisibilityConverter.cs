using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// Converter that returns Visibility based on whether collection is empty.
    /// </summary>
    public class CollectionEmptyToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets visibility which will be used when converted value is null.
        /// </summary>
        public Visibility CollectionEmptyVisibility { get; set; }

        /// <summary>
        /// Gets or sets visibility which will be used when converted value is not null.
        /// </summary>
        public Visibility CollectionNotEmptyVisibility { get; set; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IEnumerable collection)
            {
                return collection.GetEnumerator().MoveNext() ? CollectionNotEmptyVisibility
                                                             : CollectionEmptyVisibility;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) => throw new InvalidOperationException($"{nameof(CollectionEmptyToVisibilityConverter)} can only be used OneWay.");
    }
}
