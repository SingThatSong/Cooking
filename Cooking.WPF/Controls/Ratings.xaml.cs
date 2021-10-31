using System.Windows;
using System.Windows.Controls;
using WPF.Commands;

namespace Cooking.WPF.Controls;

/// <summary>
/// Ratings control.
/// </summary>
public partial class Ratings : UserControl
{
    /// <summary>
    /// DependencyProperty for <see cref="HeightStep"/>.
    /// </summary>
    public static readonly DependencyProperty HeightStepProperty = DependencyProperty.Register(
                                                                        nameof(HeightStep),
                                                                        typeof(double),
                                                                        typeof(Ratings));

    /// <summary>
    /// DependencyProperty for <see cref="RatingsInternal"/>.
    /// </summary>
    public static readonly DependencyProperty RatingsInternalProperty = DependencyProperty.Register(
                                                                            nameof(RatingsInternal),
                                                                            typeof(List<int>),
                                                                            typeof(Ratings));

    /// <summary>
    /// DependencyProperty for <see cref="IntegerValue"/>.
    /// </summary>
    public static readonly DependencyProperty IntegerValueProperty = DependencyProperty.Register(
                                                                            nameof(IntegerValue),
                                                                            typeof(int?),
                                                                            typeof(Ratings));

    /// <summary>
    /// DependencyProperty for <see cref="RatingValuePreview"/>.
    /// </summary>
    public static readonly DependencyProperty RatingValuePreviewProperty = DependencyProperty.Register(
                                                                            nameof(RatingValuePreview),
                                                                            typeof(int?),
                                                                            typeof(Ratings),
                                                                            new PropertyMetadata(propertyChangedCallback: OnRatingPreviewChanged));

    /// <summary>
    /// DependencyProperty for <see cref="RatingValue"/>.
    /// </summary>
    public static readonly DependencyProperty RatingValueProperty = DependencyProperty.Register(
                                                                            nameof(RatingValue),
                                                                            typeof(int?),
                                                                            typeof(Ratings),
                                                                            new PropertyMetadata(propertyChangedCallback: OnRatingChanged));

    /// <summary>
    /// DependencyProperty for <see cref="MaxRating"/>.
    /// </summary>
    public static readonly DependencyProperty MaxRatingProperty = DependencyProperty.Register(
                                                                            nameof(MaxRating),
                                                                            typeof(int?),
                                                                            typeof(Ratings),
                                                                            new PropertyMetadata(propertyChangedCallback: OnMaxRatingChanged));

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
    public double HeightStep
    {
        get => (double)GetValue(HeightStepProperty);
        set => SetValue(HeightStepProperty, value);
    }

    /// <summary>
    /// Gets or sets internal representation of ratings. List of all possible rating values, based on MaxRating.
    /// </summary>
    public List<int>? RatingsInternal
    {
        get => (List<int>?)GetValue(RatingsInternalProperty);
        set => SetValue(RatingsInternalProperty, value);
    }

    /// <summary>
    /// Gets or sets integer value of rating for visual representation. Equals to RatingValue when idle or RatingValuePreview when MouseOver.
    /// </summary>
    public int? IntegerValue
    {
        get => (int?)GetValue(IntegerValueProperty);
        set => SetValue(IntegerValueProperty, value);
    }

    /// <summary>
    /// Gets or sets ratingValue which is underneath mouse when MouseOver.
    /// </summary>
    public int? RatingValuePreview
    {
        get => (int?)GetValue(RatingValuePreviewProperty);
        set => SetValue(RatingValuePreviewProperty, value);
    }

    /// <summary>
    /// Gets or sets selected rating.
    /// </summary>
    public int? RatingValue
    {
        get => (int?)GetValue(RatingValueProperty);
        set => SetValue(RatingValueProperty, value);
    }

    /// <summary>
    /// Gets or sets maximum possible rating.
    /// </summary>
    public int? MaxRating
    {
        get => (int?)GetValue(MaxRatingProperty);
        set => SetValue(MaxRatingProperty, value);
    }

    /// <summary>
    /// Gets command to clear rating value.
    /// </summary>
    public DelegateCommand ClearValueCommand => new(() => RatingValue = null);

    /// <summary>
    /// Gets command to set value on click.
    /// </summary>
    public DelegateCommand<int> ClickCommand => new(i => RatingValue = i);

    /// <summary>
    /// Gets command to set preview value on MouseOver.
    /// </summary>
    public DelegateCommand<int> MouseOverCommand => new(i => RatingValuePreview = i);

    /// <summary>
    /// Gets command to erase preview value on MouseLeave.
    /// </summary>
    public DelegateCommand MouseLeaveCommand => new(() => RatingValuePreview = null);

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
