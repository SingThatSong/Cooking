using Cooking.Data.Context;
using Cooking.ServiceLayer;
using Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLayer
{
    public class IngredientService : CRUDService<Ingredient>
    {
        public IngredientService(IContextFactory contextFactory) : base(contextFactory)
        {

        }

        public List<string> GetSearchNames()
        {
            using var context = ContextFactory.Create();
            return GetCultureSpecificSet(context)
                          .Where(x => x.Name != null)
                          .Select(x => x.Name!)
                          .ToList();
        }
    }
}
