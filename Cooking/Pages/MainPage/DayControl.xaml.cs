using Bindables;
using System.Windows.Controls;

namespace Cooking.Pages.MainPage
{
    /// <summary>
    /// Логика взаимодействия для DayControl.xaml
    /// </summary>
    [DependencyProperty]
    public partial class DayControl : UserControl
    {
        public DayControl()
        {
            InitializeComponent();
        }

        public bool Edit { get; set; }

        public string DayName { get; set; }
    }
}
