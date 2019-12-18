using Cooking.ServiceLayer;
using Data.Context;
using Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class IngredientService : CRUDService<Ingredient>
    {
        public List<string> GetSearchNames()
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            return context.Ingredients.Select(x => x.Name).ToList();
        }
    }
}
