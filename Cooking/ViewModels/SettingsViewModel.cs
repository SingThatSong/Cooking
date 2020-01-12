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

            Process.Start(Application.ResourceAssembly.Location.Replace("dll", "exe"));
            Application.Current.Shutdown();
        });
    }
}
