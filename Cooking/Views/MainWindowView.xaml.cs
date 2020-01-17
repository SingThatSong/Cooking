using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Regions;

namespace Cooking
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindowView
    {
        private readonly IRegionManager regionManager;

        public MainWindowView(IRegionManager regionManager)
        {
            InitializeComponent();
            this.regionManager = regionManager;

            DialogParticipation.SetRegister(this, DataContext);
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.InvokedItem is HamburgerMenuItem hamburgerMenuItem 
             && hamburgerMenuItem.Tag is string typeName)
            {
                regionManager.RequestNavigate(Consts.MainContentRegion, typeName);
            }
        }
    }
}
