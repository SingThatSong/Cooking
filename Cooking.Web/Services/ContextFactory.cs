using Cooking.Data.Context;
using Microsoft.Extensions.Configuration;

namespace Cooking.Web
{
    /// <summary>
    /// Context factory for web.
    /// </summary>
    public class ContextFactory : IContextFactory
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFactory"/> class.
        /// </summary>
        /// <param name="configuration">Configuration provider.</param>
        // TODO: Replace with IOptions
        public ContextFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public CookingContext Create() => new(configuration["dbName"]);
    }
}
