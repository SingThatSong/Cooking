using Cooking.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer
{
    /// <summary>
    /// Service to perform actions on database itself (e.g. migrations).
    /// </summary>
    public sealed class DatabaseService
    {
        private readonly IContextFactory contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for <see cref="CookingContext"/> creation.</param>
        public DatabaseService(IContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Migrate database to current version.
        /// </summary>
        public void MigrateDatabase()
        {
            using CookingContext context = contextFactory.Create();
            context.Database.Migrate();
        }
    }
}
