using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Regions;
using System;

namespace Cooking
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IRegionManager regionManager;

        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            this.regionManager = regionManager;

            DialogParticipation.SetRegister(this, DataContext);
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.InvokedItem is HamburgerMenuItem hamburgerMenuItem 
             && hamburgerMenuItem.Tag is Type type)
            {
                regionManager.RequestNavigate(Consts.MainContentRegion, type.Name);
            }
        }
    }
}
