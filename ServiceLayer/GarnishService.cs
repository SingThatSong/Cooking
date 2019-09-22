using Cooking.ServiceLayer;
using Data.Context;
using Data.Model.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public static class GarnishService
    {
        public static List<T> GetGarnishes<T>() where T : GarnishDTO
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return MapperService.Mapper.ProjectTo<T>(context.Garnishes).ToList();
            }
        }

        public static async Task<Guid> CreateGarnishAsync(Garnish garnish)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                await context.Garnishes.AddAsync(garnish);
                await context.SaveChangesAsync();
                return garnish.ID;
            }
        }

        public static async Task DeleteGarnishAsync(Guid id)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                var entity = await context.Garnishes.FindAsync(id);
                context.Garnishes.Remove(entity);
            }
        }

        public static async Task UpdateGarnishAsync(Garnish garnish)
        {
            using (var context = new CookingContext())
            {
                var existing = await context.Garnishes.FindAsync(garnish.ID);
                MapperService.Mapper.Map(garnish, existing);
                await context.SaveChangesAsync();
            }
        }

        public static List<string> GetSearchNames()
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return context.Garnishes.Select(x => x.Name).ToList();
            }
        }
    }
}
