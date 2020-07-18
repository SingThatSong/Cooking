using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model.Plan;
using Cooking.ServiceLayer;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLayer
{
    /// <summary>
    /// Service for working with garnishes.
    /// </summary>
    public class GarnishService : CRUDService<Garnish>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GarnishService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="CookingContext"/> instances.</param>
        /// <param name="cultureProvider">Culture provider for determining which culture enities should belong to.</param>
        /// <param name="mapper">Dependency on database-projection mapper.</param>
        public GarnishService(IContextFactory contextFactory, ICurrentCultureProvider cultureProvider, IMapper mapper)
            : base(contextFactory, cultureProvider, mapper)
        {
        }

        /// <summary>
        /// Get all garnish names.
        /// </summary>
        /// <returns>All garnish names.</returns>
        public List<string> GetNames()
        {
            using CookingContext context = ContextFactory.Create();
            return GetCultureSpecificSet(context)
                          .AsNoTracking()
                          .Where(x => x.Name != null)
                          .Select(x => x.Name!)
                          .ToList();
        }
    }
}
