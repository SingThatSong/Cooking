using Prism.Regions;
using System.Windows.Controls;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for MainPage.xaml.
    /// </summary>
    public partial class WeekView : UserControl, IRegionMemberLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeekView"/> class.
        /// </summary>
        public WeekView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
