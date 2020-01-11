using Cooking.Data.Context;
using Cooking.ServiceLayer;
using Data.Context;
using Data.Model.Plan;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLayer
{
    public class GarnishService : CRUDService<Garnish>
    {
        public GarnishService(IContextFactory contextFactory) : base(contextFactory)
        {

        }

        public List<string> GetSearchNames()
        {
            using var context = ContextFactory.Create();
            return context.Garnishes
                          .Where(x => x.Name != null)
                          .Select(x => x.Name!)
                          .ToList();
        }
    }
}
