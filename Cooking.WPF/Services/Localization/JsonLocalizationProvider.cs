using Cooking.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Providers;

namespace Cooking.WPF.Services
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
                foreach (FileInfo file in new DirectoryInfo(Consts.LocalizationFolder).EnumerateFiles())
                {
                    string lang = file.Name.Replace(Consts.LocalizationFilename, string.Empty, StringComparison.Ordinal)
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
        public string? this[string key] => GetLocalizedString(key);

        /// <inheritdoc/>
        public FullyQualifiedResourceKeyBase GetFullyQualifiedResourceKey(string key, DependencyObject? target) => new FQAssemblyDictionaryKey(key);

        /// <inheritdoc/>
        public object? GetLocalizedObject(string key, DependencyObject? target, CultureInfo culture)
        {
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
                InitCache(culture);
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

        /// <summary>
        /// Get all values for prefix.
        /// </summary>
        /// <param name="prefix">Prefix for a string collection.</param>
        /// <returns>Dictionary of localization keys and values.</returns>
        public Dictionary<string, string> GetAllValuesFor(string prefix)
        {
            CultureInfo culture = CurrentCulture;

            if (localizationCache.ContainsKey(culture.Name))
            {
                return localizationCache[culture.Name]
                            .Where(x => x.Key.StartsWith($"{prefix}_", StringComparison.Ordinal))
                            .ToDictionary(x =>
                            {
                                int indexAfterUnderscore = x.Key.IndexOf('_', StringComparison.Ordinal) + 1;
                                return x.Key[indexAfterUnderscore..];
                            },
                            x => x.Value);
            }
            else
            {
                InitCache(culture);
                return localizationCache[culture.Name]
                            .Where(x => x.Key.StartsWith($"{prefix}_", StringComparison.Ordinal))
                            .ToDictionary(x =>
                            {
                                int indexAfterUnderscore = x.Key.IndexOf('_', StringComparison.Ordinal) + 1;
                                return x.Key[indexAfterUnderscore..];
                            },
                            x => x.Value);
            }
        }

        private void InitCache(CultureInfo culture)
        {
            string filename = Consts.LocalizationFilename;
            if (culture.Name.Length != 0)
            {
                filename += $".{culture.Name}";
            }

            filename += ".json";
            string json = File.ReadAllText($@"{Consts.LocalizationFolder}\" + filename);

            Dictionary<string, string>? deserialized = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (deserialized != null)
            {
                localizationCache[culture.Name] = deserialized;
            }
        }
    }
}
