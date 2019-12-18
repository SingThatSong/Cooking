using Data.Context;
using Data.Model.Plan;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public static class DayService
    {
        public static void SetDinnerWasCooked(Guid dayId, bool wasCooked)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            var dayDb = context.Days.Find(dayId);
            dayDb.DinnerWasCooked = wasCooked;
            context.SaveChanges();
        }

        public static async Task SetDinner(Guid dayId, Guid dinnerId)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            var dayDb = await context.Days.FindAsync(dayId);
            dayDb.DinnerID = dinnerId;
            context.SaveChanges();
        }

        public static async Task CreateDinner(Guid weekId, Guid dinnerId, DayOfWeek dayOfWeek)
        {
            using var context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true);
            var weekDb = await context.Weeks.FindAsync(weekId);
            if (weekDb.Days == null)
            {
                weekDb.Days = new List<Day>();
            }

            weekDb.Days.Add(new Day()
            {
                DinnerID = dinnerId,
                Date = weekDb.Start.AddDays(WeekService.DaysFromMonday(dayOfWeek)),
                DayOfWeek = dayOfWeek
            });

            context.SaveChanges();
        }

        public static async Task DeleteDay(Guid id)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            var dayDb = await context.Days.FindAsync(id);
            context.Days.Remove(dayDb);
            context.SaveChanges();
        }
    }
}
