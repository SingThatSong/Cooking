using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cooking.ServiceLayer
{
    /// <summary>
    /// Basic create-retrieve-update-delete service.
    /// </summary>
    /// <typeparam name="T">Type of entity to work on.</typeparam>
    public abstract class CRUDService<T>
        where T : Entity, new()
    {
        private readonly ICurrentCultureProvider cultureProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CRUDService{T}"/> class.
        /// </summary>
        /// <param name="contextFactory">Context factory for creating <see cref="CookingContext"/>.</param>
        /// <param name="cultureProvider">Culture provider for determining which culture enities should belong to.</param>
        /// <param name="mapper">Dependency on database-projection mapper.</param>
        public CRUDService(IContextFactory contextFactory, ICurrentCultureProvider cultureProvider, IMapper mapper)
        {
            ContextFactory = contextFactory;
            this.cultureProvider = cultureProvider;
            Mapper = mapper;
        }

        /// <summary>
        /// Gets factory to create database contexts (unit of works).
        /// </summary>
        protected IContextFactory ContextFactory { get; }

        /// <summary>
        /// Gets dependency on database-projection mapper.
        /// </summary>
        protected IMapper Mapper { get; }

        /// <summary>
        /// Get all entities for a type.
        /// </summary>
        /// <returns>All entities for type <see cref="T"/>.</returns>
        public List<T> GetAll()
        {
            using CookingContext context = ContextFactory.Create();
            IQueryable<T> cultureSet = GetCultureSpecificSet(context);
            IQueryable<T>? fullSet = GetFullGraph(cultureSet);

            return fullSet.AsNoTracking()
                          .AsSplitQuery()
                          .ToList();
        }

        /// <summary>
        /// Get entity with whole graph.
        /// </summary>
        /// <param name="id">ID of entity to load.</param>
        /// <returns>Entity with whole graph.</returns>
        public T Get(Guid id)
        {
            using CookingContext context = ContextFactory.Create();
            return Get(id, context);
        }

        /// <summary>
        /// Get all entries filtered by expression and projected to some type.
        /// </summary>
        /// <typeparam name="TProjection">Type of projection.</typeparam>
        /// <param name="predicate">Predicate to filter.</param>
        /// <returns>Projected and filtered collection.</returns>
        public virtual List<TProjection> GetProjected<TProjection>(Expression<Func<T, bool>> predicate)
            where TProjection : Entity
        {
            using CookingContext context = ContextFactory.Create();
            IQueryable cultureSpecificSet = GetCultureSpecificSet(context).Where(predicate);
            return Mapper.ProjectTo<TProjection>(cultureSpecificSet)
                         .AsNoTracking()
                         .ToList();
        }

        /// <summary>
        /// Get all entries filtered by expression and projected to some type.
        /// Filtration occurs on client side.
        /// </summary>
        /// <typeparam name="TProjection">Type of projection.</typeparam>
        /// <param name="predicate">Predicate to filter.</param>
        /// <returns>Projected and filtered collection.</returns>
        public virtual List<TProjection> GetProjectedClientside<TProjection>(Expression<Func<T, bool>> predicate)
            where TProjection : Entity
        {
            using CookingContext context = ContextFactory.Create();

            IQueryable<T> cultureSet = GetCultureSpecificSet(context);
            IQueryable<T>? fullSet = GetFullGraph(cultureSet);

            IEnumerable<T>? set = fullSet.AsNoTracking()
                                         .AsEnumerable()
                                         .Where(predicate.Compile())
                                         .ToList();

            return Mapper.Map<List<TProjection>>(set);
        }

        /// <summary>
        /// Get entry, projected from <see cref="T" />.
        /// </summary>
        /// <typeparam name="TProjection">Type of entry to project to.</typeparam>
        /// <param name="id">ID of entity to find and project.</param>
        /// <returns>Found projected entity.</returns>
        public virtual TProjection GetProjected<TProjection>(Guid id)
            where TProjection : Entity
        {
            using CookingContext context = ContextFactory.Create();
            IQueryable<T> cultureSpecificSet = GetCultureSpecificSet(context);
            return Mapper.ProjectTo<TProjection>(cultureSpecificSet)
                         .AsNoTracking()
                         .FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// Get all entities of type <see cref="T" /> projected to TProjection.
        /// </summary>
        /// <typeparam name="TProjection">Type to project <see cref="T"/> to.</typeparam>
        /// <param name="mapper">Mapper that defines projection.</param>
        /// <returns>List of all projected entities.</returns>
        public List<TProjection> GetAllProjected<TProjection>()
        {
            using CookingContext context = ContextFactory.Create();
            IQueryable<T> cultureSpecificSet = GetCultureSpecificSet(context).AsNoTracking();
            return Mapper.ProjectTo<TProjection>(cultureSpecificSet).ToList();
        }

        /// <summary>
        /// Create new entity in database.
        /// </summary>
        /// <param name="entity">Entity to insert into database.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<Guid> CreateAsync(T entity)
        {
            using CookingContext context = ContextFactory.Create();
            entity.Culture = GetCurrentCulture();
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return entity.ID;
        }

        /// <summary>
        /// Create new entity in database.
        /// </summary>
        /// <typeparam name="TProjection">Projection which should be a base for entity. Entity will be created via mapping.</typeparam>
        /// <param name="entity">Entity to insert into database.</param>
        /// <returns>ID of created entity.</returns>
        public async Task<Guid> CreateAsync<TProjection>(TProjection entity)
            where TProjection : Entity
        {
            using CookingContext context = ContextFactory.Create();
            entity.Culture = GetCurrentCulture();
            T dbType = Mapper.Map<T>(entity);
            await context.Set<T>().AddAsync(dbType);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return dbType.ID;
        }

        /// <summary>
        /// Remove entity from database.
        /// </summary>
        /// <param name="id">Id of entity to delete.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task DeleteAsync(Guid id)
        {
            using CookingContext context = ContextFactory.Create();

            // Remove entity without loading it, using stub object.
            context.Set<T>().Remove(new T { ID = id });
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Update existing entity using object as a new state.
        /// </summary>
        /// <param name="entity">New state of existing entity.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdateAsync(T entity)
        {
            using CookingContext context = ContextFactory.Create();
            T existing = await GetFullGraph(context.Set<T>()).FirstAsync(x => x.ID == entity.ID);
            Mapper.Map(entity, existing);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Update existing entity using object as a new state.
        /// </summary>
        /// <typeparam name="TProjection">Projection containing part of new state for entity.</typeparam>
        /// <param name="entity">New state of existing entity.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdateAsync<TProjection>(TProjection entity)
            where TProjection : Entity
        {
            using CookingContext context = ContextFactory.Create();

            T existing = Get(entity.ID, context, isTracking: true);
            Mapper.Map(entity, existing);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get DbSet for entities filtered by current culture.
        /// </summary>
        /// <param name="context">Context that set belongs to.</param>
        /// <returns>DbSet for entities of type <see cref="T"/> filtered by current culture.</returns>
        protected IQueryable<T> GetCultureSpecificSet(CookingContext context)
        {
            string currentCulure = GetCurrentCulture();
            return context.Set<T>().Where(x => x.Culture == currentCulure);
        }

        /// <summary>
        /// Get current culture.
        /// </summary>
        /// <returns>Current culture name. E.g. "en-US".</returns>
        protected string GetCurrentCulture() => cultureProvider.CurrentCulture.Name;

        /// <summary>
        /// Get full graph for an entity.
        /// </summary>
        /// <param name="baseQuery">Base set for a graph.</param>
        /// <returns>Full graph for an entity.</returns>
        protected virtual IQueryable<T> GetFullGraph(IQueryable<T> baseQuery) => baseQuery;

        private T Get(Guid id, CookingContext context, bool isTracking = false)
        {
            IQueryable<T> cultureSet = GetCultureSpecificSet(context);
            IQueryable<T> fullSet = GetFullGraph(cultureSet);

            if (!isTracking)
            {
                fullSet = fullSet.AsNoTracking();
            }

            return fullSet.AsSplitQuery().Single(x => x.ID == id);
        }
    }
}
