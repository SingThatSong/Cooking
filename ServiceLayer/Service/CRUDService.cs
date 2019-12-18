using AutoMapper;
using Data.Context;
using Data.Model;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.ServiceLayer
{
    public class CRUDService<T> where T : Entity, new()
    {
        public List<TProjection> GetProjected<TProjection>(IMapper mapper)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            return mapper.ProjectTo<TProjection>(context.Set<T>()).ToList();
        }

        public async Task<Guid> CreateAsync(T entity)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return entity.ID;
        }

        public void Delete(Guid id)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            context.Set<T>().Remove(new T { ID = id });
        }

        public async Task UpdateAsync(T garnish)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            var existing = await context.Set<T>().FindAsync(garnish.ID);
            MapperService.Mapper.Map(garnish, existing);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
