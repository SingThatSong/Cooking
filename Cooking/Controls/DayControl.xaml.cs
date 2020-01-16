using Bindables;
using System;
using System.Windows.Controls;

namespace Cooking.WPF.Views
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

        public DayOfWeek? DayName { get; set; }
    }
}
