using Cooking.WPF.Services;
using NullGuard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Cooking.WPF.Converters
{
    /// Converter for displaying <see cref="System.ComponentModel.DescriptionAttribute"/> values from enums
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Enum @enum)
            {
                return @enum.Description();
            }
            else if (value is IEnumerable collection)
            {
                var list = new List<string>();

                foreach (object? val in collection)
                {
                    if (val is Enum @enumValue)
                    {
                        string? description = @enumValue.Description();
                        if (description != null)
                        {
                            list.Add(description);
                        }
                    }
                }

                return list;
            }
            else
            {
                return value;
            }
        }

        public object? ConvertBack([AllowNull] object? value, Type? targetType, [AllowNull] object? parameter, CultureInfo culture)
        {
            if (value != null && targetType != null)
            {
                string? valAsString = value.ToString();

                if (valAsString != null)
                {
                    return targetType.Enum(valAsString);
                }
            }

            return Binding.DoNothing;
        }
    }
}
