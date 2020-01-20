using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cooking.WPF.Helpers
{
    public interface ILocalization
    {
        CultureInfo CurrentCulture { get; }
        Dictionary<string, string> GetAllValuesFor(string key);
        string? GetLocalizedString(Enum key);
        string? GetLocalizedString(string key);
        string? GetLocalizedString(string key, params object?[] args);
    }
}
