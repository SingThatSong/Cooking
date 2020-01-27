using Cooking.Data.Context;
using Cooking.Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="DayService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="CookingContext"/> instances.</param>
        /// <param name="cultureProvider">Culture provider for determining which culture enities should belong to.</param>
        public DayService(IContextFactory contextFactory, ICurrentCultureProvider cultureProvider)
            : base(contextFactory, cultureProvider)
        {
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
        /// Load week by date.
        /// </summary>
        /// <param name="dayOfWeek">Day of week that belongs to required week.</param>
        /// <returns>Week which contains provided day or null if no such week exists.</returns>
        public async Task<List<Day>?> GetWeekAsync(DateTime dayOfWeek)
        {
            using CookingContext context = ContextFactory.Create();
            DateTime mondayDate = FirstDayOfWeek(dayOfWeek).Date;
            DateTime sundayDate = LastDayOfWeek(dayOfWeek).Date;

            List<Day> weekDays = await GetCultureSpecificSet(context)
                                           .Include(x => x.Dinner)
                                           .Where(x => mondayDate <= x.Date && x.Date <= sundayDate)
                                           .ToListAsync();

            return weekDays.Count > 0 ? weekDays : null;
        }

        /// <summary>
        /// Create new week.
        /// </summary>
        /// <param name="weekStart">First day of week.</param>
        /// <param name="selectedRecepies">Recipies to add to the week.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateWeekAsync(DateTime weekStart, Dictionary<DayOfWeek, Guid?> selectedRecepies)
        {
            using CookingContext context = ContextFactory.Create();

            var days = new List<Day>();

            foreach (KeyValuePair<DayOfWeek, Guid?> recipe in selectedRecepies.Where(x => x.Value != null))
            {
                days.Add(new Day()
                {
                    DinnerID = recipe.Value!.Value,
                    Date = weekStart.AddDays(DaysFromMonday(recipe.Key)),
                    DayOfWeek = recipe.Key,
                    Culture = GetCurrentCulture()
                });
            }

            await context.AddRangeAsync(days);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Calculate day offset from monday (e.g. tuesday will return 1).
        /// </summary>
        /// <param name="day">Day of week to calculate distance from monday.</param>
        /// <returns>Day offset from monday.</returns>
        public int DaysFromMonday(DayOfWeek day)
        {
            // int value for sunday is 0, other weekdays ordered
            if (day == DayOfWeek.Sunday)
            {
                return 6;
            }

            return day - DayOfWeek.Monday;
        }

        /// <summary>
        /// Get shopping list for selected week.
        /// </summary>
        /// <param name="weekStart">First day of a period to fetch shopping list.</param>
        /// <param name="weekEnd">Last day of a period to fetch shopping list.</param>
        /// <returns>Shopping list for a week as a collection of ingredient groups.</returns>
        public List<ShoppingListIngredientsGroup> GetWeekShoppingList(DateTime weekStart, DateTime weekEnd)
        {
            using CookingContext context = ContextFactory.Create(useLazyLoading: true);
            var days = GetCultureSpecificSet(context).Where(x => weekStart.Date <= x.Date && x.Date <= weekEnd.Date).ToList();

            // Create single list of all ingredients in recipies for a week
            var ingredients = from dinner in days.Where(x => x.Dinner?.Ingredients != null)
                              from recipeIngredient in dinner.Dinner!.Ingredients
                              select new { dinner.Dinner, Ingredient = recipeIngredient };

            // Include ingredients in groups
            var ingredientsInGroupds = from dinner in days.Where(x => x.Dinner?.IngredientGroups != null)
                                       from recipeIngredient in dinner.Dinner!.IngredientGroups.SelectMany(g => g.Ingredients)
                                       select new { dinner.Dinner, Ingredient = recipeIngredient };

            var allIngredients = ingredients.Union(ingredientsInGroupds);

            var ingredientGroups = allIngredients.Where(x => x.Ingredient.Ingredient != null)
                                                 .GroupBy(x => x.Ingredient.Ingredient!.Type?.Name)
                                                 .OrderBy(x => x.Key);

            var result = new List<ShoppingListIngredientsGroup>();

            foreach (var ingredientGroup in ingredientGroups)
            {
                var item = new ShoppingListIngredientsGroup
                {
                    IngredientGroupName = ingredientGroup.Key
                };

                foreach (var ingredient in ingredientGroup.GroupBy(x => x.Ingredient.Ingredient!.Name))
                {
                    var measures = ingredient.GroupBy(x => x.Ingredient.MeasureUnit?.FullName);
                    item.Ingredients.Add(new ShoppingListIngredient()
                    {
                        Name = ingredient.Key,
                        IngredientAmounts = measures.Where(x => x.Key != null)
                                                    .Select(x => new ShoppingListAmount()
                                                    {
                                                        MeasurementUnit = x.Key!,
                                                        Amount = x.Where(a => a.Ingredient.Amount.HasValue).Sum(a => a.Ingredient.Amount!.Value)
                                                    }).ToList(),
                        RecipiesSources = ingredient.Where(x => x.Dinner.Name != null)
                                                    .Select(x => x.Dinner.Name!)
                                                    .Distinct()
                                                    .ToList()
                    });
                }

                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Check if all week's existing days' dinners were marked as cooked.
        /// </summary>
        /// <param name="dayOfWeek">Day of week to determine week itself.</param>
        /// <returns>True if week is filled and false if not.</returns>
        public async Task<bool> IsWeekFilled(DateTime dayOfWeek)
        {
            List<Day>? weekdays = await GetWeekAsync(dayOfWeek);

            if (weekdays == null)
            {
                return true;
            }

            return weekdays.All(x => x.DinnerWasCooked);
        }

        /// <summary>
        /// Set dinner for a new day.
        /// </summary>
        /// <param name="dayOnWeek">Day of week to identify a week.</param>
        /// <param name="dinnerId">Dinner to set to the new day.</param>
        /// <param name="dayOfWeek">New day's weekday.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateDinner(DateTime dayOnWeek, Guid dinnerId, DayOfWeek dayOfWeek)
        {
            using CookingContext context = ContextFactory.Create(useLazyLoading: true);

            var newDay = new Day()
            {
                DinnerID = dinnerId,
                Date = FirstDayOfWeek(dayOnWeek).AddDays(DaysFromMonday(dayOfWeek)),
                DayOfWeek = dayOfWeek
            };

            await context.AddAsync(newDay);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Get first day of week for a given day.
        /// </summary>
        /// <param name="date">Any day on week.</param>
        /// <returns>DateTime of monday.</returns>
        public DateTime FirstDayOfWeek(DateTime date)
        {
            int daysFromMonday = DaysFromMonday(date.DayOfWeek);
            return date.AddDays(daysFromMonday * -1);
        }

        /// <summary>
        /// Get last day of week for a given day.
        /// </summary>
        /// <param name="date">Any day on week.</param>
        /// <returns>DateTime of sunday.</returns>
        public DateTime LastDayOfWeek(DateTime date) => FirstDayOfWeek(date).AddDays(6);

        /// <summary>
        /// Move day to next week.
        /// </summary>
        /// <param name="dayId">Day to move.</param>
        /// <param name="selectedWeekday">Weekday to move day to.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task MoveDayToNextWeek(Guid dayId, DayOfWeek selectedWeekday)
        {
            using CookingContext context = ContextFactory.Create(useLazyLoading: true);
            Day day = context.Days.First(x => x.ID == dayId);

            // Change date
            DateTime dayOnNextWeek = LastDayOfWeek(day.Date).AddDays(1);
            day.Date = dayOnNextWeek.AddDays(DaysFromMonday(selectedWeekday));

            // Change weekday
            day.DayOfWeek = selectedWeekday;

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Delete whole week from database.
        /// </summary>
        /// <param name="weekStart">First day of a period to which deleted days should belong.</param>
        /// <param name="weekEnd">Last day of a period to which deleted days should belong.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task DeleteWeekAsync(DateTime weekStart, DateTime weekEnd)
        {
            using CookingContext context = ContextFactory.Create();

            IEnumerable<Day> days = GetCultureSpecificSet(context)
                                        .Where(x => weekStart.Date <= x.Date && x.Date <= weekEnd.Date)
                                        .Select(x => x.ID)

                                        // Load ids from database
                                        .ToList()

                                        // Create stub objects to delete
                                        .Select(id => new Day { ID = id });

            context.RemoveRange(days);
            await context.SaveChangesAsync();
        }
    }
}
