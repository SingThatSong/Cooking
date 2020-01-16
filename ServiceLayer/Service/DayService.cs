using Cooking.Data.Context;
using Data.Model.Plan;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.ServiceLayer
{
    public class DayService : CRUDService<Day>
    {
        private readonly WeekService weekService;

        public DayService(IContextFactory contextFactory, WeekService weekService) : base(contextFactory)
        {
            this.weekService = weekService;
        }

        public DateTime? GetLastCookedDate(Guid recipeId)
        {
            using CookingContext context = ContextFactory.Create();
            return GetCultureSpecificSet(context).Where(x => x.DinnerID == recipeId && x.DinnerWasCooked && x.Date != null)
                                                 .OrderByDescending(x => x.Date)
                                                 .FirstOrDefault()?
                                                 .Date;
        }

        public void SetDinnerWasCooked(Guid dayId, bool wasCooked)
        {
            using var context = ContextFactory.Create();
            var dayDb = context.Days.Find(dayId);
            dayDb.DinnerWasCooked = wasCooked;
            context.SaveChanges();
        }

        public async Task SetDinner(Guid dayId, Guid dinnerId)
        {
            using var context = ContextFactory.Create();
            var dayDb = await context.Days.FindAsync(dayId);
            dayDb.DinnerID = dinnerId;
            context.SaveChanges();
        }

        public async Task CreateDinner(Guid weekId, Guid dinnerId, DayOfWeek dayOfWeek)
        {
            using var context = ContextFactory.Create(useLazyLoading: true);
            var weekDb = await context.Weeks.FindAsync(weekId);
            if (weekDb.Days == null)
            {
                weekDb.Days = new List<Day>();
            }

            weekDb.Days.Add(new Day()
            {
                DinnerID = dinnerId,
                Date = weekDb.Start.AddDays(weekService.DaysFromMonday(dayOfWeek)),
                DayOfWeek = dayOfWeek
            });

            context.SaveChanges();
        }
    }
}
