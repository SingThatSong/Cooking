using Cooking.ServiceLayer;
using Cooking.ServiceLayer.Projections;
using Data.Context;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public static class RecipeService
    {
        private static readonly Dictionary<Guid, DateTime?> lastCookedId = new Dictionary<Guid, DateTime?>();

        public static int DaysFromLasCook(Guid recipeId)
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

        public static DateTime? DayWhenLasWasCooked(Guid recipeId)
        {
            if (lastCookedId.ContainsKey(recipeId))
            {
                return lastCookedId[recipeId];
            }

            using CookingContext context = new CookingContext(DatabaseService.DbFileName);
            return lastCookedId[recipeId] = context.Days.Where(x => x.DinnerID == recipeId && x.DinnerWasCooked && x.Date != null).OrderByDescending(x => x.Date).FirstOrDefault()?.Date;
        }

        public static List<RecipeSlim> GetRecipies()
        {
            return GetRecipiesByParameters(null, null, null, null, false);
        }

        public static List<RecipeSlim> GetRecipiesByParameters(List<Guid> requiredTags, List<CalorieType> calorieTypes, int? maxComplexity, int? minRating, bool onlyNew)
        {
            Debug.WriteLine("RecipeService.GetRecipies");

            using CookingContext context = new CookingContext(DatabaseService.DbFileName);
            IQueryable<Recipe> query = context.Recipies
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
                    predicate = predicate.Or(x => x.Tags.Any(t => t.Tag.ID == tag));
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

        public static Recipe Get(Guid recipeId)
        {
            using CookingContext context = new CookingContext();
            return context.Recipies
                          .Include(x => x.Tags)
                             .ThenInclude(x => x.Tag)
                          .Include(x => x.Ingredients)
                             .ThenInclude(x => x.Ingredient)
                          .Include(x => x.IngredientGroups)
                             .ThenInclude(x => x.Ingredients)
                                .ThenInclude(x => x.Ingredient)
                          .Single(x => x.ID == recipeId);
        }

        public static TRecipe GetProjection<TRecipe>(Guid recipeId) where TRecipe : RecipeProjection
        {
            using CookingContext context = new CookingContext();

            return MapperService.Mapper.ProjectTo<TRecipe>(context.Recipies)
                                       .Single(x => x.ID == recipeId);
        }

        public static async Task Delete(Guid id)
        {
            using CookingContext context = new CookingContext(DatabaseService.DbFileName);
            context.Recipies.Remove(new Recipe { ID = id });
            await context.SaveChangesAsync();
        }

        public static async Task UpdateAsync<TRecipe>(TRecipe recipe) where TRecipe : RecipeProjection
        {
            using CookingContext context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true);
            Recipe existingRecipe = await context.Recipies.FindAsync(recipe.ID);

            MapperService.Mapper.Map(recipe, existingRecipe);
            await context.SaveChangesAsync();
        }

        public static async Task UpdateAsync(Recipe recipe)
        {
            using CookingContext context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true);
            Recipe existingRecipe = await context.Recipies.FindAsync(recipe.ID);

            MapperService.Mapper.Map(recipe, existingRecipe);
            await context.SaveChangesAsync();
        }

        public static async Task<Guid> CreateAsync(Recipe recipe)
        {
            using CookingContext context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true);
            Recipe recipeNew = MappingsHelper.MapToRecipe(recipe, context);

            await context.Recipies.AddAsync(recipeNew);
            await context.SaveChangesAsync();
            return recipeNew.ID;
        }
    }
}
