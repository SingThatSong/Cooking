using WPFLocalizeExtension.Providers;

namespace Cooking.WPF.Helpers
{
    public interface ILocalization
    {
        string? GetLocalizedString(string key);
    }
}
