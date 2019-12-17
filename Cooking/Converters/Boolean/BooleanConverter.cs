﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Cooking.Converters
{
    // WPF Converter for converting bool values (e.x. Visibility)
    // https://stackoverflow.com/a/5182660
    public class BooleanConverter<T> : IValueConverter
    {
        public BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public T True { get; set; }
        public T False { get; set; }

        public virtual object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isTrue
                && isTrue ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }
    }
}
