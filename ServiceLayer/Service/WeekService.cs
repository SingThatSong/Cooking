using Cooking.Data.Context;
using Cooking.Data.Model.Plan;
using Cooking.ServiceLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Service for work with weeks.
    /// </summary>
    public class WeekService : CRUDService<Week>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeekService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="CookingContext"/> instances.</param>
        /// <param name="cultureProvider">Culture provider for determining which culture enities should belong to.</param>
        public WeekService(IContextFactory contextFactory, ICurrentCultureProvider cultureProvider)
            : base(contextFactory, cultureProvider)
        {
        }

        /// <summary>
        /// Load week by date.
        /// </summary>
        /// <param name="dayOfWeek">Day of week that belongs to required week.</param>
        /// <returns>Week which contains provided day or null if no such week exists.</returns>
        public async Task<Week?> GetWeekAsync(DateTime dayOfWeek)
        {
            Debug.WriteLine("WeekService.GetWeek(DateTime)");
            using CookingContext context = ContextFactory.Create();
            return await GetCultureSpecificSet(context)
                                .Include(x => x.Days)
                                    .ThenInclude(x => x.Dinner)
                                .SingleOrDefaultAsync(x => x.Start.Date <= dayOfWeek.Date
                                                        && dayOfWeek.Date <= x.End.Date).ConfigureAwait(false);
        }

        /// <summary>
        /// Create new week.
        /// </summary>
        /// <param name="weekStart">First day of week.</param>
        /// <param name="selectedRecepies">Recipies to add to the week.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateWeekAsync(DateTime weekStart, Dictionary<DayOfWeek, Guid?> selectedRecepies)
        {
            Debug.WriteLine("WeekService.CreateWeekAsync");
            using CookingContext context = ContextFactory.Create();
            var newWeek = new Week()
            {
                Start = weekStart,
                End = LastDayOfWeek(weekStart),
                ID = Guid.NewGuid(),
                Culture = GetCurrentCulture()
            };

            var days = new List<Day>();

            foreach (KeyValuePair<DayOfWeek, Guid?> recipe in selectedRecepies.Where(x => x.Value != null))
            {
                days.Add(new Day()
                {
                    DinnerID = recipe.Value,
                    Date = weekStart.AddDays(DaysFromMonday(recipe.Key)),
                    DayOfWeek = recipe.Key,
                    ID = Guid.NewGuid(),
                    Culture = GetCurrentCulture()
                });
            }

            newWeek.Days = days;
            await context.AddAsync(newWeek);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get shopping list for selected week.
        /// </summary>
        /// <param name="weekID">ID of a week to create shopping list.</param>
        /// <returns>Shopping list for a week as a collection of ingredient groups.</returns>
        public List<ShoppingListIngredientsGroup> GetWeekShoppingList(Guid weekID)
        {
            Debug.WriteLine("WeekService.GetWeekIngredients");
            using CookingContext context = ContextFactory.Create(useLazyLoading: true);
            Week week = GetCultureSpecificSet(context).First(x => x.ID == weekID);

            // Create single list of all ingredients in recipies for a week
            var ingredients = from dinner in week.Days.Where(x => x.Dinner?.Ingredients != null)
                              from recipeIngredient in dinner.Dinner!.Ingredients
                              select new { dinner.Dinner, Ingredient = recipeIngredient };

            // Include ingredients in groups
            var ingredientsInGroupds = from dinner in week.Days.Where(x => x.Dinner?.IngredientGroups != null)
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
                    // TODO: Localize
                    IngredientGroupName = ingredientGroup.Key ?? "Без категории"
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
        public bool IsWeekFilled(DateTime dayOfWeek)
        {
            Debug.WriteLine("WeekService.IsWeekFilled");
            using CookingContext context = ContextFactory.Create(useLazyLoading: true);
            Week? week = GetWeekInternal(dayOfWeek, context);

            if (week?.Days == null)
            {
                return true;
            }

            return week.Days.All(x => x.DinnerWasCooked);
        }

        /// <summary>
        /// Calculate day offset from monday (e.g. tuesday will return 1).
        /// </summary>
        /// <param name="day">Day of week to calculate distance from monday.</param>
        /// <returns>Day offset from monday.</returns>
        public int DaysFromMonday(DayOfWeek day)
        {
            Debug.WriteLine("WeekService.DaysFromMonday");

            // int value for sunday is 0, other weekdays ordered
            if (day == DayOfWeek.Sunday)
            {
                return 6;
            }

            return day - DayOfWeek.Monday;
        }

        /// <summary>
        /// Get first day of week for a given day.
        /// </summary>
        /// <param name="date">Any day on week.</param>
        /// <returns>DateTime of monday.</returns>
        public DateTime FirstDayOfWeek(DateTime date)
        {
            Debug.WriteLine("WeekService.FirstDayOfWeek");
            int daysFromMonday = DaysFromMonday(date.DayOfWeek);
            return date.AddDays(daysFromMonday * -1);
        }

        /// <summary>
        /// Get last day of week for a given day.
        /// </summary>
        /// <param name="date">Any day on week.</param>
        /// <returns>DateTime of sunday.</returns>
        public DateTime LastDayOfWeek(DateTime date)
        {
            Debug.WriteLine("WeekService.LastDayOfWeek");
            return FirstDayOfWeek(date).AddDays(6);
        }

        /// <summary>
        /// Move day to next week.
        /// </summary>
        /// <param name="currentWeekId">Week to remove the day from.</param>
        /// <param name="dayId">Day to move.</param>
        /// <param name="selectedWeekday">Weekday to move day to.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task MoveDayToNextWeek(Guid currentWeekId, Guid dayId, DayOfWeek selectedWeekday)
        {
            Debug.WriteLine("WeekService.MoveDayToNextWeek");
            using CookingContext context = ContextFactory.Create(useLazyLoading: true);
            Day day = context.Days.First(x => x.ID == dayId);

            // Удаление дня на этой неделе
            Week week = context.Weeks.First(x => x.ID == currentWeekId);
            week.Days!.Remove(day);

            // Перенос дня на неделю вперёд
            DateTime dayOnNextWeek = week.End.AddDays(1);
            Week? nextWeek = GetWeekInternal(dayOnNextWeek, context);

            if (nextWeek == null)
            {
                nextWeek = CreateWeekInternal(dayOnNextWeek, context);
                nextWeek.Days = new List<Day>();
            }
            else
            {
                nextWeek.Days ??= new List<Day>();
                nextWeek.Days.RemoveAll(x => x.DayOfWeek == selectedWeekday);
            }

            day.Date = nextWeek.Start.AddDays(DaysFromMonday(selectedWeekday));
            day.DayOfWeek = selectedWeekday;

            nextWeek.Days.Add(day);
            day.WeekID = nextWeek.ID;

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        private Week? GetWeekInternal(DateTime dayOnWeek, CookingContext context)
        {
            Debug.WriteLine("WeekService.GetWeekInternal");
            return GetCultureSpecificSet(context).SingleOrDefault(x => x.Start.Date <= dayOnWeek.Date && dayOnWeek.Date <= x.End.Date);
        }

        private Week CreateWeekInternal(DateTime dayOnWeek, CookingContext context)
        {
            Debug.WriteLine("WeekService.CreateWeekInternal");
            var week = new Week()
            {
                Start = FirstDayOfWeek(dayOnWeek),
                End = LastDayOfWeek(dayOnWeek),
                Culture = GetCurrentCulture()
            };
            context.Add(week);

            return week;
        }
    }
}
