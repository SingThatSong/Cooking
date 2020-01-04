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
        public virtual List<T> GetAll()
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            return context.Set<T>().ToList();
        }

        public virtual TProjection GetProjected<TProjection>(Guid id) where TProjection : Entity => GetProjected<TProjection>(id, MapperService.Mapper);

        public virtual TProjection GetProjected<TProjection>(Guid id, IMapper mapper) where TProjection : Entity
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            return mapper.ProjectTo<TProjection>(context.Set<T>()).FirstOrDefault(x => x.ID == id);
        }

        public List<TProjection> GetProjected<TProjection>() => GetProjected<TProjection>(MapperService.Mapper);

        public List<TProjection> GetProjected<TProjection>(IMapper mapper)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            return mapper.ProjectTo<TProjection>(context.Set<T>()).ToList();
        }

        public async Task<Guid> CreateAsync(T entity)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            entity.ID = Guid.NewGuid();
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return entity.ID;
        }

        public async Task DeleteAsync(Guid id)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            context.Set<T>().Remove(new T { ID = id });
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(T entity)
        {
            using var context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true);
            var existing = await context.Set<T>().FindAsync(entity.ID);
            MapperService.Mapper.Map(entity, existing);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
