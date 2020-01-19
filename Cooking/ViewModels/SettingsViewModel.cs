using Cooking.WPF.Commands;
using System.IO;
using WPFLocalizeExtension.Engine;
using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System;
using Serilog;
using Cooking.WPF.Helpers;
using System.Reflection;
using System.Linq;

namespace Cooking.WPF.Views
{
    public class SettingsViewModel
    {
        private readonly ILogger logger;

        public SettingsViewModel(ILogger logger, ILocalization localization)
        {
            this.logger = logger;
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
