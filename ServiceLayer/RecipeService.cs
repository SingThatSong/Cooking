using Cooking.ServiceLayer;
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
        private readonly static Dictionary<Guid, DateTime?> lastCookedId = new Dictionary<Guid, DateTime?>();

        public static int DaysFromLasCook(Guid recipeId)
        {
            var date = DayWhenLasWasCooked(recipeId);

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
            if (lastCookedId.ContainsKey(recipeId)) return lastCookedId[recipeId];

            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return lastCookedId[recipeId] = context.Days.Where(x => x.DinnerID == recipeId && x.DinnerWasCooked && x.Date != null).OrderByDescending(x => x.Date).FirstOrDefault()?.Date;
            }
        }

        public static List<RecipeSlim> GetRecipies() => GetRecipiesByParameters(null, null, null, null, false);

        public static List<RecipeSlim> GetRecipiesByParameters(List<Guid> requiredTags, List<CalorieType> calorieTypes, int? maxComplexity, int? minRating, bool onlyNew)
        {
            Debug.WriteLine("RecipeService.GetRecipies");

            using (var context = new CookingContext(DatabaseService.DbFileName))
            {

                var query = context.Recipies
                                   .Include(x => x.Tags)
                                       .ThenInclude(x => x.Tag)
                                   .Include(x => x.Ingredients)
                                       .ThenInclude(x => x.Ingredient)
                                   .AsQueryable();

                if (requiredTags != null && requiredTags.Count > 0)
                {
                    var predicate = PredicateBuilder.False<Recipe>();

                    foreach (var tag in requiredTags)
                    {
                        predicate = predicate.Or(x => x.Tags.Any(t => t.Tag.ID == tag));
                    }

                    query = query.Where(predicate);
                }


                if (calorieTypes != null && calorieTypes.Count > 0)
                {
                    var predicate = PredicateBuilder.False<Recipe>();

                    foreach (var calorieType in calorieTypes)
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

                var queryResult = MapperService.Mapper.ProjectTo<RecipeSlim>(query).ToList();

                // Клиентская обработка
                if (onlyNew)
                {
                    queryResult = queryResult.Where(x => DayWhenLasWasCooked(x.ID) == null).ToList();
                }

                return queryResult.OrderByDescending(x => DaysFromLasCook(x.ID)).ToList();
            }
        }

        public static async Task<TRecipe> GetRecipe<TRecipe>(Guid recipeId) where TRecipe : RecipeSlim
        {
            using (var context = new CookingContext())
            {
                return await MapperService.Mapper.ProjectTo<TRecipe>(context.Recipies)
                                                 .SingleAsync(x => x.ID == recipeId);
            }
        }

        public static async Task DeleteRecipe(Guid id)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                var recipeId = await context.Recipies.FindAsync(id);
                context.Recipies.Remove(recipeId);
                context.SaveChanges();
            }
        }

        public static async Task UpdateAsync(Recipe recipe)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true))
            {
                var existingRecipe = await context.Recipies.FindAsync(recipe.ID);
                MapperService.Mapper.Map(recipe, existingRecipe);
                MappingsHelper.MapToRecipe(existingRecipe, context);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<Guid> CreateAsync(Recipe recipe)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true))
            {
                var recipeNew = MappingsHelper.MapToRecipe(recipe, context);

                await context.Recipies.AddAsync(recipeNew);
                await context.SaveChangesAsync();
                return recipeNew.ID;
            }
        }
    }
}
