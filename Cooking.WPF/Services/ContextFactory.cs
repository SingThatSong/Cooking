using Cooking.Data.Context;
using Cooking.WPF.Services;

namespace Cooking.WPF;

/// <summary>
/// <see cref="IContextFactory"/> implementation for WPF part.
/// </summary>
public class ContextFactory : IContextFactory
{
    private readonly AppSettings appSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextFactory"/> class.
    /// </summary>
    /// <param name="appSettings">App settings that contains instance of <see cref="AppSettings"/>.</param>
    public ContextFactory(AppSettings appSettings)
    {
        this.appSettings = appSettings;
    }

    /// <inheritdoc/>
    public CookingContext Create() => new(appSettings.DbName, appSettings.Culture);
}
