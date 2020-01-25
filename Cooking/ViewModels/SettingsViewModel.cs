using Cooking.WPF.Commands;
using Cooking.WPF.Services;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Cooking.WPF.Views
{
    public class SettingsViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="localization">Localization provider dependency.</param>
        public SettingsViewModel(ILogger logger, ILocalization localization)
        {
            ChangedCommand = new DelegateCommand(() =>
            {
                string currentConfig = File.ReadAllText(Consts.SettingsFilename);
                Dictionary<string, string> configParsed = JsonSerializer.Deserialize<Dictionary<string, string>>(currentConfig);

                if (configParsed.ContainsKey(Consts.LanguageConfigParameter))
                {
                    configParsed[Consts.LanguageConfigParameter] = localization.CurrentCulture.Name;
                }
                else
                {
                    configParsed.Add(Consts.LanguageConfigParameter, localization.CurrentCulture.Name);
                }

                File.WriteAllText(Consts.SettingsFilename, JsonSerializer.Serialize(configParsed));

                // If we cache views, there is no way to update culture in it
                // We guess that lang change is too rare to give up caching, so we restart whole app
                string exeFile = Process.GetCurrentProcess().MainModule.FileName;

                logger.Information($"Restarting process. Exe filename: {exeFile}. New localization: {localization.CurrentCulture.Name}");
                Process.Start(exeFile);
                Application.Current.Shutdown();
            });
        }

        public DelegateCommand ChangedCommand { get; }
    }
}
