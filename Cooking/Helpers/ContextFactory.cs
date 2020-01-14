using Cooking.Data.Context;
using Cooking.WPF.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Cooking.WPF
{
    public class ContextFactory : IContextFactory
    {
        private readonly IOptions<AppSettings> appSettings;

        public ContextFactory(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings;
        }

        public CookingContext Create(bool useLazyLoading = false) => new CookingContext(appSettings.Value.DbName, useLazyLoading);
    }
}
