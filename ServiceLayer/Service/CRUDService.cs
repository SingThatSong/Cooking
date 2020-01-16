using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model;
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

        protected string GetCurrentCulture() => Thread.CurrentThread.CurrentUICulture.Name;

        public virtual List<T> GetAll()
        {
            using CookingContext context = ContextFactory.Create();
            return GetCultureSpecificSet(context).ToList();
        }

        public virtual TProjection GetProjected<TProjection>(Guid id) where TProjection : Entity => GetProjected<TProjection>(id, MapperService.Mapper);

        public virtual TProjection GetProjected<TProjection>(Guid id, IMapper mapper) where TProjection : Entity
        {
            using CookingContext context = ContextFactory.Create();
            IQueryable<T> cultureSpecificSet = GetCultureSpecificSet(context);
            return mapper.ProjectTo<TProjection>(cultureSpecificSet).FirstOrDefault(x => x.ID == id);
        }

        public List<TProjection> GetProjected<TProjection>() => GetProjected<TProjection>(MapperService.Mapper);

        public List<TProjection> GetProjected<TProjection>(IMapper mapper)
        {
            using CookingContext context = ContextFactory.Create();
            IQueryable<T> cultureSpecificSet = GetCultureSpecificSet(context);
            return mapper.ProjectTo<TProjection>(cultureSpecificSet).ToList();
        }

        public async Task<Guid> CreateAsync(T entity)
        {
            using CookingContext context = ContextFactory.Create();
            entity.ID = Guid.NewGuid();
            entity.Culture = GetCurrentCulture();
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return entity.ID;
        }

        public async Task DeleteAsync(Guid id)
        {
            using CookingContext context = ContextFactory.Create();
            context.Set<T>().Remove(new T { ID = id });
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(T entity)
        {
            using CookingContext context = ContextFactory.Create(useLazyLoading: true);
            T existing = await context.Set<T>().FindAsync(entity.ID);
            MapperService.Mapper.Map(entity, existing);
            existing.Culture = GetCurrentCulture();
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        protected IQueryable<T> GetCultureSpecificSet(CookingContext context)
        {
            string currentCulure = GetCurrentCulture();
            return context.Set<T>().Where(x => x.Culture == currentCulure);
        }
    }
}
