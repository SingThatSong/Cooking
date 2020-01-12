using Cooking.Commands;
using System.IO;
using WPFLocalizeExtension.Engine;
using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Cooking.Pages
{
    public class SettingsViewModel
    {
        public DelegateCommand ChangedCommand { get; } = new DelegateCommand(() =>
        {
            var currentConfig = File.ReadAllText(Consts.SettingsFilename);
            var configParsed = JsonSerializer.Deserialize<Dictionary<string, string>>(currentConfig);

            if (configParsed.ContainsKey(Consts.LanguageConfigParameter))
            {
                configParsed[Consts.LanguageConfigParameter] = LocalizeDictionary.Instance.Culture.Name;
            }
            else
            {
                configParsed.Add(Consts.LanguageConfigParameter, LocalizeDictionary.Instance.Culture.Name);
            }

            File.WriteAllText(Consts.SettingsFilename, JsonSerializer.Serialize(configParsed));

            // If we cache views, there is no way to update culture in it
            // We guess that lang change is too rare to give up caching, so we restart whole app
            Process.Start(Application.ResourceAssembly.Location.Replace("dll", "exe"));
            Application.Current.Shutdown();
        });
    }
}
