using Cooking.Data.Context;
using Cooking.Data.Model.Plan;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.ServiceLayer
{
    /// <summary>
    /// Service for work with days.
    /// </summary>
    public class DayService : CRUDService<Day>
    {
        private readonly WeekService weekService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DayService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="CookingContext"/> instances.</param>
        /// <param name="cultureProvider">Culture provider for determining which culture enities should belong to.</param>
        /// <param name="weekService"><see cref="WeekService"/> dependency.</param>
        public DayService(IContextFactory contextFactory, ICurrentCultureProvider cultureProvider, WeekService weekService)
            : base(contextFactory, cultureProvider)
        {
            this.weekService = weekService;
        }

        /// <summary>
        /// Get a date when recipe was last cooked.
        /// </summary>
        /// <param name="recipeId">Id of recipe to search last cooked date.</param>
        /// <returns>Date when recipe was last (most recently) cooked or null of recipe was never cooked.</returns>
        public DateTime? GetLastCookedDate(Guid recipeId)
        {
            using CookingContext context = ContextFactory.Create();

            // TODO: Set Date non-nullable
            return GetCultureSpecificSet(context).Where(x => x.DinnerID == recipeId && x.DinnerWasCooked && x.Date != null)
                                                 .OrderByDescending(x => x.Date)
                                                 .FirstOrDefault()?
                                                 .Date;
        }

        /// <summary>
        /// Toggle dinner was cooked on a given day.
        /// </summary>
        /// <param name="dayId">Id of the day of the dinner.</param>
        /// <param name="wasCooked">Indicator of whether dinner was cooked.</param>
        public void SetDinnerWasCooked(Guid dayId, bool wasCooked)
        {
            using CookingContext context = ContextFactory.Create();
            Day dayDb = context.Days.Find(dayId);
            dayDb.DinnerWasCooked = wasCooked;
            context.SaveChanges();
        }

        /// <summary>
        /// Set dinner for an existing day.
        /// </summary>
        /// <param name="dayId">ID of an existing day to which dinner should be set.</param>
        /// <param name="dinnerId">ID of a dinner to be set.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SetDinner(Guid dayId, Guid dinnerId)
        {
            using CookingContext context = ContextFactory.Create();
            Day dayDb = await context.Days.FindAsync(dayId);
            dayDb.DinnerID = dinnerId;
            context.SaveChanges();
        }

        /// <summary>
        /// Set dinner for a new day.
        /// </summary>
        /// <param name="weekId">Week where day should be placed.</param>
        /// <param name="dinnerId">Dinner to set to the new day.</param>
        /// <param name="dayOfWeek">New day's weekday.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateDinner(Guid weekId, Guid dinnerId, DayOfWeek dayOfWeek)
        {
            using CookingContext context = ContextFactory.Create(useLazyLoading: true);
            Week weekDb = await context.Weeks.FindAsync(weekId);
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
