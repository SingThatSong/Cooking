using WPFLocalizeExtension.Providers;

namespace Cooking.WPF.Helpers
{
    public interface ILocalization : ILocalizationProvider
    {
        string? GetLocalizedString(string key);
    }
}
