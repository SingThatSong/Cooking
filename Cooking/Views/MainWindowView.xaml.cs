using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Regions;

namespace Cooking
{
    /// <summary>
    /// Logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindowView
    {
        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowView"/> class.
        /// </summary>
        /// <param name="regionManager"></param>
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
