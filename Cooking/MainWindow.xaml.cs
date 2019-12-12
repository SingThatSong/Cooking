using MahApps.Metro.Controls;
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
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            var viewType = ((HamburgerMenuItem)e.InvokedItem).Tag as Type;
            var view = container.Resolve(viewType);

            IRegion region = regionManager.Regions["PageDataRegion"];
            if (!region.Views.Contains(view))
            {
                region.Add(view);
            }

            region.Activate(view);
        }
    }
}
