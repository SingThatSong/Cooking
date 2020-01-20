using Bindables;
using Cooking.WPF.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cooking.WPF.Controls
{
    /// <summary>
    /// Ratings control.
    /// </summary>
    public partial class Ratings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ratings"/> class.
        /// </summary>
        public Ratings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets internal value for determining column height, which is HeightStep * ColumnValue.
        /// </summary>
        [DependencyProperty]
        public double HeightStep { get; set; }

        /// <summary>
        /// Gets internal representation of ratings. List of all possible rating values, based on MaxRating.
        /// </summary>
        [DependencyProperty]
        public List<int>? RatingsInternal { get; private set; }

        /// <summary>
        /// Gets or sets integer value of rating for visual representation. Equals to RatingValue when idle or RatingValuePreview when MouseOver.
        /// </summary>
        [DependencyProperty]
        public int? IntegerValue { get; set; }

        /// <summary>
        /// Gets or sets ratingValue which is underneath mouse when MouseOver.
        /// </summary>
        [DependencyProperty(OnPropertyChanged = nameof(OnRatingPreviewChanged))]
        public int? RatingValuePreview { get; set; }

        /// <summary>
        /// Gets or sets selected rating.
        /// </summary>
        [DependencyProperty(OnPropertyChanged = nameof(OnRatingChanged))]
        public int? RatingValue { get; set; }

        /// <summary>
        /// Gets or sets maximum possible rating.
        /// </summary>
        [DependencyProperty(OnPropertyChanged = nameof(OnMaxRatingChanged))]
        public int? MaxRating { get; set; }

        /// <summary>
        /// Gets command to clear rating value.
        /// </summary>
        public DelegateCommand ClearValueCommand     => new DelegateCommand(() => RatingValue = null);

        /// <summary>
        /// Gets command to set value on click.
        /// </summary>
        public DelegateCommand<int> ClickCommand     => new DelegateCommand<int>(i => RatingValue = i);

        /// <summary>
        /// Gets command to set preview value on MouseOver.
        /// </summary>
        public DelegateCommand<int> MouseOverCommand => new DelegateCommand<int>(i => RatingValuePreview = i);

        /// <summary>
        /// Gets command to erase preview value on MouseLeave.
        /// </summary>
        public DelegateCommand MouseLeaveCommand     => new DelegateCommand(() => RatingValuePreview = null);

        private static void OnRatingPreviewChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is Ratings obj)
            {
                if (dependencyPropertyChangedEventArgs.NewValue != null)
                {
                    obj.IntegerValue = (int?)dependencyPropertyChangedEventArgs.NewValue;
                }
                else
                {
                    // Mouse leaving control, fall back to RatingValue
                    obj.IntegerValue = obj.RatingValue;
                }
            }
        }

        private static void OnRatingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is Ratings obj)
            {
                obj.IntegerValue = (int?)dependencyPropertyChangedEventArgs.NewValue;
            }
        }

        private static void OnMaxRatingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Only control initialization
            if (dependencyPropertyChangedEventArgs.OldValue == null
             && dependencyObject is Ratings obj)
            {
                obj.RatingsInternal = Enumerable.Range(1, (int)dependencyPropertyChangedEventArgs.NewValue).ToList();
                object height = obj.GetValue(HeightProperty);
                obj.HeightStep = (double)height / obj.RatingsInternal.Count;
            }
        }
    }
}
