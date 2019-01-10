using Cooking.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public int? RatingValue
        {
            get { return (int?)GetValue(RatingValueProperty); }
            set { SetValue(RatingValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RatingValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RatingValueProperty =
            DependencyProperty.Register("RatingValue", typeof(int?), typeof(Ratings));

        public int? RatingValuePreview
        {
            get { return (int?)GetValue(RatingValuePreviewProperty); }
            set { SetValue(RatingValuePreviewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RatingValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RatingValuePreviewProperty =
            DependencyProperty.Register("RatingValuePreview", typeof(int?), typeof(Ratings), new PropertyMetadata(null));

        public DelegateCommand ClearValueCommand => new DelegateCommand(() => RatingValue = null);

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

        private void Grid_MouseEnter1(object sender, MouseEventArgs e)
        {
            RatingValuePreview = 1;
        }
        private void Grid_MouseEnter2(object sender, MouseEventArgs e)
        {
            RatingValuePreview = 2;
        }
        private void Grid_MouseEnter3(object sender, MouseEventArgs e)
        {
            RatingValuePreview = 3;
        }
        private void Grid_MouseEnter4(object sender, MouseEventArgs e)
        {
            RatingValuePreview = 4;
        }
        private void Grid_MouseEnter5(object sender, MouseEventArgs e)
        {
            RatingValuePreview = 5;
        }
        private void Grid_MouseEnter6(object sender, MouseEventArgs e)
        {
            RatingValuePreview = 6;
        }
        private void Grid_MouseEnter7(object sender, MouseEventArgs e)
        {
            RatingValuePreview = 7;
        }
        private void Grid_MouseEnter8(object sender, MouseEventArgs e)
        {
            RatingValuePreview = 8;
        }
        private void Grid_MouseEnter9(object sender, MouseEventArgs e)
        {
            RatingValuePreview = 9;
        }
        private void Grid_MouseEnter10(object sender, MouseEventArgs e)
        {
            RatingValuePreview = 10;
        }
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            RatingValuePreview = null;
        }
    }

    public class RatingConverter : IMultiValueConverter
    {
        public Brush PreviewBrush { get; set; }
        public Brush OnBrush { get; set; }
        public Brush OffBrush { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] == null)
            {
                double rating = 0;
                double number = 0;
                if (double.TryParse(values[0]?.ToString(), out rating) && double.TryParse(parameter.ToString(), out number))
                {
                    if (rating >= number)
                    {
                        return OnBrush;
                    }
                    return OffBrush;
                }
            }
            else
            {
                double number = 0;
                if (double.TryParse(parameter.ToString(), out number))
                {
                    if ((int?)values[1] >= number)
                    {
                        return PreviewBrush;
                    }
                    return OffBrush;
                }
            }

            return OffBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
