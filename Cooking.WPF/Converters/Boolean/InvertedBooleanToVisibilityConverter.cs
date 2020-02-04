using System.Windows;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// Implementation of BooleanConverter when true means Collapsed and false means Visible.
    /// </summary>
    public sealed class InvertedBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanToVisibilityConverter"/> class.
        /// </summary>
        public InvertedBooleanToVisibilityConverter()
            : base(Visibility.Collapsed, Visibility.Visible)
        {
        }
    }
}
