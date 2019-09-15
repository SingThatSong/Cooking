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

            var valueIndex = (int)values[0];
            var rating = (int?)values[1];
            var ratingPreview = (int?)values[2];

            if (ratingPreview == null)
            {
                if (rating >= valueIndex)
                {
                    return OnBrush;
                }
            }
            else if (ratingPreview >= valueIndex)
            {
                return PreviewBrush;
            }

            return OffBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
