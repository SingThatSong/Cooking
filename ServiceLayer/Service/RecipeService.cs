using AutoMapper;
using Cooking.Data.Context;
using Cooking.ServiceLayer.Projections;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cooking.ServiceLayer
{
    public class RecipeService : CRUDService<Recipe>
    {
        private static readonly Dictionary<Guid, DateTime?> lastCookedId = new Dictionary<Guid, DateTime?>();
        private readonly DayService dayService;

        public RecipeService(IContextFactory contextFactory, DayService dayService) : base(contextFactory)
        {
            this.dayService = dayService;
        }

        public int DaysFromLasCook(Guid recipeId)
        {
            DateTime? date = DayWhenLasWasCooked(recipeId);

            if (date != null)
            {
                return (int)(DateTime.Now - date.Value).TotalDays;
            }
            else
            {
                return -1;
            }
        }

        public override TProjection GetProjected<TProjection>(Guid id, IMapper mapper)
        {
            var recipe = Get(id);
            return mapper.Map<TProjection>(recipe);
        }

        public DateTime? DayWhenLasWasCooked(Guid recipeId)
        {
            if (lastCookedId.ContainsKey(recipeId))
            {
                return lastCookedId[recipeId];
            }

            return lastCookedId[recipeId] = dayService.GetLastCookedDate(recipeId);
        }

        public List<RecipeSlim> GetRecipies()
        {
            return GetRecipiesByParameters(null, null, null, null, false);
        }

        public List<RecipeSlim> GetRecipiesByParameters(List<Guid>? requiredTags, List<CalorieType>? calorieTypes, int? maxComplexity, int? minRating, bool onlyNew)
        {
            Debug.WriteLine("RecipeService.GetRecipies");

            using CookingContext context = ContextFactory.Create();
            IQueryable<Recipe> query = GetCultureSpecificSet(context)
                               .Include(x => x.Tags)
                                   .ThenInclude(x => x.Tag)
                               .Include(x => x.Ingredients)
                                   .ThenInclude(x => x.Ingredient)
                               .AsQueryable();

            if (requiredTags != null && requiredTags.Count > 0)
            {
                System.Linq.Expressions.Expression<Func<Recipe, bool>> predicate = PredicateBuilder.False<Recipe>();

                foreach (Guid tag in requiredTags)
                {
#pragma warning disable CS8602
                    predicate = predicate.Or(x => x.Tags.Any(t => t.Tag.ID == tag));
#pragma warning restore CS8602
                }

                query = query.Where(predicate);
            }

            if (calorieTypes != null && calorieTypes.Count > 0)
            {
                System.Linq.Expressions.Expression<Func<Recipe, bool>> predicate = PredicateBuilder.False<Recipe>();

                foreach (CalorieType calorieType in calorieTypes)
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

            List<RecipeSlim> queryResult = MapperService.Mapper.ProjectTo<RecipeSlim>(query).ToList();

            // Клиентская обработка
            if (onlyNew)
            {
                queryResult = queryResult.Where(x => DayWhenLasWasCooked(x.ID) == null).ToList();
            }

            return queryResult.OrderByDescending(x => DaysFromLasCook(x.ID)).ToList();
        }

        public Recipe Get(Guid recipeId)
        {
            using CookingContext context = ContextFactory.Create();
            return GetCultureSpecificSet(context)
                          .Include(x => x.Tags)
                             .ThenInclude(x => x.Tag)
                          .Include(x => x.Ingredients)
                             .ThenInclude(x => x.Ingredient)
                          .Include(x => x.IngredientGroups)
                             .ThenInclude(x => x.Ingredients)
                                .ThenInclude(x => x.Ingredient)
                          .Single(x => x.ID == recipeId);
        }
    }
}
