using Bindables;
using Cooking.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cooking.Controls
{
    /// <summary>
    /// Логика взаимодействия для Ratings.xaml
    /// </summary>
    
    public partial class Ratings : UserControl
    {
        public Ratings()
        {
            InitializeComponent();
        }

        [DependencyProperty] public double HeightStep { get; set; }
        [DependencyProperty] public List<int> RatingsInternal { get; set; }
        [DependencyProperty] public int? IntegerValue { get; set; }
        [DependencyProperty] public int? RatingValuePreview { get; set; }


        [DependencyProperty(OnPropertyChanged = nameof(OnRatingChanged))]
        public int? RatingValue { get; set; }

        [DependencyProperty(OnPropertyChanged = nameof(OnMaxRatingChanged))]
        public int? MaxRating { get; set; }

        private static void OnMaxRatingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyPropertyChangedEventArgs.OldValue == null)
            {
                var obj = dependencyObject as Ratings;
                obj.RatingsInternal = Enumerable.Range(1, (int)dependencyPropertyChangedEventArgs.NewValue).ToList();
                var height = obj.GetValue(HeightProperty);
                obj.HeightStep = (double)height / obj.RatingsInternal.Count;
            }
        }

        private static void OnRatingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyPropertyChangedEventArgs.OldValue == null)
            {
                var obj = dependencyObject as Ratings;
                obj.IntegerValue = (int?)dependencyPropertyChangedEventArgs.NewValue;
            }
        }

        public DelegateCommand ClearValueCommand => new DelegateCommand(() =>
        {
            RatingValue = null;
            IntegerValue = null;
        });

        public DelegateCommand<int> ClickCommand => new DelegateCommand<int>(i =>
        {
            RatingValue = i;
            IntegerValue = i;
        });

        public DelegateCommand<int> MouseOverCommand => new DelegateCommand<int>(i => {
            RatingValuePreview = i;
            IntegerValue = i;
        });

        public DelegateCommand MouseLeaveCommand => new DelegateCommand(() =>
        {
            RatingValuePreview = null;
            IntegerValue = RatingValue;
        });
    }
}
