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
    /// <summary>
    /// Json localization provider for WPFLocalizeExtension.
    /// </summary>
    public class JsonLocalizationProvider : ILocalization, ILocalizationProvider
    {
        /// <summary>
        /// Cache for localizations: Culture - (key - value).
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, string>> localizationCache = new Dictionary<string, Dictionary<string, string>>();

        /// <inheritdoc/>
        public event ProviderChangedEventHandler? ProviderChanged;

        /// <inheritdoc/>
        public event ProviderErrorEventHandler? ProviderError;

        /// <inheritdoc/>
        public event ValueChangedEventHandler? ValueChanged;

        /// <inheritdoc/>
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

        /// <summary>
        /// Gets current system culture.
        /// </summary>
        public CultureInfo CurrentCulture => LocalizeDictionary.Instance.Culture;

        /// <inheritdoc/>
        public FullyQualifiedResourceKeyBase GetFullyQualifiedResourceKey(string key, DependencyObject target) => new FQAssemblyDictionaryKey(key);

        /// <inheritdoc/>
        public object? GetLocalizedObject(string key, DependencyObject? target, CultureInfo culture)
        {
            Debug.WriteLine("GetLocalizedObject");
            if (localizationCache.ContainsKey(culture.Name))
            {
                if (localizationCache[culture.Name].ContainsKey(key))
                {
                    return localizationCache[culture.Name][key];
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

                localizationCache[culture.Name] = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (localizationCache[culture.Name].ContainsKey(key))
                {
                    return localizationCache[culture.Name][key];
                }
                else
                {
                    ProviderError?.Invoke(this, new ProviderErrorEventArgs(target, key, "No such key"));
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets localized string for enum value.
        /// </summary>
        /// <param name="key">Enum value to get localized string for.</param>
        /// <returns>Localized string.</returns>
        public string? GetLocalizedString(Enum key) => GetLocalizedObject($"{key.GetType().Name}_{Enum.GetName(key.GetType(), key)}", null, CurrentCulture) as string;

        /// <summary>
        /// Gets localized string for a key.
        /// </summary>
        /// <param name="key">Key to get localized string for.</param>
        /// <returns>Localized string.</returns>
        public string? GetLocalizedString(string key) => GetLocalizedObject(key, null, CurrentCulture) as string;

        /// <summary>
        /// Gets formatted localized string for a key.
        /// </summary>
        /// <param name="key">Key to get localized string for. Will be used as format in string.Format.</param>
        /// <param name="args">Arguments for string.Format.</param>
        /// <returns>Localized and formatted string.</returns>
        public string? GetLocalizedString(string key, params object?[] args)
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
