using Prism.Regions;
using System.Globalization;
using System.Windows.Controls;
using WPFLocalizeExtension.Engine;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml.
    /// </summary>
    public partial class SettingsView : UserControl, IRegionMemberLifetime
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
