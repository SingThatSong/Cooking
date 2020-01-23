using Cooking.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Cooking.WPF.Helpers
{
    public interface ILocalization : ICurrentCultureProvider
    {
        Dictionary<string, string> GetAllValuesFor(string key);
        string? GetLocalizedString(Enum key);
        string? GetLocalizedString(string key);
        string? GetLocalizedString(string key, params object?[] args);
    }
}
