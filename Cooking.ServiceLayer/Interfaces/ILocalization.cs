using Cooking.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Cooking.ServiceLayer
{
    /// <summary>
    /// Interface for providing localized text.
    /// </summary>
    public interface ILocalization : ICurrentCultureProvider
    {
        /// <summary>
        /// Get localized string for a key.
        /// Shortcut for <see cref="GetLocalizedString(string)"/>.
        /// </summary>
        /// <param name="key">Key to get a string.</param>
        /// <returns>Localized string.</returns>
        string? this[string key] { get; }

        /// <summary>
        /// Get all values for prefix. Usually prefix is enum type.
        /// </summary>
        /// <param name="prefix">Key prefix.</param>
        /// <returns>All values for selected prefix.</returns>
        Dictionary<string, string> GetAllValuesFor(string prefix);

        /// <summary>
        /// Get localized value for enum.
        /// </summary>
        /// <param name="value">Enum value for localization.</param>
        /// <returns>Localized string.</returns>
        string? GetLocalizedString(Enum value);

        /// <summary>
        /// Get localized string for a key.
        /// </summary>
        /// <param name="key">Key to get a string.</param>
        /// <returns>Localized string.</returns>
        string? GetLocalizedString(string key);

        /// <summary>
        /// Get formatted and localized string.
        /// </summary>
        /// <param name="key">Key for a localized string. Will be used as format in string.Format.</param>
        /// <param name="args">Arguments for string.Format. Used as provided.</param>
        /// <returns>Localized string.</returns>
        string? GetLocalizedString(string key, params object?[] args);
    }
}
