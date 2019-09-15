using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Cooking.Converters
{
    public class RatingConverter : IMultiValueConverter
    {
        public Brush PreviewBrush { get; set; }
        public Brush OnBrush { get; set; }
        public Brush OffBrush { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3) return null;

            parameter = values[0];
            if (values[2] == null)
            {
                if (double.TryParse(values[1]?.ToString(), out double rating) && double.TryParse(parameter.ToString(), out double number))
                {
                    if (rating >= number)
                    {
                        return OnBrush;
                    }
                    return OffBrush;
                }
            }
            else
            {
                if (double.TryParse(parameter.ToString(), out double number))
                {
                    if ((int?)values[2] >= number)
                    {
                        return PreviewBrush;
                    }
                    return OffBrush;
                }
            }

            return OffBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
