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
    public partial class RatingsFive : UserControl
    {
        public RatingsFive()
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
            DependencyProperty.Register("RatingValue", typeof(int?), typeof(RatingsFive));

        public int? RatingValuePreview
        {
            get { return (int?)GetValue(RatingValuePreviewProperty); }
            set { SetValue(RatingValuePreviewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RatingValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RatingValuePreviewProperty =
            DependencyProperty.Register("RatingValuePreview", typeof(int?), typeof(RatingsFive), new PropertyMetadata(null));

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
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            RatingValuePreview = null;
        }
    }
}
