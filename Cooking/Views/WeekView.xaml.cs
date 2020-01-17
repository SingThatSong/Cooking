using Prism.Regions;
using System.Windows.Controls;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class WeekView : UserControl, IRegionMemberLifetime
    {
        public WeekView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
