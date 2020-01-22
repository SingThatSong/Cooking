using Cooking.Data.Context;
using Cooking.Data.Model.Plan;
using Cooking.ServiceLayer;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLayer
{
    public class GarnishService : CRUDService<Garnish>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GarnishService"/> class.
        /// </summary>
        /// <param name="contextFactory"></param>
        public GarnishService(IContextFactory contextFactory) : base(contextFactory)
        {

        }

        public List<string> GetSearchNames()
        {
            using CookingContext context = ContextFactory.Create();
            return GetCultureSpecificSet(context)
                          .Where(x => x.Name != null)
                          .Select(x => x.Name!)
                          .ToList();
        }
    }
}
