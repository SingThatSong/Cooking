using System.Globalization;

namespace Cooking.WPF.Helpers
{
    public interface ILocalization
    {
        CultureInfo CurrentCulture { get; }
        string? GetLocalizedString(string key);
    }
}
