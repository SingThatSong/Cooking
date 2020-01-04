using Cooking.Data.Context;
using Data.Context;
using Microsoft.Extensions.Configuration;

namespace Cooking.WPF
{
    public class ContextFactory : IContextFactory
    {
        private readonly IConfiguration configuration;

        public ContextFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public CookingContext GetContext(bool useLazyLoading = false) => new CookingContext(configuration[Consts.DbFilenameConfigParameter], useLazyLoading);
    }
}
