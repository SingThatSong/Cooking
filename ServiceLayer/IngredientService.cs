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
    public static class IngredientService
    {
        public static List<T> GetIngredients<T>() where T : IngredientData
        {
            if (typeof(T).Assembly != Assembly.GetExecutingAssembly())
            {
                throw new InvalidOperationException();
            }

            using var context = new CookingContext(DatabaseService.DbFileName);
            return MapperService.Mapper.ProjectTo<T>(context.Ingredients).ToList();
        }

        public static async Task<Guid> CreateAsync(Ingredient item)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            await context.Ingredients.AddAsync(item);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return item.ID;
        }

        public static async Task DeleteAsync(Guid id)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            context.Ingredients.Remove(new Ingredient() { ID = id });
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public static async Task UpdateIngredientAsync(Ingredient ingredient)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            var existing = await context.Ingredients.FindAsync(ingredient.ID);
            MapperService.Mapper.Map(ingredient, existing);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public static List<string> GetSearchNames()
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            return context.Ingredients.Select(x => x.Name).ToList();
        }
    }
}
