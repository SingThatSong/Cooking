﻿using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.Services;
using MahApps.Metro;
using Microsoft.Extensions.Options;
using PropertyChanged;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// View model for settings.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
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

            AppThemes = ThemeManager.Themes
                                    .GroupBy(x => x.BaseColorScheme)
                                    .Select(x => x.First())
                                    .ToList();

            ColorThemes = ThemeManager.ColorSchemes.OrderBy(x => x.Name).ToList();

            Theme currentTheme = ThemeManager.DetectTheme();

            SelectedAppTheme = AppThemes.First(x => x.BaseColorScheme == currentTheme.BaseColorScheme);
            SelectedColor = ColorThemes.First(x => x.Name == currentTheme.ColorScheme);
        }

        /// <summary>
        /// Gets all available app themes.
        /// </summary>
        public List<Theme> AppThemes { get; }

        /// <summary>
        /// Gets or sets selected app theme.
        /// </summary>
        public Theme SelectedAppTheme { get; set; }

        /// <summary>
        /// Gets all available app themes.
        /// </summary>
        public List<ColorScheme> ColorThemes { get; }

        /// <summary>
        /// Gets or sets selected app theme.
        /// </summary>
        public ColorScheme SelectedColor { get; set; }

        /// <summary>
        /// Gets command to be fired when culture combobox's selected language changes.
        /// </summary>
        public DelegateCommand CultureSelectionChangedCommand { get; }

        /// <summary>
        /// PropertyChanged-injected callbach callde when SelectedAppTheme is changed.
        /// </summary>
        public void OnSelectedAppThemeChanged()
        {
            ThemeManager.ChangeThemeBaseColor(Application.Current, SelectedAppTheme.BaseColorScheme);

            options.Value.Theme = SelectedAppTheme.BaseColorScheme;
            settingsService.UpdateAppSettings(options.Value);
        }

        /// <summary>
        /// PropertyChanged-injected callbach callde when SelectedColor is changed.
        /// </summary>
        public void OnSelectedColorChanged()
        {
            ThemeManager.ChangeThemeColorScheme(Application.Current, SelectedColor.Name);

            options.Value.Accent = SelectedColor.Name;
            settingsService.UpdateAppSettings(options.Value);
        }

        private void ChangeCulture()
        {
            options.Value.Culture = localization.CurrentCulture.Name;
            settingsService.UpdateAppSettings(options.Value);

            // If we cache views, there is no way to update culture in it
            // We guess that lang change is too rare to give up caching, so we restart whole app
            string? exeFile = Process.GetCurrentProcess().MainModule?.FileName;

            if (exeFile != null)
            {
                logger.Information($"Restarting process. Exe filename: {exeFile}. New localization: {localization.CurrentCulture.Name}");
                Process.Start(exeFile);
                Application.Current.Shutdown();
            }
        }
    }
}
