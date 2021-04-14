using AutoMapper;
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
    public class DayService : CRUDService<Day>, IDayService
    {
        private static readonly Dictionary<Guid, DateTime?> LastCookedDateCache = new();
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="DayService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="CookingContext"/> instances.</param>
        /// <param name="localization">Localization provider dependency.</param>
        /// <param name="mapper">Dependency on database-projection mapper.</param>
        public DayService(IContextFactory contextFactory, ILocalization localization, IMapper mapper)
            : base(contextFactory, localization, mapper)
        {
            this.localization = localization;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task SetDinnerWasCookedAsync(Guid dayID, bool wasCooked)
        {
            using CookingContext context = ContextFactory.Create();
            Day? dayDb = await context.Days.FindAsync(dayID);

            if (dayDb != null)
            {
                dayDb.DinnerWasCooked = wasCooked;
                await context.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task SetDinnerAsync(Guid dayID, Guid dinnerID)
        {
            using CookingContext context = ContextFactory.Create();
            Day? dayDb = await context.Days.FindAsync(dayID);
            if (dayDb != null)
            {
                dayDb.DinnerID = dinnerID;
                context.SaveChanges();
            }
        }

        /// <inheritdoc/>
        public async Task<List<Day>?> GetWeekAsync(DateTime weekday)
        {
            using CookingContext context = ContextFactory.Create();
            DateTime mondayDate = FirstDayOfWeek(weekday).Date;

            // Get day's last second
            DateTime sundayDate = EndOfDay(LastDayOfWeek(weekday));

            List<Day> weekDays = await GetCultureSpecificSet(context)
                                           .Include(x => x.Dinner)
                                           .Include(x => x.DinnerGarnish)
                                           .AsNoTracking()
                                           .Where(x => mondayDate <= x.Date && x.Date <= sundayDate)
                                           .ToListAsync();

            return weekDays.Count > 0 ? weekDays : null;
        }

        /// <inheritdoc/>
        public async Task CreateWeekAsync(DateTime weekday, Dictionary<DayOfWeek, (Guid? RecipeID, Guid? GarnishID)> selectedRecepies)
        {
            using CookingContext context = ContextFactory.Create();

            var days = new List<Day>();

            foreach (KeyValuePair<DayOfWeek, (Guid? RecipeID, Guid? GarnishID)> recipe in selectedRecepies.Where(x => x.Value.RecipeID.HasValue))
            {
                days.Add(new Day()
                {
                    DinnerID = recipe.Value!.RecipeID!.Value,
                    DinnerGarnishID = recipe.Value!.GarnishID,
                    Date = FirstDayOfWeek(weekday).AddDays(DaysFromMonday(recipe.Key)),
                    Culture = GetCurrentCulture(),
                });
            }

            await context.AddRangeAsync(days);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public int DaysFromMonday(DayOfWeek day)
        {
            // int value for sunday is 0, other weekdays ordered
            if (day == DayOfWeek.Sunday)
            {
                return 6;
            }

            return day - DayOfWeek.Monday;
        }

        /// <inheritdoc/>
        public List<ShoppingListIngredientsGroup> GetWeekShoppingList(DateTime periodStart, DateTime periodEnd)
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
                                                     .Where(x => periodStart.Date <= x.Date && x.Date <= periodEnd.Date)
                                                     .AsNoTracking()
                                                     .ToList();
#pragma warning restore CS8602, CS8604

            // Create single list of all ingredients in recipies for a week
            var ingredients = from dinner in days.Where(x => x.Dinner != null)
                              from recipeIngredient in dinner.Dinner!.Ingredients
                              select new { dinner.Dinner, Ingredient = recipeIngredient };

            // Include ingredients in groups
            var ingredientsInGroupds = from dinner in days.Where(x => x.Dinner != null)
                                       from recipeIngredient in dinner.Dinner!.IngredientGroups.SelectMany(g => g.Ingredients!)
                                       select new { dinner.Dinner, Ingredient = recipeIngredient };

            // Include ingredients in garnishes
            var garnishIngredients = from garnish in days.Where(x => x.DinnerGarnish != null)
                                     from recipeIngredient in garnish.DinnerGarnish!.Ingredients
                                     select new { Dinner = garnish.DinnerGarnish, Ingredient = recipeIngredient };

            // Include ingredients in groups
            var garnishIngredientsInGroupds = from garnish in days.Where(x => x.DinnerGarnish != null)
                                              from recipeIngredient in garnish.DinnerGarnish!.IngredientGroups.SelectMany(g => g.Ingredients!)
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
                    IngredientGroupName = localization[ingredientTypeGroup.Key]
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

                        RecipiesSources = ingredientNameGroup.Select(x => x.Dinner.Name!)
                                                             .Distinct()
                                                             .ToList()
                    });
                }

                result.Add(item);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> IsWeekFilledAsync(DateTime weekday)
        {
            List<Day>? weekdays = await GetWeekAsync(weekday);

            if (weekdays == null)
            {
                return true;
            }

            return weekdays.All(x => x.DinnerWasCooked);
        }

        /// <inheritdoc/>
        public async Task CreateDinnerAsync(DateTime weekday, Guid dinnerID, DayOfWeek dayOfWeek)
        {
            using CookingContext context = ContextFactory.Create();

            var newDay = new Day()
            {
                DinnerID = dinnerID,
                Date = FirstDayOfWeek(weekday).AddDays(DaysFromMonday(dayOfWeek)),
                Culture = GetCurrentCulture()
            };

            await context.AddAsync(newDay);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public DateTime FirstDayOfWeek(DateTime date)
        {
            int daysFromMonday = DaysFromMonday(date.DayOfWeek);
            return StartOfDay(date.AddDays(daysFromMonday * -1));
        }

        /// <inheritdoc/>
        public DateTime LastDayOfWeek(DateTime date) => FirstDayOfWeek(date).AddDays(6);

        /// <inheritdoc/>
        public async Task MoveDayToNextWeekAsync(Guid dayID, DayOfWeek selectedWeekday)
        {
            using CookingContext context = ContextFactory.Create();
            Day day = context.Days.First(x => x.ID == dayID);

            // Change date
            DateTime dayOnNextWeek = LastDayOfWeek(day.Date).AddDays(1);
            day.Date = dayOnNextWeek.AddDays(DaysFromMonday(selectedWeekday));

            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteWeekAsync(DateTime weekday)
        {
            DateTime weekStart = StartOfDay(FirstDayOfWeek(weekday));
            DateTime weekEnd = EndOfDay(LastDayOfWeek(weekday));
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
        protected override IQueryable<Day> GetFullGraph(IQueryable<Day> baseQuery)
        {
            // Includes for database querying, null-checks are not applicable.
#pragma warning disable CS8602
            return baseQuery.Include(x => x.Dinner)
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