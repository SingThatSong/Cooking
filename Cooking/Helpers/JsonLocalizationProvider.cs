using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using WPFLocalizeExtension.Providers;

namespace Cooking.WPF.Helpers
{
    public class JsonLocalizationProvider : ILocalizationProvider
    {
        public ObservableCollection<CultureInfo> AvailableCultures => new ObservableCollection<CultureInfo>()
        {
            CultureInfo.GetCultureInfo("ru-RU"),
            CultureInfo.GetCultureInfo("en-US")
        };

        public event ProviderChangedEventHandler? ProviderChanged;
        public event ProviderErrorEventHandler? ProviderError;
        public event ValueChangedEventHandler? ValueChanged;

        public FullyQualifiedResourceKeyBase GetFullyQualifiedResourceKey(string key, DependencyObject target)
        {
            return new FQAssemblyDictionaryKey(key);
        }

        // Cache for localizations: Culture - (key - value)
        private readonly Dictionary<string, Dictionary<string, string>> cache = new Dictionary<string, Dictionary<string, string>>();

        public object? GetLocalizedObject(string key, DependencyObject target, CultureInfo culture)
        {
            Debug.WriteLine("GetLocalizedObject");
            if (cache.ContainsKey(culture.Name))
            {
                if (cache[culture.Name].ContainsKey(key))
                {
                    return cache[culture.Name][key];
                }
                else
                {
                    ProviderError?.Invoke(this, new ProviderErrorEventArgs(target, key, "No such key"));
                    return null;
                }
            }
            else
            {
                var filename = "local";
                if (culture.Name.Length != 0)
                {
                    filename += $".{culture.Name}";
                }

                filename += ".json";

                if (!File.Exists(@"Localization\" + filename)) return null;

                var json = File.ReadAllText(@"Localization\" + filename);

                cache[culture.Name] = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (cache[culture.Name].ContainsKey(key))
                {
                    return cache[culture.Name][key];
                }
                else
                {
                    ProviderError?.Invoke(this, new ProviderErrorEventArgs(target, key, "No such key"));
                    return null;
                }
            }
        }
    }
}
