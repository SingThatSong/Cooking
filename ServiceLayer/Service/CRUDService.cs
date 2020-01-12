using AutoMapper;
using Cooking.Data.Context;
using Data.Model;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cooking.ServiceLayer
{
    public class CRUDService<T> where T : Entity, new()
    {
        protected IContextFactory ContextFactory { get; }

        public CRUDService(IContextFactory contextFactory)
        {
            ContextFactory = contextFactory;
        }

        public virtual List<T> GetAll()
        {
            using var context = ContextFactory.Create();
            return GetCultureSpecificSet(context).ToList();
        }

        public virtual TProjection GetProjected<TProjection>(Guid id) where TProjection : Entity => GetProjected<TProjection>(id, MapperService.Mapper);

        public virtual TProjection GetProjected<TProjection>(Guid id, IMapper mapper) where TProjection : Entity
        {
            using var context = ContextFactory.Create();
            var cultureSpecificSet = GetCultureSpecificSet(context);
            return mapper.ProjectTo<TProjection>(cultureSpecificSet).FirstOrDefault(x => x.ID == id);
        }

        public List<TProjection> GetProjected<TProjection>() => GetProjected<TProjection>(MapperService.Mapper);

        public List<TProjection> GetProjected<TProjection>(IMapper mapper)
        {
            using var context = ContextFactory.Create();
            var cultureSpecificSet = GetCultureSpecificSet(context);
            return mapper.ProjectTo<TProjection>(cultureSpecificSet).ToList();
        }

        public async Task<Guid> CreateAsync(T entity)
        {
            using var context = ContextFactory.Create();
            entity.ID = Guid.NewGuid();
            entity.Culture = Thread.CurrentThread.CurrentUICulture.Name;
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return entity.ID;
        }

        public async Task DeleteAsync(Guid id)
        {
            using var context = ContextFactory.Create();
            context.Set<T>().Remove(new T { ID = id });
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(T entity)
        {
            using var context = ContextFactory.Create(useLazyLoading: true);
            var existing = await context.Set<T>().FindAsync(entity.ID);
            MapperService.Mapper.Map(entity, existing);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        private IQueryable<T> GetCultureSpecificSet(CookingContext context)
        {
            return context.Set<T>().Where(x => x.Culture == Thread.CurrentThread.CurrentUICulture.Name);
        }
    }
}
