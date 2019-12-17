using AutoMapper.QueryableExtensions;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.MainPage;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public static class WeekService
    {
        /// <summary>
        /// Загрузка недели по любому из дней в ней
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public static async Task<WeekMainPage> GetWeekAsync(DateTime dayOfWeek)
         {
            Debug.WriteLine("WeekService.GetWeek(DateTime)");
            using var context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true);
            return await MapperService.Mapper.ProjectTo<WeekMainPage>(context.Weeks)
.SingleOrDefaultAsync(x => x.Start.Date <= dayOfWeek.Date && dayOfWeek.Date <= x.End.Date).ConfigureAwait(false);
        }

        /// <summary>
        /// Загрузка недели поп id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<WeekMainPage> GetWeekAsync(Guid id)
        {
            Debug.WriteLine("WeekService.GetWeek(Guid)");
            using var context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true);
            return await MapperService.Mapper.ProjectTo<WeekMainPage>(context.Weeks)
.SingleOrDefaultAsync(x => x.ID == id).ConfigureAwait(false);
        }

        public static async Task CreateWeekAsync(DateTime weekStart, Dictionary<DayOfWeek, Guid?> selectedRecepies)
        {
            Debug.WriteLine("WeekService.CreateWeekAsync");
            using var context = new CookingContext();
            var newWeek = new Week()
            {
                Start = weekStart,
                End = LastDayOfWeek(weekStart)
            };

            var days = new List<Day>();

            foreach (var recepie in selectedRecepies.Where(x => x.Value != null))
            {
                days.Add(new Day()
                {
                    DinnerID = recepie.Value,
                    Date = weekStart.AddDays(DaysFromMonday(recepie.Key)),
                    DayOfWeek = recepie.Key
                });
            }

            newWeek.Days = days;
            await context.AddAsync(newWeek);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public static List<ShoppingListItem> GetWeekIngredients(Guid id)
        {
            Debug.WriteLine("WeekService.GetWeekIngredients");
            using var context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true);
            var week = context.Weeks.Find(id);

            var ingredients = from dinner in week.Days.Where(x => x.Dinner?.Ingredients != null)
                              from ingredient in dinner.Dinner.Ingredients
                              select new { dinner.Dinner, Ingredient = ingredient };

            var ingredientsInGroupds = from dinner in week.Days.Where(x => x.Dinner?.IngredientGroups != null)
                                       from ingredient in dinner.Dinner.IngredientGroups.SelectMany(g => g.Ingredients)
                                       select new { dinner.Dinner, Ingredient = ingredient };

            var allIngredients = ingredients.Union(ingredientsInGroupds);

            var ingredientGroups = allIngredients.GroupBy(x => x.Ingredient.Ingredient.Type?.Name).OrderBy(x => x.Key);

            var result = new List<ShoppingListItem>();

            foreach (var ingredientGroup in ingredientGroups)
            {
                var item = new ShoppingListItem
                {
                    IngredientGroupName = ingredientGroup.Key ?? "Без категории"
                };

                foreach (var ingredient in ingredientGroup.GroupBy(x => x.Ingredient.Ingredient.Name))
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
                        RecipiesSources = ingredient.Select(x => x.Dinner.Name).Distinct().ToList()
                    });
                }

                result.Add(item);
            }

            return result;
        }

        public static bool IsWeekFilled(DateTime dayOfWeek)
        {
            Debug.WriteLine("WeekService.IsWeekFilled");
            using var context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true);
            var week = GetWeekInternal(dayOfWeek, context);

            if (week == null || week.Days == null)
            {
                return true;
            }

            return week.Days.All(x => x.DinnerWasCooked);
        }

        public static DayOfWeek GetDayOfWeek(string name)
        {
            Debug.WriteLine("WeekService.GetDayOfWeek");
            return name switch
            {
                "Понедельник" => DayOfWeek.Monday,
                "Вторник" => DayOfWeek.Tuesday,
                "Среда" => DayOfWeek.Wednesday,
                "Четверг" => DayOfWeek.Thursday,
                "Пятница" => DayOfWeek.Friday,
                "Суббота" => DayOfWeek.Saturday,
                "Воскресенье" => DayOfWeek.Sunday,
                _ => throw new InvalidOperationException(),
            };
        }
        public static async Task DeleteWeekAsync(Guid id)
        {
            Debug.WriteLine("WeekService.DeleteWeekAsync");
            using var context = new CookingContext(DatabaseService.DbFileName);
            var entity = await context.Weeks.FindAsync(id);
            context.Remove(entity);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public static int DaysFromMonday(DayOfWeek day)
        {
            Debug.WriteLine("WeekService.DaysFromMonday");
            if (day == DayOfWeek.Sunday)
            {
                return 6;
            }

            return day - DayOfWeek.Monday;
        }

        public static DateTime FirstDayOfWeek(DateTime date)
        {
            Debug.WriteLine("WeekService.FirstDayOfWeek");
            DayOfWeek fdow = DayOfWeek.Monday;
            int dayOfWeek = date.DayOfWeek != DayOfWeek.Sunday ? (int)date.DayOfWeek : 7;
            int offset = (int)fdow - dayOfWeek;
            DateTime fdowDate = date.AddDays(offset);
            return fdowDate;
        }

        public static DateTime LastDayOfWeek(DateTime date)
        {
            Debug.WriteLine("WeekService.LastDayOfWeek");
            DateTime ldowDate = FirstDayOfWeek(date).AddDays(6);
            return ldowDate;
        }

        public static async Task MoveDayToNextWeek(Guid currentWeekId, Guid dayId, DayOfWeek selectedWeekday)
        {
            Debug.WriteLine("WeekService.MoveDayToNextWeek");
            using var context = new CookingContext(DatabaseService.DbFileName, useLazyLoading: true);
            var day = context.Days.First(x => x.ID == dayId);

            // Удаление дня на этой неделе
            var week = context.Weeks.First(x => x.ID == currentWeekId);
            week.Days.Remove(day);

            // Перенос дня на неделю вперёд
            var dayOnNextWeek = week.End.AddDays(1);
            var nextWeek = GetWeekInternal(dayOnNextWeek, context);

            if (nextWeek == null)
            {
                nextWeek = CreateWeekInternal(dayOnNextWeek, context);
                nextWeek.Days = new List<Day>();
            }
            else
            {
                nextWeek.Days.RemoveAll(x => x.DayOfWeek == selectedWeekday);
            }

            day.Date = nextWeek.Start.AddDays(DaysFromMonday(selectedWeekday));
            day.DayOfWeek = selectedWeekday;

            nextWeek.Days.Add(day);
            day.WeekID = nextWeek.ID;

            await context.SaveChangesAsync().ConfigureAwait(false);
        }


        private static Week GetWeekInternal(DateTime dayOnWeek, CookingContext context)
        {
            Debug.WriteLine("WeekService.GetWeekInternal");
            return context.Weeks.SingleOrDefault(x => x.Start.Date <= dayOnWeek.Date && dayOnWeek.Date <= x.End.Date);
        }

        private static Week CreateWeekInternal(DateTime dayOnWeek, CookingContext context)
        {
            Debug.WriteLine("WeekService.CreateWeekInternal");
            var week = new Week()
            {
                Start = FirstDayOfWeek(dayOnWeek),
                End = LastDayOfWeek(dayOnWeek)
            };
            context.Add(week);

            return week;
        }
    }
}
