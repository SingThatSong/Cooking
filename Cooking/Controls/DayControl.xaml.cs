using Bindables;
using System;
using System.Windows.Controls;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для DayControl.xaml.
    /// </summary>
    [DependencyProperty]
    public partial class DayControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DayControl"/> class.
        /// </summary>
        public DayControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether control is in edit state.
        /// </summary>
        public bool Edit { get; set; }

        /// <summary>
        /// Gets or sets current day.
        /// </summary>
        public DayOfWeek? Day { get; set; }
    }
}
