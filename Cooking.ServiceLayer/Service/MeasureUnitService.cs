using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model;
using Cooking.ServiceLayer;

namespace ServiceLayer
{
    /// <summary>
    /// Service for working with measurement units.
    /// </summary>
    public class MeasureUnitService : CRUDService<MeasureUnit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureUnitService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="CookingContext"/> instances.</param>
        /// <param name="cultureProvider">Culture provider for determining which culture enities should belong to.</param>
        public MeasureUnitService(IContextFactory contextFactory, ICurrentCultureProvider cultureProvider, IMapper mapper)
            : base(contextFactory, cultureProvider, mapper)
        {
        }
    }
}
