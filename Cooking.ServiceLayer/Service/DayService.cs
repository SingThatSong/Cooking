﻿using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
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
        private static readonly Dictionary<Guid, DateTime?> LastCookedDateCache = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DayService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="CookingContext"/> instances.</param>
        /// <param name="cultureProvider">Culture provider for determining which culture enities should belong to.</param>
        /// <param name="mapper">Dependency on database-projection mapper.</param>
        public DayService(IContextFactory contextFactory, ICurrentCultureProvider cultureProvider, IMapper mapper)
            : base(contextFactory, cultureProvider, mapper)
        {
        }

        /// <summary>
        /// Load last cooked dates for all recipies to fill the cache.
        /// </summary>
        public void InitCache()
        {
            using CookingContext context = ContextFactory.Create();

            var allDaysCooked = GetCultureSpecificSet(context)
                                         .Where(x => x.DinnerWasCooked)
                                         .AsNoTracking()
                                         .ToList();

            foreach (IGrouping<Guid, Day> daysCookedForRecipe in allDaysCooked.GroupBy(x => x.DinnerID))
            {
                LastCookedDateCache[daysCookedForRecipe.Key] = daysCookedForRecipe.OrderBy(x => x.Date).Last().Date;
            }
        }

        /// <summary>
        /// Get a date when recipe was last cooked.
        /// </summary>
        /// <param name="recipeID">Id of recipe to search last cooked date.</param>
        /// <returns>Date when recipe was last (most recently) cooked or null of recipe was never cooked.</returns>
        public DateTime? GetLastCookedDate(Guid recipeID)
        {
            if (LastCookedDateCache.ContainsKey(recipeID))
            {
                return LastCookedDateCache[recipeID];
            }

            using CookingContext context = ContextFactory.Create();

            DateTime? date = GetCultureSpecificSet(context).Where(x => (x.DinnerID == recipeID || x.DinnerGarnishID == recipeID) && x.DinnerWasCooked)
                                                           .OrderByDescending(x => x.Date)
                                                           .AsNoTracking()
                                                           .FirstOrDefault()?
                                                           .Date;

            return LastCookedDateCache[recipeID] = date;
        }

        /// <summary>
        /// Toggle dinner was cooked on a given day.
        /// </summary>
        /// <param name="dayID">Id of the day of the dinner.</param>
        /// <param name="wasCooked">Indicator of whether dinner was cooked.</param>
        public void SetDinnerWasCooked(Guid dayID, bool wasCooked)
        {
            using CookingContext context = ContextFactory.Create();
            Day dayDb = context.Days.Find(dayID);
            dayDb.DinnerWasCooked = wasCooked;
            context.SaveChanges();
        }

        /// <summary>
        /// Set dinner for an existing day.
        /// </summary>
        /// <param name="dayID">ID of an existing day to which dinner should be set.</param>
        /// <param name="dinnerID">ID of a dinner to be set.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SetDinnerAsync(Guid dayID, Guid dinnerID)
        {
            using CookingContext context = ContextFactory.Create();
            Day dayDb = await context.Days.FindAsync(dayID);
            dayDb.DinnerID = dinnerID;
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

            // Get day's last second
            DateTime sundayDate = LastDayOfWeek(dayOfWeek);

            List<Day> weekDays = await GetCultureSpecificSet(context)
                                           .Include(x => x.Dinner)
                                           .Include(x => x.DinnerGarnish)
                                           .AsNoTracking()
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
        public async Task CreateWeekAsync(DateTime weekStart, Dictionary<DayOfWeek, (Guid? RecipeID, Guid? GarnishID)> selectedRecepies)
        {
            using CookingContext context = ContextFactory.Create();

            var days = new List<Day>();

            foreach (KeyValuePair<DayOfWeek, (Guid? RecipeID, Guid? GarnishID)> recipe in selectedRecepies.Where(x => x.Value.RecipeID.HasValue))
            {
                days.Add(new Day()
                {
                    DinnerID = recipe.Value!.RecipeID!.Value,
                    DinnerGarnishID = recipe.Value!.GarnishID,
                    Date = weekStart.AddDays(DaysFromMonday(recipe.Key)),
                    DayOfWeek = recipe.Key,
                    Culture = GetCurrentCulture(),
                });
            }

            await context.AddRangeAsync(days);
            await context.SaveChangesAsync();
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
        /// <param name="localization">Localization provider.</param>
        /// <returns>Shopping list for a week as a collection of ingredient groups.</returns>
        public List<ShoppingListIngredientsGroup> GetWeekShoppingList(DateTime weekStart, DateTime weekEnd, ILocalization localization)
        {
            using CookingContext context = ContextFactory.Create();

            // Includes for database querying, null-checks are not applicable.
            #pragma warning disable CS8602, CS8604
            var days = GetCultureSpecificSet(context).Include(x => x.Dinner)
                                                         .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.Ingredient)
                                                     .Include(x => x.Dinner)
                                                         .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.MeasureUnit)
                                                     .Include(x => x.Dinner)
                                                         .ThenInclude(x => x.IngredientGroups)
                                                            .ThenInclude(x => x.Ingredients)
                                                                .ThenInclude(x => x.Ingredient)
                                                     .Include(x => x.Dinner)
                                                         .ThenInclude(x => x.IngredientGroups)
                                                            .ThenInclude(x => x.Ingredients)
                                                                .ThenInclude(x => x.MeasureUnit)
                                                     .Include(x => x.DinnerGarnish)
                                                         .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.Ingredient)
                                                     .Include(x => x.DinnerGarnish)
                                                         .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.MeasureUnit)
                                                     .Include(x => x.DinnerGarnish)
                                                         .ThenInclude(x => x.IngredientGroups)
                                                            .ThenInclude(x => x.Ingredients)
                                                                .ThenInclude(x => x.Ingredient)
                                                     .Include(x => x.DinnerGarnish)
                                                         .ThenInclude(x => x.IngredientGroups)
                                                            .ThenInclude(x => x.Ingredients)
                                                                .ThenInclude(x => x.MeasureUnit)
                                                     .Where(x => weekStart.Date <= x.Date && x.Date <= weekEnd.Date)
                                                     .AsNoTracking()
                                                     .ToList();
            #pragma warning restore CS8602, CS8604

            // Create single list of all ingredients in recipies for a week
            var ingredients          = from dinner in days.Where(x => x.Dinner?.Ingredients != null)
                                       from recipeIngredient in dinner.Dinner!.Ingredients!
                                       select new { dinner.Dinner, Ingredient = recipeIngredient };

            // Include ingredients in groups
            var ingredientsInGroupds = from dinner in days.Where(x => x.Dinner?.IngredientGroups != null)
                                       from recipeIngredient in dinner.Dinner!.IngredientGroups!.SelectMany(g => g.Ingredients!)
                                       select new { dinner.Dinner, Ingredient = recipeIngredient };

            // Include ingredients in garnishes
            var garnishIngredients = from garnish in days.Where(x => x.DinnerGarnish?.Ingredients != null)
                                     from recipeIngredient in garnish.DinnerGarnish!.Ingredients!
                                     select new { Dinner = garnish.DinnerGarnish, Ingredient = recipeIngredient };

            // Include ingredients in groups
            var garnishIngredientsInGroupds = from garnish in days.Where(x => x.DinnerGarnish?.IngredientGroups != null)
                                              from recipeIngredient in garnish.DinnerGarnish!.IngredientGroups!.SelectMany(g => g.Ingredients!)
                                              select new { Dinner = garnish.DinnerGarnish, Ingredient = recipeIngredient };

            var allIngredients = ingredients.Union(ingredientsInGroupds)
                                            .Union(garnishIngredients)
                                            .Union(garnishIngredientsInGroupds);

            var ingredientsGroupedByType = allIngredients.Where(x => x.Ingredient.Ingredient?.Type != null)
                                                         .GroupBy(x => x.Ingredient.Ingredient!.Type!.Value)
                                                         .OrderBy(x => x.Key);

            var result = new List<ShoppingListIngredientsGroup>();

            foreach (var ingredientTypeGroup in ingredientsGroupedByType)
            {
                var item = new ShoppingListIngredientsGroup
                {
                    IngredientGroupName = localization.GetLocalizedString(ingredientTypeGroup.Key)
                };

                foreach (var ingredientNameGroup in ingredientTypeGroup.GroupBy(x => x.Ingredient.Ingredient!.Name).OrderBy(x => x.Key))
                {
                    var measures = ingredientNameGroup.GroupBy(x => x.Ingredient.MeasureUnit?.FullNamePluralization);
                    item.Ingredients.Add(new ShoppingListIngredient()
                    {
                        Name = ingredientNameGroup.Key,

                        IngredientAmounts = measures.Where(x => x.Key != null)
                                                    .Select(x => new ShoppingListAmount()
                                                    {
                                                        MeasurementUnitPluralization = x.Key!,
                                                        Amount = x.Where(a => a.Ingredient.Amount.HasValue)
                                                                  .Sum(a => a.Ingredient.Amount!.Value)
                                                    })
                                                    .ToList(),

                        RecipiesSources = ingredientNameGroup.Where(x => x.Dinner.Name != null)
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
        public async Task<bool> IsWeekFilledAsync(DateTime dayOfWeek)
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
        /// <param name="dinnerID">Dinner to set to the new day.</param>
        /// <param name="dayOfWeek">New day's weekday.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateDinnerAsync(DateTime dayOnWeek, Guid dinnerID, DayOfWeek dayOfWeek)
        {
            using CookingContext context = ContextFactory.Create();

            var newDay = new Day()
            {
                DinnerID = dinnerID,
                Date = FirstDayOfWeek(dayOnWeek).AddDays(DaysFromMonday(dayOfWeek)),
                DayOfWeek = dayOfWeek,
                Culture = GetCurrentCulture()
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
            return StartOfDay(date.AddDays(daysFromMonday * -1));
        }

        /// <summary>
        /// Get last day of week for a given day.
        /// </summary>
        /// <param name="date">Any day on week.</param>
        /// <returns>DateTime of sunday.</returns>
        public DateTime LastDayOfWeek(DateTime date) => EndOfDay(FirstDayOfWeek(date).AddDays(6));

        /// <summary>
        /// Move day to next week.
        /// </summary>
        /// <param name="dayID">Day to move.</param>
        /// <param name="selectedWeekday">Weekday to move day to.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task MoveDayToNextWeekAsync(Guid dayID, DayOfWeek selectedWeekday)
        {
            using CookingContext context = ContextFactory.Create();
            Day day = context.Days.First(x => x.ID == dayID);

            // Change date
            DateTime dayOnNextWeek = LastDayOfWeek(day.Date).AddDays(1);
            day.Date = dayOnNextWeek.AddDays(DaysFromMonday(selectedWeekday));

            // Change weekday
            day.DayOfWeek = selectedWeekday;

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete whole week from database.
        /// </summary>
        /// <param name="weekStart">First day of a period to which deleted days should belong.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task DeleteWeekAsync(DateTime weekStart)
        {
            DateTime weekEnd = LastDayOfWeek(weekStart);
            using CookingContext context = ContextFactory.Create();
            IEnumerable<Day> days = GetCultureSpecificSet(context)
                                        .AsNoTracking()
                                        .Where(x => weekStart <= x.Date && x.Date <= weekEnd)
                                        .Select(x => x.ID)

                                        // Load ids from database
                                        .ToList()

                                        // Create stub objects to delete
                                        .Select(id => new Day { ID = id });

            context.RemoveRange(days);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        protected override IQueryable<Day> GetFullGraph(IQueryable<Day> set)
        {
            // Includes for database querying, null-checks are not applicable.
#pragma warning disable CS8602
            return set.Include(x => x.Dinner)
                        .ThenInclude(x => x.IngredientGroups)
                          .ThenInclude(x => x.Ingredients)
                            .ThenInclude(x => x.Ingredient)
                      .Include(x => x.Dinner)
                        .ThenInclude(x => x.IngredientGroups)
                          .ThenInclude(x => x.Ingredients)
                            .ThenInclude(x => x.MeasureUnit)
                      .Include(x => x.Dinner)
                        .ThenInclude(x => x.Ingredients)
                          .ThenInclude(x => x.Ingredient)
                      .Include(x => x.Dinner)
                        .ThenInclude(x => x.Ingredients)
                          .ThenInclude(x => x.MeasureUnit)
                      .Include(x => x.Dinner)
                        .ThenInclude(x => x.Tags);
#pragma warning restore CS8602
        }

        /// <summary>
        /// Gets start of a given date.
        /// </summary>
        /// <param name="date">Date.</param>
        /// <returns>Start of a given date.</returns>
        private DateTime StartOfDay(DateTime date) => date.Date;

        /// <summary>
        /// Gets start of a given date.
        /// </summary>
        /// <param name="date">Date.</param>
        /// <returns>Start of a given date.</returns>
        private DateTime EndOfDay(DateTime date) => date.Date.AddDays(1).AddTicks(-1);
    }
}