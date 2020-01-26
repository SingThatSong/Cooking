using Cooking.WPF.Commands;
using Cooking.WPF.Services;
using Microsoft.Extensions.Options;
using Serilog;
using System.Diagnostics;
using System.Windows;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// View model for settings.
    /// </summary>
    public class SettingsViewModel
    {
        private readonly ILogger logger;
        private readonly ILocalization localization;
        private readonly SettingsService settingsService;
        private readonly IOptions<AppSettings> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        /// <param name="logger">Logger dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        /// <param name="options">Surrent settings.</param>
        /// <param name="settingsService">Settings service dependency.</param>
        public SettingsViewModel(ILogger logger,
                                 ILocalization localization,
                                 IOptions<AppSettings> options,
                                 SettingsService settingsService)
        {
            CultureSelectionChangedCommand = new DelegateCommand(ChangeCulture);
            this.logger = logger;
            this.localization = localization;
            this.options = options;
            this.settingsService = settingsService;
        }

        /// <summary>
        /// Gets command to be fired when culture combobox's selected language changes.
        /// </summary>
        public DelegateCommand CultureSelectionChangedCommand { get; }

        private void ChangeCulture()
        {
            options.Value.Culture = localization.CurrentCulture.Name;
            settingsService.UpdateAppSettings(options.Value);

            // If we cache views, there is no way to update culture in it
            // We guess that lang change is too rare to give up caching, so we restart whole app
            string exeFile = Process.GetCurrentProcess().MainModule.FileName;

            logger.Information($"Restarting process. Exe filename: {exeFile}. New localization: {localization.CurrentCulture.Name}");
            Process.Start(exeFile);
            Application.Current.Shutdown();
        }
    }
}
