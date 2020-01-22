using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model;
using Microsoft.EntityFrameworkCore;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cooking.ServiceLayer
{
    /// <summary>
    /// Basic create-retrieve-update-delete service.
    /// </summary>
    /// <typeparam name="T">Type of entity to work on.</typeparam>
    public class CRUDService<T>
        where T : Entity, new()
    {
        /// <summary>
        /// Factory to create database contexts (unit of works).
        /// </summary>
        protected IContextFactory ContextFactory { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CRUDService{T}"/> class.
        /// </summary>
        /// <param name="contextFactory"></param>
        public CRUDService(IContextFactory contextFactory)
        {
            ContextFactory = contextFactory;
        }

        protected string GetCurrentCulture() => Thread.CurrentThread.CurrentUICulture.Name;

        public virtual List<T> GetAll()
        {
            using CookingContext context = ContextFactory.Create();
            return GetCultureSpecificSet(context).AsNoTracking().ToList();
        }

        public virtual TProjection GetProjected<TProjection>(Guid id, IMapper mapper) where TProjection : Entity
        {
            using CookingContext context = ContextFactory.Create();
            IQueryable<T> cultureSpecificSet = GetCultureSpecificSet(context);
            return mapper.ProjectTo<TProjection>(cultureSpecificSet).AsNoTracking().FirstOrDefault(x => x.ID == id);
        }

        public List<TProjection> GetProjected<TProjection>(IMapper mapper)
        {
            using CookingContext context = ContextFactory.Create();
            IQueryable<T> cultureSpecificSet = GetCultureSpecificSet(context).AsNoTracking();
            return mapper.ProjectTo<TProjection>(cultureSpecificSet).ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<Guid> CreateAsync(T entity)
        {
            using CookingContext context = ContextFactory.Create();
            entity.ID = Guid.NewGuid();
            entity.Culture = GetCurrentCulture();
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return entity.ID;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task DeleteAsync(Guid id)
        {
            using CookingContext context = ContextFactory.Create();
            context.Set<T>().Remove(new T { ID = id });
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
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
