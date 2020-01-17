using System.Globalization;

namespace Cooking.WPF.Helpers
{
    public interface ILocalization
    {
        string? GetLocalizedString(string key);

        CultureInfo CurrentCulture { get; }
    }
}
