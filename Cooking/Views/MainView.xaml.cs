using Prism.Regions;
using System.Windows.Controls;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainView : UserControl, IRegionMemberLifetime
    {
        public MainView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
