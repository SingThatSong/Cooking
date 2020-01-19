using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Providers;

namespace Cooking.WPF.Helpers
{
    public class JsonLocalizationProvider : ILocalization, ILocalizationProvider
    {
        public event ProviderChangedEventHandler? ProviderChanged;
        public event ProviderErrorEventHandler? ProviderError;
        public event ValueChangedEventHandler? ValueChanged;

        public ObservableCollection<CultureInfo> AvailableCultures
        {
            get
            {
                var result = new ObservableCollection<CultureInfo>();
                foreach (FileInfo file in new DirectoryInfo("Localization").EnumerateFiles())
                {
                    string lang = file.Name.Replace("local", string.Empty, StringComparison.Ordinal)
                                           .Replace(".", string.Empty, StringComparison.Ordinal)
                                           .Replace("json", string.Empty, StringComparison.Ordinal);
                    result.Add(CultureInfo.GetCultureInfo(lang));
                }

                return result;
            }
        }

        public CultureInfo CurrentCulture => LocalizeDictionary.Instance.Culture;

        public FullyQualifiedResourceKeyBase GetFullyQualifiedResourceKey(string key, DependencyObject target) => new FQAssemblyDictionaryKey(key);

        // Cache for localizations: Culture - (key - value)
        private readonly Dictionary<string, Dictionary<string, string>> cache = new Dictionary<string, Dictionary<string, string>>();

        public object? GetLocalizedObject(string key, DependencyObject? target, CultureInfo culture)
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
                string filename = "local";
                if (culture.Name.Length != 0)
                {
                    filename += $".{culture.Name}";
                }

                filename += ".json";

                if (!File.Exists(@"Localization\" + filename))
                {
                    return null;
                }

                string json = File.ReadAllText(@"Localization\" + filename);

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

        public string? GetLocalizedString(Enum key) => GetLocalizedObject($"{key.GetType().Name}_{Enum.GetName(key.GetType(), key)}", null, CurrentCulture) as string;
        public string? GetLocalizedString(string key) => GetLocalizedObject(key, null, CurrentCulture) as string;

        public string? GetLocalizedString(string key, params object[] args)
        {
            string? localizedString = GetLocalizedString(key);

            if (localizedString != null)
            {
                return string.Format(CurrentCulture, localizedString, args);
            }
            else
            {
                return null;
            }
        }
    }
}
