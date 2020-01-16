﻿using Cooking.Data.Context;
using Cooking.ServiceLayer;
using Cooking.Data.Model.Plan;
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
            using CookingContext context = ContextFactory.Create();
            return GetCultureSpecificSet(context)
                          .Where(x => x.Name != null)
                          .Select(x => x.Name!)
                          .ToList();
        }
    }
}
