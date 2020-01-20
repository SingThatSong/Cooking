using System;
using System.Globalization;

namespace Cooking.WPF.Helpers
{
    public interface ILocalization
    {
        CultureInfo CurrentCulture { get; }
        string? GetLocalizedString(Enum key);
        string? GetLocalizedString(string key);
        string? GetLocalizedString(string key, params object?[] args);
    }
}
