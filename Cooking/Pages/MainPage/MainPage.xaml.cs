using Prism.Regions;
using System.Windows.Controls;

namespace Cooking.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl, IRegionMemberLifetime
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
