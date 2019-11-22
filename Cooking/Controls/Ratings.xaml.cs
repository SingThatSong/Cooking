using Bindables;
using Cooking.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cooking.Controls
{
    /// <summary>
    /// Ratings control
    /// </summary>
    public partial class Ratings : UserControl
    {
        public Ratings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Internal value for determining column height, which is HeightStep * ColumnValue
        /// </summary>
        [DependencyProperty] public double HeightStep { get; set; }

        /// <summary>
        /// Internal representation of ratings. List of all possible rating values, based on MaxRating
        /// </summary>
        [DependencyProperty] public List<int> RatingsInternal { get; private set; }

        /// <summary>
        /// Integer value of rating for visual representation. Equals to RatingValue when idle or RatingValuePreview when MouseOver
        /// </summary>
        [DependencyProperty] public int? IntegerValue { get; set; }

        /// <summary>
        /// RatingValue which is underneath mouse when MouseOver
        /// </summary>
        [DependencyProperty(OnPropertyChanged = nameof(OnRatingPreviewChanged))]
        public int? RatingValuePreview { get; set; }
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

        /// <summary>
        /// Selected rating
        /// </summary>
        [DependencyProperty(OnPropertyChanged = nameof(OnRatingChanged))]
        public int? RatingValue { get; set; }
        private static void OnRatingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is Ratings obj)
            {
                obj.IntegerValue = (int?)dependencyPropertyChangedEventArgs.NewValue;
            }
        }

        /// <summary>
        /// Maximum possible rating
        /// </summary>
        [DependencyProperty(OnPropertyChanged = nameof(OnMaxRatingChanged))]
        public int? MaxRating { get; set; }
        private static void OnMaxRatingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Only control initialization
            if (dependencyPropertyChangedEventArgs.OldValue == null
             && dependencyObject is Ratings obj)
            {
                obj.RatingsInternal = Enumerable.Range(1, (int)dependencyPropertyChangedEventArgs.NewValue).ToList();
                var height = obj.GetValue(HeightProperty);
                obj.HeightStep = (double)height / obj.RatingsInternal.Count;
            }
        }


        public DelegateCommand ClearValueCommand     => new DelegateCommand(() => RatingValue = null);
        public DelegateCommand<int> ClickCommand     => new DelegateCommand<int>(i => RatingValue = i);

        public DelegateCommand<int> MouseOverCommand => new DelegateCommand<int>(i => RatingValuePreview = i);
        public DelegateCommand MouseLeaveCommand     => new DelegateCommand(() => RatingValuePreview = null);
    }
}
