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
        /// <summary>
        /// Index of values array's position of valueIndex.
        /// </summary>
        private const int IndexOfvalueIndex = 0;

        /// <summary>
        /// Index of values array's position of rating.
        /// </summary>
        private const int RatingIndex = 1;

        /// <summary>
        /// Index of values array's position of ratingPreview.
        /// </summary>
        private const int RatingPreviewIndex = 2;

        /// <summary>
        /// Gets or sets brush that will be used for values which is about to be selected on MouseOver.
        /// </summary>
        public Brush? PreviewBrush { get; set; }

        /// <summary>
        /// Gets or sets brush that will be used for selected values.
        /// </summary>
        public Brush? OnBrush { get; set; }

        /// <summary>
        /// Gets or sets brush that will be used for not selected values.
        /// </summary>
        public Brush? OffBrush { get; set; }

        /// <inheritdoc/>
        public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            int parametersCount = 3;
            if (values?.Length != parametersCount)
            {
                return null;
            }

            int valueIndex     = (int)values[IndexOfvalueIndex];
            int? rating        = (int?)values[RatingIndex];
            int? ratingPreview = (int?)values[RatingPreviewIndex];

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

        /// <inheritdoc/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
