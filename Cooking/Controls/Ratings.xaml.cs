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

        private void Rectangle1_Mousep(object sender, MouseButtonEventArgs e)
        {
            RatingValue = 1;
        }

        private void Rectangle2_Mousep(object sender, MouseButtonEventArgs e)
        {
            RatingValue = 2;
        }

        private void Rectangle3_Mousep(object sender, MouseButtonEventArgs e)
        {
            RatingValue = 3;
        }

        private void Rectangle4_Mousep(object sender, MouseButtonEventArgs e)
        {
            RatingValue = 4;
        }

        private void Rectangle5_Mousep(object sender, MouseButtonEventArgs e)
        {
            RatingValue = 5;
        }

        private void Rectangle6_Mousep(object sender, MouseButtonEventArgs e)
        {
            RatingValue = 6;
        }

        private void Rectangle7_Mousep(object sender, MouseButtonEventArgs e)
        {
            RatingValue = 7;
        }

        private void Rectangle8_Mousep(object sender, MouseButtonEventArgs e)
        {
            RatingValue = 8;
        }

        private void Rectangle9_Mousep(object sender, MouseButtonEventArgs e)
        {
            RatingValue = 9;
        }

        private void Rectangle10_Mousep(object sender, MouseButtonEventArgs e)
        {
            RatingValue = 10;
        }
    }

    public class RatingConverter : IValueConverter
    {
        public Brush OnBrush { get; set; }
        public Brush OffBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
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
