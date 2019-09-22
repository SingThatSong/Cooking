using Cooking.ServiceLayer;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public static class IngredientService
    {
        public static List<T> GetIngredients<T>() where T : IngredientData
        {
            if (typeof(T).Assembly != Assembly.GetExecutingAssembly())
            {
                throw new InvalidOperationException();
            }

            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return MapperService.Mapper.ProjectTo<T>(context.Ingredients).ToList();
            }
        }

        public static async Task<Guid> CreateGarnishAsync(Ingredient garnish)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                await context.Ingredients.AddAsync(garnish);
                await context.SaveChangesAsync();
                return garnish.ID;
            }
        }

        public static async Task DeleteAsync(Guid id)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                var entity = await context.Ingredients.FindAsync(id);
                context.Ingredients.Remove(entity);
            }
        }

        public static async Task UpdateIngredientAsync(Ingredient ingredient)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                var existing = await context.Ingredients.FindAsync(ingredient.ID);
                MapperService.Mapper.Map(ingredient, existing);
                await context.SaveChangesAsync();
            }
        }

        public static List<string> GetSearchNames()
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return context.Ingredients.Select(x => x.Name).ToList();
            }
        }
    }
}
