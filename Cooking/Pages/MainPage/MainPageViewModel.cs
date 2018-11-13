using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.MainPage.Dialogs;
using Cooking.Pages.MainPage.Dialogs.Model;
using Cooking.Pages.MainPage.Dialogs.Model.CalorieTypeSelect;
using Cooking.Pages.Recepies;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Cooking.Pages.MainPage
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand LoadedCommand => new DelegateCommand(() =>
        {
            CurrentWeek = GetWeek(WeekStart);
        });

        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }

        public WeekDTO CurrentWeek { get; set; }

        private WeekDTO GetWeek(DateTime dayOfWeek)
        {
            using (var context = new CookingContext())
            {
                var currentWeek = context.Weeks.Include(x => x.Monday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.Ingredients)
                                                          .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Monday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.IngredientGroups)
                                                          .ThenInclude(x => x.Ingredients)
                                                               .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Monday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)
                                                            .ThenInclude(x => x.Tag)

                                               .Include(x => x.Tuesday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.Ingredients)
                                                          .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Tuesday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.IngredientGroups)
                                                           .ThenInclude(x => x.Ingredients)
                                                               .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Tuesday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)
                                                            .ThenInclude(x => x.Tag)

                                               .Include(x => x.Wednesday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.Ingredients)
                                                          .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Wednesday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.IngredientGroups)
                                                           .ThenInclude(x => x.Ingredients)
                                                               .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Wednesday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)
                                                            .ThenInclude(x => x.Tag)

                                               .Include(x => x.Thursday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.Ingredients)
                                                          .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Thursday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.IngredientGroups)
                                                           .ThenInclude(x => x.Ingredients)
                                                               .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Thursday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)
                                                            .ThenInclude(x => x.Tag)

                                               .Include(x => x.Friday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.Ingredients)
                                                          .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Friday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.IngredientGroups)
                                                           .ThenInclude(x => x.Ingredients)
                                                               .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Friday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)
                                                            .ThenInclude(x => x.Tag)

                                               .Include(x => x.Saturday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.Ingredients)
                                                          .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Saturday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.IngredientGroups)
                                                           .ThenInclude(x => x.Ingredients)
                                                               .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Saturday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)
                                                            .ThenInclude(x => x.Tag)

                                               .Include(x => x.Sunday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.Ingredients)
                                                          .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Sunday)
                                                  .ThenInclude(x => x.Dinner)
                                                      .ThenInclude(x => x.IngredientGroups)
                                                           .ThenInclude(x => x.Ingredients)
                                                               .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Sunday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)
                                                            .ThenInclude(x => x.Tag)

                                               .SingleOrDefault(x => x.Start.Date <= dayOfWeek.Date && dayOfWeek.Date <= x.End.Date);

                if (currentWeek != null)
                {
                    var week = Mapper.Map<WeekDTO>(currentWeek);

                    if (week.Monday != null)
                    {
                        week.Monday.PropertyChanged += Day_PropertyChanged;
                    }

                    if (week.Tuesday != null)
                    {
                        week.Tuesday.PropertyChanged += Day_PropertyChanged;
                    }

                    if (week.Wednesday != null)
                    {
                        week.Wednesday.PropertyChanged += Day_PropertyChanged;
                    }

                    if (week.Thursday != null)
                    {
                        week.Thursday.PropertyChanged += Day_PropertyChanged;
                    }

                    if (week.Friday != null)
                    {
                        week.Friday.PropertyChanged += Day_PropertyChanged;
                    }

                    if (week.Saturday != null)
                    {
                        week.Saturday.PropertyChanged += Day_PropertyChanged;
                    }

                    if (week.Sunday != null)
                    {
                        week.Sunday.PropertyChanged += Day_PropertyChanged;
                    }

                    return week;
                }
                else
                {
                    return null;
                }
            }
        }

        private void Day_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Day.DinnerWasCooked))
            {
                var day = sender as DayDTO;

                using (var context = new CookingContext())
                {
                    var dayDb = context.Days.Find(day.ID);
                    dayDb.DinnerWasCooked = day.DinnerWasCooked;
                    context.SaveChanges();
                }
            }
        }

        public MainPageViewModel()
        {
            WeekStart = FirstDayOfWeek(DateTime.Now);
            WeekEnd   = LastDayOfWeek(DateTime.Now);

            //CurrentWeek = GetWeek(DateTime.Now);

            CreateNewWeekCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(() => CreateWeek(DateTime.Now)));
            DeleteCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(() => 
            {
                using (var context = new CookingContext())
                {
                    var entity = context.Weeks.Find(CurrentWeek.ID);
                    context.Remove(entity);
                    context.SaveChanges();
                }

                CurrentWeek = null;
            }));

            CreateShoppingListCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(async () =>
            {
                var allProducts = new List<RecipeIngredientDTO>();
                if (CurrentWeek.Monday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Monday.Dinner.Ingredients);
                }

                if (CurrentWeek.Monday?.Dinner?.IngredientGroups != null)
                {
                    allProducts.AddRange(CurrentWeek.Monday.Dinner.IngredientGroups.SelectMany(x => x.Ingredients));
                }

                if (CurrentWeek.Tuesday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Tuesday.Dinner.Ingredients);
                }

                if (CurrentWeek.Tuesday?.Dinner?.IngredientGroups != null)
                {
                    allProducts.AddRange(CurrentWeek.Tuesday.Dinner.IngredientGroups.SelectMany(x => x.Ingredients));
                }

                if (CurrentWeek.Wednesday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Wednesday.Dinner.Ingredients);
                }

                if (CurrentWeek.Wednesday?.Dinner?.IngredientGroups != null)
                {
                    allProducts.AddRange(CurrentWeek.Wednesday.Dinner.IngredientGroups.SelectMany(x => x.Ingredients));
                }

                if (CurrentWeek.Thursday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Thursday.Dinner.Ingredients);
                }

                if (CurrentWeek.Thursday?.Dinner?.IngredientGroups != null)
                {
                    allProducts.AddRange(CurrentWeek.Thursday.Dinner.IngredientGroups.SelectMany(x => x.Ingredients));
                }

                if (CurrentWeek.Friday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Friday.Dinner.Ingredients);
                }

                if (CurrentWeek.Friday?.Dinner?.IngredientGroups != null)
                {
                    allProducts.AddRange(CurrentWeek.Friday.Dinner.IngredientGroups.SelectMany(x => x.Ingredients));
                }

                if (CurrentWeek.Saturday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Saturday.Dinner.Ingredients);
                }

                if (CurrentWeek.Saturday?.Dinner?.IngredientGroups != null)
                {
                    allProducts.AddRange(CurrentWeek.Saturday.Dinner.IngredientGroups.SelectMany(x => x.Ingredients));
                }

                if (CurrentWeek.Sunday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Sunday.Dinner.Ingredients);
                }

                if (CurrentWeek.Sunday?.Dinner?.IngredientGroups != null)
                {
                    allProducts.AddRange(CurrentWeek.Sunday.Dinner.IngredientGroups.SelectMany(x => x.Ingredients));
                }

                var typeGroups = allProducts.GroupBy(x => x.Ingredient?.Type?.Name).OrderBy(x => x.Key);
                var sb = new StringBuilder();
                foreach (var type in typeGroups.Where(x => x.Key != null))
                {
                    sb.AppendLine(type.Key);

                    var grouped = type.GroupBy(x => x.Ingredient?.Name).OrderBy(x => x.Key);

                    foreach (var group in grouped)
                    {
                        sb.AppendLine($"\t" + group.Key);
                        var measureGroup = group.GroupBy(x => x.MeasureUnit?.FullName);

                        foreach (var measure in measureGroup)
                        {
                            if (measure.Key != null)
                            {
                                sb.AppendLine($"\t{measure.Sum(x => x.Amount)} {measure.Key}");
                            }
                        }
                        sb.AppendLine();
                    }
                }

                sb.AppendLine("---------------------------------------------");

                foreach (var type in typeGroups.Where(x => x.Key == null))
                {
                    sb.AppendLine(type.Key);

                    var grouped = type.GroupBy(x => x.Ingredient?.Name).OrderBy(x => x.Key);

                    foreach (var group in grouped)
                    {
                        sb.AppendLine($"\t" + group.Key);
                        var measureGroup = group.GroupBy(x => x.MeasureUnit?.FullName);

                        foreach (var measure in measureGroup)
                        {
                            if (measure.Key != null)
                            {
                                sb.AppendLine($"\t{measure.Sum(x => x.Amount)} {measure.Key}");
                            }
                        }
                        sb.AppendLine();
                    }
                }

                var viewModel = new ShoppingCartViewModel(sb.ToString());

                var dialog = new CustomDialog()
                {
                    Content = new ShoppingCartView()
                    {
                        DataContext = viewModel
                    }
                };

                await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                await dialog.WaitUntilUnloadedAsync();
            }));

            ShowRecipe = new Lazy<DelegateCommand<RecipeDTO>>(
                () => new DelegateCommand<RecipeDTO>(async (recipe) => {
                    var viewModel = new RecipeViewModel(recipe);

                    var dialog = new CustomDialog()
                    {
                        Content = new RecipeView()
                        {
                            DataContext = viewModel
                        }
                    };

                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();
                }));

            SelectNextWeekCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(() => {
                    var dayOnNextWeek = WeekEnd.AddDays(1);

                    CurrentWeek = GetWeek(dayOnNextWeek);
                    WeekStart = FirstDayOfWeek(dayOnNextWeek);
                    WeekEnd = LastDayOfWeek(dayOnNextWeek);
                }));

            SelectPreviousWeekCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(() => {
                    var dayOnPreviousWeek = WeekStart.AddDays(-1);

                    CurrentWeek = GetWeek(dayOnPreviousWeek);
                    WeekStart = FirstDayOfWeek(dayOnPreviousWeek);
                    WeekEnd = LastDayOfWeek(dayOnPreviousWeek);
                }));
        }

        
        public Lazy<DelegateCommand<RecipeDTO>> ShowRecipe { get; }
        public Lazy<DelegateCommand> CreateShoppingListCommand { get; }
        public Lazy<DelegateCommand> CreateNewWeekCommand { get; }
        public Lazy<DelegateCommand> DeleteCommand { get; }
        public Lazy<DelegateCommand> SelectNextWeekCommand { get; }
        public Lazy<DelegateCommand> SelectPreviousWeekCommand { get; }
        

        private async void CreateWeek(DateTime dayOfWeek)
        {
            var viewModel = new WeekSettingsViewModel(WeekStart, WeekEnd);

            var dialog = new CustomDialog()
            {
                Title = "Фильтр для блюд на неделю",
                Content = new WeekSettings()
                {
                    DataContext = viewModel
                }
            };

            var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

            await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

            do
            {
                await dialog.WaitUntilUnloadedAsync();
            }
            while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

            if (viewModel.IsDialogResultOK)
            {
                var selectedDays = viewModel.Days.Where(x => x.IsSelected);
                GenerateRecipies(selectedDays);

                var showGeneratedWeekViewModel = new ShowGeneratedWeekViewModel(WeekStart, WeekEnd, selectedDays);

                var showGeneratedWeekView = new CustomDialog()
                {
                    Title = "Сгенерированные рецепты",
                    Content = new ShowGeneratedWeekView()
                    {
                        DataContext = showGeneratedWeekViewModel
                    }
                };

                current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

                await DialogCoordinator.Instance.ShowMetroDialogAsync(this, showGeneratedWeekView);

                do
                {
                    await showGeneratedWeekView.WaitUntilUnloadedAsync();
                }
                while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

                if (showGeneratedWeekViewModel.IsDialogResultOK)
                {
                    using (var context = new CookingContext())
                    {
                        var newWeek = new Week();
                        newWeek.Start = WeekStart;
                        newWeek.End = WeekEnd;

                        foreach (var day in showGeneratedWeekViewModel.Days)
                        {
                            switch (day.DayName)
                            {
                                case "Пн":
                                    newWeek.Monday = new Day()
                                    {
                                        DinnerID = day.SpecificRecipe != null 
                                                    ? day.SpecificRecipe?.ID
                                                    : day.Recipe?.ID,
                                        Date = WeekStart
                                    };
                                    break;
                                case "Вт":
                                    newWeek.Tuesday = new Day()
                                    {
                                        DinnerID = day.SpecificRecipe != null
                                                    ? day.SpecificRecipe?.ID
                                                    : day.Recipe?.ID,
                                        Date = WeekStart.AddDays(1)
                                    };
                                    break;
                                case "Ср":
                                    newWeek.Wednesday = new Day()
                                    {
                                        DinnerID = day.SpecificRecipe != null
                                                    ? day.SpecificRecipe?.ID
                                                    : day.Recipe?.ID,
                                        Date = WeekStart.AddDays(2)
                                    };
                                    break;
                                case "Чт":
                                    newWeek.Thursday = new Day()
                                    {
                                        DinnerID = day.SpecificRecipe != null
                                                    ? day.SpecificRecipe?.ID
                                                    : day.Recipe?.ID,
                                        Date = WeekStart.AddDays(3)
                                    };
                                    break;
                                case "Пт":
                                    newWeek.Friday = new Day()
                                    {
                                        DinnerID = day.SpecificRecipe != null
                                                    ? day.SpecificRecipe?.ID
                                                    : day.Recipe?.ID,
                                        Date = WeekStart.AddDays(4)
                                    };
                                    break;
                                case "Сб":
                                    newWeek.Saturday = new Day()
                                    {
                                        DinnerID = day.SpecificRecipe != null
                                                    ? day.SpecificRecipe?.ID
                                                    : day.Recipe?.ID,
                                        Date = WeekStart.AddDays(5)
                                    };
                                    break;
                                case "Вс":
                                    newWeek.Sunday = new Day()
                                    {
                                        DinnerID = day.SpecificRecipe != null
                                                    ? day.SpecificRecipe?.ID
                                                    : day.Recipe?.ID,
                                        Date = WeekStart.AddDays(6)
                                    };
                                    break;
                            }
                        }

                        context.Add(newWeek);
                        context.SaveChanges();
                        CurrentWeek = GetWeek(newWeek.Start);
                    }
                }
            }
        }

        private Random random = new Random();

        private void GenerateRecipies(IEnumerable<DayPlan> selectedDays)
        {
            var cacheCooked = new LastDayCooked();

            foreach (var day in selectedDays)
            {
                using (var context = new CookingContext())
                {
                    List<TagDTO> requiredTags = new List<TagDTO>();

                    if (!day.NeededDishTypes.Contains(TagDTO.Any))
                    {
                        requiredTags.AddRange(day.NeededDishTypes);
                    }

                    if (!day.NeededMainIngredients.Contains(TagDTO.Any))
                    {
                        requiredTags.AddRange(day.NeededMainIngredients);
                    }

                    var query = context.Recipies
                                       .Include(x => x.Tags)
                                           .ThenInclude(x => x.Tag)
                                       .Include(x => x.Ingredients)
                                           .ThenInclude(x => x.Ingredient).AsQueryable();

                    foreach(var tag in requiredTags)
                    {
                        query = query.Where(x => x.Tags.Any(ttag => ttag.Tag.ID == tag.ID));
                    }

                    if (!day.CalorieTypes.Contains(CalorieTypeSelection.Any))
                    {
                        var predicate = PredicateBuilder.False<Recipe>();

                        foreach (var calorieType in day.CalorieTypes)
                        {
                            predicate = predicate.Or(p => p.CalorieType == calorieType.CalorieType);
                        }

                        query = query.Where(predicate);
                    }

                    var test = query.ToList();
                    day.RecipeAlternatives = test.Select(x => Mapper.Map<RecipeDTO>(x)).ToList();

                    var selectedRecipies = selectedDays.Where(x => x.Recipe != null).Select(x => x.Recipe);
                    var recipiesNotSelectedYet = test.Where(x => !selectedRecipies.Any(selected => selected.ID == x.ID)).ToList();

                    if (recipiesNotSelectedYet.Count > 0)
                    {
                        day.Recipe = Mapper.Map<RecipeDTO>(recipiesNotSelectedYet.OrderByDescending(x => cacheCooked.DaysFromLasCook(x)).First());
                    }
                }
            }
        }

        public static DateTime FirstDayOfWeek(DateTime date)
        {
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            int dayOfWeek = date.DayOfWeek != DayOfWeek.Sunday ? (int)date.DayOfWeek : 7;
            int offset = (int)fdow - dayOfWeek;
            DateTime fdowDate = date.AddDays(offset);
            return fdowDate;
        }

        public static DateTime LastDayOfWeek(DateTime date)
        {
            DateTime ldowDate = FirstDayOfWeek(date).AddDays(6);
            return ldowDate;
        }
    }

    internal class LastDayCooked
    {
        private Dictionary<Recipe, DateTime?> cache = new Dictionary<Recipe, DateTime?>();

        public int DaysFromLasCook(Recipe recipe)
        {
            var date = DayWhenLasWasCooked(recipe);

            if (date != null)
            {
                return (int)(DateTime.Now - date.Value).TotalDays;
            }
            else
            {
                return int.MaxValue;
            }
        }

        public DateTime? DayWhenLasWasCooked(Recipe recipe)
        {
            if (cache.ContainsKey(recipe)) return cache[recipe];

            using (var context = new CookingContext())
            {
                var test = context.Days.Where(x => x.DinnerID == recipe.ID && x.DinnerWasCooked && x.Date != null).OrderBy(x => x.Date).ToList();
                return cache[recipe] = context.Days.Where(x => x.DinnerID == recipe.ID && x.DinnerWasCooked && x.Date != null).OrderBy(x => x.Date).FirstOrDefault()?.Date;
            }
        }
    }

    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}
