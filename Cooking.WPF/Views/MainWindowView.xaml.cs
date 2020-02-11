using Cooking.WPF.Controls;
using Cooking.WPF.Services;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Options;
using Prism.Regions;
using System.ComponentModel;
using System.Windows;

namespace Cooking
{
    /// <summary>
    /// Logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindowView
    {
        private readonly IRegionManager regionManager;
        private readonly IOptions<AppSettings> appSettings;
        private readonly SettingsService settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowView"/> class.
        /// </summary>
        /// <param name="regionManager">Region manager for Prism navigation.</param>
        /// <param name="appSettings">Current application settings.</param>
        /// <param name="settingsService">Service for settings update.</param>
        public MainWindowView(IRegionManager regionManager,
                              IOptions<AppSettings> appSettings,
                              SettingsService settingsService)
        {
            InitializeComponent();
            this.regionManager = regionManager;
            this.appSettings = appSettings;
            this.settingsService = settingsService;

            DialogParticipation.SetRegister(this, DataContext);
        }

        /// <summary>
        /// Save current window dimensions on exit.
        /// </summary>
        /// <param name="e">Cancelable event patten argument.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            appSettings.Value.IsWindowMaximized = WindowState == WindowState.Maximized;

            if (!appSettings.Value.IsWindowMaximized)
            {
                appSettings.Value.WindowWidth = ActualWidth;
                appSettings.Value.WindowHeight = ActualHeight;
            }

            settingsService.UpdateAppSettings(appSettings.Value);
            base.OnClosing(e);
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.InvokedItem is HamburgerMenuItem hamburgerMenuItem
             && hamburgerMenuItem.Tag is string viewName)
            {
                if (e.InvokedItem is TagHamburgerMenuItem tagMenuItem)
                {
                    regionManager.NavigateMain(viewName, (Consts.TagNameParameter, tagMenuItem.Label));
                }
                else
                {
                    regionManager.NavigateMain(viewName);
                }
            }
        }
    }
}
