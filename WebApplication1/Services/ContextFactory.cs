using Cooking.Data.Context;
using Microsoft.Extensions.Configuration;

namespace Cooking.Web
{
    public class ContextFactory : IContextFactory
    {
        private readonly IConfiguration configuration;

        public ContextFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public CookingContext Create(bool useLazyLoading = false) => new CookingContext(configuration["dbName"], useLazyLoading);
    }
}
