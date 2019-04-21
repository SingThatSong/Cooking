using System.Windows;
using System.Windows.Controls;

namespace Cooking.Pages.MainPage
{
    /// <summary>
    /// Логика взаимодействия для DayControl.xaml
    /// </summary>
    public partial class DayControl : UserControl
    {
        public DayControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty EditProperty =
                            DependencyProperty.Register("Edit",
                            typeof(bool),
                            typeof(DayControl)
        );

        public bool Edit
        {
            get { return (bool)GetValue(EditProperty); }
            set { SetValue(EditProperty, value); }
        }

        public static readonly DependencyProperty DayNameProperty =
                            DependencyProperty.Register("DayName",
                            typeof(string),
                            typeof(DayControl)
        );

        public string DayName
        {
            get { return (string)GetValue(DayNameProperty); }
            set { SetValue(DayNameProperty, value); }
        }
    }
}
