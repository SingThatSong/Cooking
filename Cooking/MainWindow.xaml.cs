using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Ioc;
using Prism.Regions;
using System;

namespace Cooking
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IContainerExtension container;
        private readonly IRegionManager regionManager;

        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();
            this.container = container;
            this.regionManager = regionManager;


            DialogParticipation.SetRegister(this, DataContext);
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            var viewType = ((HamburgerMenuItem)e.InvokedItem).Tag as Type;
            var view = container.Resolve(viewType);

            IRegion region = regionManager.Regions[Consts.MainContentRegion];
            if (!region.Views.Contains(view))
            {
                region.Add(view);
            }

            region.Activate(view);
        }
    }
}
