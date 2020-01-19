using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// <see cref="Controls.Ratings"/> control-specific controller
    /// Returns current brush for a column
    ///
    /// Accepts 3 values: current column index, rating value and rating preview value
    ///
    /// If rating preview is not null and it's value greater or equals to column index (mouse is over one of columns to the right) - returns PreviewBrush
    /// If rating preview is null and rating value greater or equals to column index - returns OnBrush
    /// Otherwise returs OffBrush.
    /// </summary>
    public class RatingConverter : IMultiValueConverter
    {
        public Brush? PreviewBrush { get; set; }
        public Brush? OnBrush { get; set; }
        public Brush? OffBrush { get; set; }

        public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values?.Length != 3)
            {
                return null;
            }

            int valueIndex = (int)values[0];
            int? rating = (int?)values[1];
            int? ratingPreview = (int?)values[2];

            if (ratingPreview >= valueIndex)
            {
                return PreviewBrush;
            }

            if (rating >= valueIndex)
            {
                return OnBrush;
            }

            return OffBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
