using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Cooking.ServiceLayer;

/// <summary>
/// Basic create-retrieve-update-delete service.
/// </summary>
/// <typeparam name="T">Type of entity to work on.</typeparam>
public class CRUDService<T>
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
    /// <param name="predicate">Predicate to filter.</param>
    /// <returns>All entities for type <typeparamref name="T" />.</returns>
    public List<T> GetAll(Expression<Func<T, bool>>? predicate = null)
    {
        using CookingContext context = ContextFactory.Create();
        IQueryable<T>? fullSet = GetFullGraph(context.Set<T>());

        if (predicate != null)
        {
            fullSet = fullSet.Where(predicate);
        }

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
    /// Get entry, projected from <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="TProjection">Type of entry to project to.</typeparam>
    /// <param name="id">ID of entity to find and project.</param>
    /// <returns>Found projected entity.</returns>
    public TProjection GetProjected<TProjection>(Guid id)
        where TProjection : Entity
    {
        using CookingContext context = ContextFactory.Create();
        TProjection? entryProjected = Mapper.ProjectTo<TProjection>(context.Set<T>())
                                            .AsNoTracking()
                                            .FirstOrDefault(x => x.ID == id);

        return Mapper.Map<TProjection>(entryProjected);
    }

    /// <summary>
    /// Get a list of required properties for all objects matching filter.
    /// </summary>
    /// <typeparam name="TProperty">Type of property.</typeparam>
    /// <param name="propertySelector">Selector for a required property.</param>
    /// <param name="filter">Filter of objects to retrieve.</param>
    /// <returns>List of required properties for all objects matching filter.</returns>
    public List<TProperty> GetProperty<TProperty>(Expression<Func<T, TProperty>> propertySelector, Expression<Func<T, bool>>? filter = null)
    {
        using CookingContext context = ContextFactory.Create();
        IQueryable<T> cultureSpecificSet = context.Set<T>().AsNoTracking();

        if (filter != null)
        {
            cultureSpecificSet = cultureSpecificSet.Where(filter);
        }

        return cultureSpecificSet.Select(propertySelector).ToList();
    }

    /// <summary>
    /// Get all entities of type <typeparamref name="T" /> projected to TProjection.
    /// </summary>
    /// <typeparam name="TProjection">Type to project <typeparamref name="T" /> to.</typeparam>
    /// <param name="predicate">Predicate to filter.</param>
    /// <param name="callAfterMap">Do mapping after projection to call AfterMap. Needs mapping from TProjection to TProjection, otherwise will have no effect.</param>
    /// <returns>List of all projected entities.</returns>
    public List<TProjection> GetProjected<TProjection>(Expression<Func<T, bool>>? predicate = null, bool callAfterMap = false)
    {
        using CookingContext context = ContextFactory.Create();
        IQueryable<T> cultureSpecificSet = context.Set<T>().AsNoTracking();

        if (predicate != null)
        {
            cultureSpecificSet = cultureSpecificSet.Where(predicate);
        }

        var allProjected = Mapper.ProjectTo<TProjection>(cultureSpecificSet).ToList();

        if (callAfterMap)
        {
            // Here we mapping projected objects to themselves to enable AfterMap calls
            return Mapper.Map<List<TProjection>>(allProjected);
        }
        else
        {
            return allProjected;
        }
    }

    /// <summary>
    /// Get all entities of type <typeparamref name="T" /> mapped to TProjection.
    /// </summary>
    /// <typeparam name="TProjection">Type to map <typeparamref name="T" /> to.</typeparam>
    /// <param name="predicate">Filter of objects to retrieve.</param>
    /// <param name="clientsidePredicate">Predicate to filter after data loading.</param>
    /// <returns>List of all mapped entities.</returns>
    public List<TProjection> GetMapped<TProjection>(Expression<Func<T, bool>>? predicate = null, Func<T, bool>? clientsidePredicate = null)
    {
        using CookingContext context = ContextFactory.Create();
        IQueryable<T> cultureSet = context.Set<T>();
        IQueryable<T>? fullSet = GetFullGraph(cultureSet);

        IEnumerable<T>? set = fullSet.AsNoTracking();

        if (predicate != null)
        {
            fullSet = fullSet.Where(predicate);
        }

        IEnumerable<T> queryResult = fullSet.AsEnumerable();

        if (clientsidePredicate != null)
        {
            queryResult = queryResult.Where(clientsidePredicate);
        }

        return Mapper.Map<List<TProjection>>(queryResult);
    }

    /// <summary>
    /// Create new entity in database.
    /// </summary>
    /// <param name="entity">Entity to insert into database.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<Guid> CreateAsync(T entity)
    {
        await using CookingContext context = ContextFactory.Create();
        entity.Culture = GetCurrentCulture();
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        return entity.ID;
    }

    /// <summary>
    /// Create new entity in database.
    /// </summary>
    /// <typeparam name="TProjection">Projection which should be a base for entity. Entity will be created via mapping.</typeparam>
    /// <param name="entity">Entity to insert into database.</param>
    /// <returns>ID of created entity.</returns>
    public async Task<Guid> CreateAsync<TProjection>(TProjection entity)
    {
        T dbEntity = Mapper.Map<T>(entity);
        return await CreateAsync(dbEntity);
    }

    /// <summary>
    /// Remove entity from database.
    /// </summary>
    /// <param name="id">Id of entity to delete.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public virtual async Task DeleteAsync(Guid id)
    {
        using CookingContext context = ContextFactory.Create();

        // Remove entity without loading it, using stub object.
        context.Set<T>().Remove(new T { ID = id });
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Update existing entity using object as a new state.
    /// </summary>
    /// <param name="entity">New state of existing entity.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task UpdateAsync(T entity)
    {
        await using CookingContext context = ContextFactory.Create();
        T existing = await GetFullGraph(context.Set<T>()).FirstAsync(x => x.ID == entity.ID);
        Mapper.Map(entity, existing);
        await context.SaveChangesAsync();
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
        await using CookingContext context = ContextFactory.Create();

        T existing = Get(entity.ID, context, isTracking: true);
        Mapper.Map(entity, existing);
        await context.SaveChangesAsync();
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
        IQueryable<T> set = context.Set<T>();
        IQueryable<T> fullSet = GetFullGraph(set);

        if (!isTracking)
        {
            fullSet = fullSet.AsNoTracking();
        }

        return fullSet.AsSplitQuery().Single(x => x.ID == id);
    }
}
