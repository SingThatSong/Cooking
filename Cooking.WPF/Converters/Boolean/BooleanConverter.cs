using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// WPF Converter for converting bool values into other types (e.x. Visibility)
    /// https://stackoverflow.com/a/5182660.
    /// </summary>
    /// <typeparam name="T">Type to convert bool value into.</typeparam>
    public class BooleanConverter<T> : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanConverter{T}"/> class.
        /// </summary>
        /// <param name="trueValue">Value indicating True of type T.</param>
        /// <param name="falseValue">Value indicating False of type T.</param>
        public BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        /// <summary>
        /// Gets or sets value which will be used instead of boolean true.
        /// </summary>
        public T True { get; set; }

        /// <summary>
        /// Gets or sets value which will be used instead of boolean false.
        /// </summary>
        public T False { get; set; }

        /// <inheritdoc/>
        public virtual object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? True : False;
            }
            else
            {
                return False;
            }
        }

        /// <inheritdoc/>
        public virtual object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture) => value is T && EqualityComparer<T>.Default.Equals((T)value, True);
    }
}
