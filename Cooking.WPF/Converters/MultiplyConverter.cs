using System;
using System.Globalization;
using System.Windows.Data;
using static System.Convert;

namespace Cooking.WPF.Converters;

/// <summary>
/// Converter which returns multiplication of its parameters. All parameters must be convertable to double (e.g. int, double, float, etc.).
/// </summary>
public class MultiplyConverter : IMultiValueConverter
{
    /// <inheritdoc/>
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Length == 0)
        {
            return Binding.DoNothing;
        }

        double result = ToDouble(values[0], CultureInfo.InvariantCulture);

        foreach (object val in values.Skip(1))
        {
            result *= ToDouble(val, CultureInfo.InvariantCulture);
        }

        return result;
    }

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => new[] { Binding.DoNothing };
}
