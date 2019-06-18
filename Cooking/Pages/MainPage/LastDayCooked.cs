using Data.Context;
using Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cooking.Pages.MainPage
{
    internal class LastDayCooked
    {
        private readonly static Dictionary<Guid, DateTime?> cache = new Dictionary<Guid, DateTime?>();

        public static int DaysFromLasCook(Recipe recipe) => DaysFromLasCook(recipe.ID);

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

        public static DateTime? DayWhenLasWasCooked(Recipe recipe) => DayWhenLasWasCooked(recipe.ID);

        public static DateTime? DayWhenLasWasCooked(Guid recipeId)
        {
            if (cache.ContainsKey(recipeId)) return cache[recipeId];

            using (var context = new CookingContext())
            {
                return cache[recipeId] = context.Days.Where(x => x.DinnerID == recipeId && x.DinnerWasCooked && x.Date != null).OrderByDescending(x => x.Date).FirstOrDefault()?.Date;
            }
        }
    }
}
