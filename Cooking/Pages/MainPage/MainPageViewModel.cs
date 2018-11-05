using AutoMapper;
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
using Prism.Commands;
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
                                                        .ThenInclude(x => x.Tags)

                                               .Include(x => x.Tuesday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Tuesday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)

                                               .Include(x => x.Wednesday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Wednesday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)

                                               .Include(x => x.Thursday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Thursday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)

                                               .Include(x => x.Friday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Friday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)

                                               .Include(x => x.Saturday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Saturday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)

                                               .Include(x => x.Sunday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Ingredients)
                                                            .ThenInclude(x => x.Ingredient)
                                               .Include(x => x.Sunday)
                                                    .ThenInclude(x => x.Dinner)
                                                        .ThenInclude(x => x.Tags)

                                               .SingleOrDefault(x => x.Start.Date <= dayOfWeek.Date && dayOfWeek.Date <= x.End.Date);

                if (currentWeek != null)
                {
                    return Mapper.Map<WeekDTO>(currentWeek);
                }
                else
                {
                    return null;
                }
            }
        }

        public MainPageViewModel()
        {
            WeekStart = FirstDayOfWeek(DateTime.Now);
            WeekEnd   = LastDayOfWeek(DateTime.Now);

            CurrentWeek = GetWeek(DateTime.Now);

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

                if (CurrentWeek.Tuesday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Tuesday.Dinner.Ingredients);
                }

                if (CurrentWeek.Wednesday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Wednesday.Dinner.Ingredients);
                }

                if (CurrentWeek.Thursday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Thursday.Dinner.Ingredients);
                }

                if (CurrentWeek.Friday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Friday.Dinner.Ingredients);
                }

                if (CurrentWeek.Saturday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Saturday.Dinner.Ingredients);
                }

                if (CurrentWeek.Sunday?.Dinner?.Ingredients != null)
                {
                    allProducts.AddRange(CurrentWeek.Sunday.Dinner.Ingredients);
                }

                var grouped = allProducts.GroupBy(x => x.Ingredient.Name).OrderBy(x => x.Key);

                var sb = new StringBuilder();
                foreach(var group in grouped)
                {
                    sb.AppendLine(group.Key);
                    var measureGroup = group.GroupBy(x => x.MeasureUnit?.FullName);
                    
                    foreach(var measure in measureGroup)
                    {
                        if (measure.Key != null)
                        {
                            sb.AppendLine($"{measure.Sum(x => x.Amount)} {measure.Key}");
                        }
                    }
                    sb.AppendLine();
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
                                        DinnerID = day.Recipe.ID
                                    };
                                    break;
                                case "Вт":
                                    newWeek.Tuesday = new Day()
                                    {
                                        DinnerID = day.Recipe.ID
                                    };
                                    break;
                                case "Ср":
                                    newWeek.Wednesday = new Day()
                                    {
                                        DinnerID = day.Recipe.ID
                                    };
                                    break;
                                case "Чт":
                                    newWeek.Thursday = new Day()
                                    {
                                        DinnerID = day.Recipe.ID
                                    };
                                    break;
                                case "Пт":
                                    newWeek.Friday = new Day()
                                    {
                                        DinnerID = day.Recipe.ID
                                    };
                                    break;
                                case "Сб":
                                    newWeek.Saturday = new Day()
                                    {
                                        DinnerID = day.Recipe.ID
                                    };
                                    break;
                                case "Вс":
                                    newWeek.Sunday = new Day()
                                    {
                                        DinnerID = day.Recipe.ID
                                    };
                                    break;
                            }
                        }

                        context.Add(newWeek);
                        context.SaveChanges();
                    }
                    CurrentWeek = GetWeek(DateTime.Now);
                }
            }
        }

        private Random random = new Random();

        private void GenerateRecipies(IEnumerable<DayPlan> selectedDays)
        {
            foreach(var day in selectedDays)
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
                                       .Include(x => x.Ingredients)
                                           .ThenInclude(x => x.Ingredient).AsQueryable();

                    foreach(var tag in requiredTags)
                    {
                        query = query.Where(x => x.Tags.Any(ttag => ttag.ID == tag.ID));
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
                        day.Recipe = Mapper.Map<RecipeDTO>(recipiesNotSelectedYet[random.Next(0, recipiesNotSelectedYet.Count - 1)]);
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
