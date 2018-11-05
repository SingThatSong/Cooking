using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public double RatingValue
        {
            get { return (double)GetValue(RatingValueProperty); }
            set { SetValue(RatingValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RatingValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RatingValueProperty =
            DependencyProperty.Register("RatingValue", typeof(double), typeof(Ratings));

    }

    public class RatingConverter : IValueConverter
    {
        public Brush OnBrush { get; set; }
        public Brush OffBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((double)value != 0)
            {

            }

            double rating = 0;
            double number = 0;
            if (double.TryParse(value.ToString(), out rating) && double.TryParse(parameter.ToString(), out number))
            {
                if (rating >= number)
                {
                    return OnBrush;
                }
                return OffBrush;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
