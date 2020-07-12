using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model;
using Microsoft.EntityFrameworkCore;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Cooking.ServiceLayer
{
    /// <summary>
    /// Service for work with recipies.
    /// </summary>
    public class RecipeService : CRUDService<Recipe>
    {
        private readonly DayService dayService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="CookingContext"/> instances.</param>
        /// <param name="cultureProvider">Culture provider for determining which culture enities should belong to.</param>
        /// <param name="dayService"><see cref="DayService"/> dependency.</param>
        public RecipeService(IContextFactory contextFactory, ICurrentCultureProvider cultureProvider, IMapper mapper, DayService dayService)
            : base(contextFactory, cultureProvider, mapper)
        {
            this.dayService = dayService;
        }

        /// <summary>
        /// Get count of days since last recipe preparation.
        /// </summary>
        /// <param name="recipeId">ID of the recipe for which count needed.</param>
        /// <returns>Count of days that passed from last time selected recipe was cooked. E.g. if it was cooked yesterday, returns 1.</returns>
        public int DaysFromLasCook(Guid recipeId)
        {
            DateTime? date = dayService.GetLastCookedDate(recipeId);

            return date != null ? (int)(DateTime.Now - date.Value).TotalDays : int.MaxValue;
        }

        /// <summary>
        /// Load the whole entity and all of it's dependencies, then map it to needed dto.
        /// Allows to use custom mappings using IoC containers and <see cref="IMappingAction{TSource, TDestination}" />.
        /// </summary>
        /// <typeparam name="TMap">Type of dto to return.</typeparam>
        /// <param name="id">ID of entity to load and map.</param>
        /// <returns>Mapped object.</returns>
        public TMap GetMapped<TMap>(Guid id)
        {
            Recipe recipe = Get(id);
            return Mapper.Map<TMap>(recipe);
        }

        /// <summary>
        /// Load the whole list of entities and all of it's dependencies, then map it to needed dto.
        /// Allows to use custom mappings using IoC containers and <see cref="IMappingAction{TSource, TDestination}" />.
        /// </summary>
        /// <typeparam name="TMap">Type of dto to return.</typeparam>
        /// <param name="mapper">Mapper containing map definition between database entity and <see cref="TMap" />.</param>
        /// <returns>Mapped object collection.</returns>
        public List<TMap> GetAllMapped<TMap>()
        {
            List<Recipe> recipe = GetAll();
            return Mapper.Map<List<TMap>>(recipe);
        }

        /// <summary>
        /// Get reipe list filtered by optional parameters.
        /// </summary>
        /// <typeparam name="T">Type of required projection.</typeparam>
        /// <param name="requiredTags">Filter reipies by tags.</param>
        /// <param name="requiredCalorieTypes">Filter reipies by calorie types.</param>
        /// <param name="maxComplexity">Filter reipies by maximal complexity.</param>
        /// <param name="minRating">Filter reipies by minimal rating.</param>
        /// <param name="onlyNew">Filter out reipies which already was cooked.</param>
        /// <returns>List of filtered recipies.</returns>
        public List<T> GetRecipiesByParametersProjected<T>(List<Guid>? requiredTags = null,
                                                    List<CalorieType>? requiredCalorieTypes = null,
                                                    int? maxComplexity = null,
                                                    int? minRating = null,
                                                    bool? onlyNew = false)
            where T : Entity
        {
            Debug.WriteLine("RecipeService.GetRecipies");

            using CookingContext context = ContextFactory.Create();
            IQueryable<Recipe> query = GetCultureSpecificSet(context)
                               .AsNoTracking()
                               .Include(x => x.Tags)
                                   .ThenInclude(x => x.Tag)
                               .AsQueryable();

            if (requiredTags != null && requiredTags.Count > 0)
            {
                Expression<Func<Recipe, bool>> predicate = PredicateHelper.False<Recipe>();

                foreach (Guid tag in requiredTags)
                {
                    predicate = predicate.Or(x => x.Tags.Any(t => t.Tag!.ID == tag));
                }

                query = query.Where(predicate);
            }

            if (requiredCalorieTypes != null && requiredCalorieTypes.Count > 0)
            {
                Expression<Func<Recipe, bool>> predicate = PredicateHelper.False<Recipe>();

                foreach (CalorieType calorieType in requiredCalorieTypes)
                {
                    predicate = predicate.Or(p => p.CalorieType == calorieType);
                }

                query = query.Where(predicate);
            }

            if (maxComplexity.HasValue)
            {
                query = query.Where(x => x.Difficulty <= maxComplexity.Value);
            }

            if (minRating.HasValue)
            {
                query = query.Where(x => x.Rating >= minRating.Value);
            }

            var queryResult = Mapper.ProjectTo<T>(query).ToList();

            // Client filtration
            if (onlyNew.HasValue && onlyNew.Value)
            {
                queryResult = queryResult.Where(x => dayService.GetLastCookedDate(x.ID) == null).ToList();
            }

            return queryResult.OrderByDescending(x => DaysFromLasCook(x.ID)).ToList();
        }

        /// <inheritdoc/>
        protected override IQueryable<Recipe> GetFullGraph(IQueryable<Recipe> baseQuery)
        {
            return baseQuery.Include(x => x.Tags)
                              .ThenInclude(x => x.Tag)
                            .Include(x => x.Ingredients)
                              .ThenInclude(x => x.Ingredient)
                            .Include(x => x.Ingredients)
                              .ThenInclude(x => x.MeasureUnit)
                            .Include(x => x.IngredientGroups)
                              .ThenInclude(x => x.Ingredients)
                                .ThenInclude(x => x.Ingredient)
                            .Include(x => x.IngredientGroups)
                              .ThenInclude(x => x.Ingredients)
                                .ThenInclude(x => x.MeasureUnit);
        }
    }
}
