using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLayer
{
    /// <summary>
    /// Service for work with ingredients.
    /// </summary>
    public class IngredientService : CRUDService<Ingredient>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="CookingContext"/> instances.</param>
        /// <param name="cultureProvider">Culture provider for determining which culture enities should belong to.</param>
        /// <param name="mapper">Dependency on database-projection mapper.</param>
        public IngredientService(IContextFactory contextFactory, ICurrentCultureProvider cultureProvider, IMapper mapper)
            : base(contextFactory, cultureProvider, mapper)
        {
        }

        /// <summary>
        /// Get all ingredients' names.
        /// </summary>
        /// <returns>All ingredients' names.</returns>
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
