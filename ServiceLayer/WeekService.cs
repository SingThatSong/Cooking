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
    public class WeekService : CRUDService<Week>
    {
        public WeekService(IContextFactory contextFactory) : base(contextFactory)
        {
        }

        /// <summary>
        /// Загрузка недели по любому из дней в ней
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public async Task<Week> GetWeekAsync(DateTime dayOfWeek)
        {
            Debug.WriteLine("WeekService.GetWeek(DateTime)");
            using CookingContext context = ContextFactory.Create();
            return await GetCultureSpecificSet(context)
                                .Include(x => x.Days)
                                    .ThenInclude(x => x.Dinner)
                                .SingleOrDefaultAsync(x => x.Start.Date <= dayOfWeek.Date
                                                        && dayOfWeek.Date <= x.End.Date).ConfigureAwait(false);
        }

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

            foreach (KeyValuePair<DayOfWeek, Guid?> recepie in selectedRecepies.Where(x => x.Value != null))
            {
                days.Add(new Day()
                {
                    DinnerID = recepie.Value,
                    Date = weekStart.AddDays(DaysFromMonday(recepie.Key)),
                    DayOfWeek = recepie.Key,
                    ID = Guid.NewGuid(),
                    Culture = GetCurrentCulture()
                });
            }

            newWeek.Days = days;
            await context.AddAsync(newWeek);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public List<ShoppingListItem> GetWeekIngredients(Guid id)
        {
            Debug.WriteLine("WeekService.GetWeekIngredients");
            using CookingContext context = ContextFactory.Create(useLazyLoading: true);
            Week week = GetCultureSpecificSet(context).First(x => x.ID == id);

            var ingredients = from dinner in week.Days.Where(x => x.Dinner?.Ingredients != null)
                              from recipeIngredient in dinner.Dinner!.Ingredients
                              select new { dinner.Dinner, Ingredient = recipeIngredient };

            var ingredientsInGroupds = from dinner in week.Days.Where(x => x.Dinner?.IngredientGroups != null)
                                       from recipeIngredient in dinner.Dinner!.IngredientGroups.SelectMany(g => g.Ingredients)
                                       select new { dinner.Dinner, Ingredient = recipeIngredient };

            var allIngredients = ingredients.Union(ingredientsInGroupds);

            var ingredientGroups = allIngredients.Where(x => x.Ingredient.Ingredient != null)
                                                 .GroupBy(x => x.Ingredient.Ingredient!.Type?.Name)
                                                 .OrderBy(x => x.Key);

            var result = new List<ShoppingListItem>();

            foreach (var ingredientGroup in ingredientGroups)
            {
                var item = new ShoppingListItem
                {
                    IngredientGroupName = ingredientGroup.Key ?? "Без категории"
                };

                foreach (var ingredient in ingredientGroup.GroupBy(x => x.Ingredient.Ingredient!.Name))
                {
                    var measures = ingredient.GroupBy(x => x.Ingredient.MeasureUnit?.FullName);
                    item.Ingredients.Add(new IngredientItem()
                    {
                        Name = ingredient.Key,
                        IngredientAmounts = measures.Where(x => x.Key != null)
                                                    .Select(x => new IngredientAmount()
                                                    {
                                                        Name = x.Key!,
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

        public DateTime FirstDayOfWeek(DateTime date)
        {
            Debug.WriteLine("WeekService.FirstDayOfWeek");
            DayOfWeek weekStart = DayOfWeek.Monday;
            int dayOfWeek = date.DayOfWeek != DayOfWeek.Sunday ? (int)date.DayOfWeek : 7;
            int offset = (int)weekStart - dayOfWeek;
            DateTime fdowDate = date.AddDays(offset);
            return fdowDate;
        }

        public DateTime LastDayOfWeek(DateTime date)
        {
            Debug.WriteLine("WeekService.LastDayOfWeek");
            DateTime ldowDate = FirstDayOfWeek(date).AddDays(6);
            return ldowDate;
        }

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
