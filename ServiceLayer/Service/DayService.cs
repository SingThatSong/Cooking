using Cooking.Data.Context;
using Cooking.Data.Model.Plan;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="DayService"/> class.
        /// </summary>
        /// <param name="contextFactory"></param>
        /// <param name="weekService"></param>
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
            using CookingContext context = ContextFactory.Create();
            var dayDb = context.Days.Find(dayId);
            dayDb.DinnerWasCooked = wasCooked;
            context.SaveChanges();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dayId"></param>
        /// <param name="dinnerId"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SetDinner(Guid dayId, Guid dinnerId)
        {
            using CookingContext context = ContextFactory.Create();
            var dayDb = await context.Days.FindAsync(dayId);
            dayDb.DinnerID = dinnerId;
            context.SaveChanges();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="weekId"></param>
        /// <param name="dinnerId"></param>
        /// <param name="dayOfWeek"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateDinner(Guid weekId, Guid dinnerId, DayOfWeek dayOfWeek)
        {
            using CookingContext context = ContextFactory.Create(useLazyLoading: true);
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
