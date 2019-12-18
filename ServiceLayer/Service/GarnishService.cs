using Cooking.ServiceLayer;
using Data.Context;
using Data.Model.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class GarnishService : CRUDService<Garnish>
    {
        public List<string> GetSearchNames()
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            return context.Garnishes.Select(x => x.Name).ToList();
        }
    }
}
