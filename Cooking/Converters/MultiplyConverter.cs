using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Cooking.Converters
{
    /// <summary>
    /// Converter which returns multiplication of its parameters. All of the parameters must be convertable to double (e.g. int, double, float, etc.)
    /// </summary>
    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0) 
                return Binding.DoNothing;

            var result = System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);

            foreach (var val in values.Skip(1))
            {
                result *= System.Convert.ToDouble(val, CultureInfo.InvariantCulture);
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
