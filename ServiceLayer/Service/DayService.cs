using Cooking.Data.Context;
using Data.Context;
using Data.Model.Plan;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cooking.ServiceLayer
{
    public class DayService : CRUDService<Day>
    {
        public DayService(IContextFactory contextFactory) : base(contextFactory)
        {

        }

        public void SetDinnerWasCooked(Guid dayId, bool wasCooked)
        {
            using var context = contextFactory.GetContext();
            var dayDb = context.Days.Find(dayId);
            dayDb.DinnerWasCooked = wasCooked;
            context.SaveChanges();
        }

        public async Task SetDinner(Guid dayId, Guid dinnerId)
        {
            using var context = contextFactory.GetContext();
            var dayDb = await context.Days.FindAsync(dayId);
            dayDb.DinnerID = dinnerId;
            context.SaveChanges();
        }

        public async Task CreateDinner(Guid weekId, Guid dinnerId, DayOfWeek dayOfWeek)
        {
            using var context = contextFactory.GetContext(useLazyLoading: true);
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
    }
}
