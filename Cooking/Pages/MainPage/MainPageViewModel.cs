using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Mappings;
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

        public bool WeekEdit { get; set; }

        public WeekDTO CurrentWeek { get; set; }

        private WeekDTO GetWeek(DateTime dayOfWeek)
        {
            using (var context = new CookingContext(useLazyLoading: true))
            {
                var currentWeek = context.Weeks.SingleOrDefault(x => x.Start.Date <= dayOfWeek.Date && dayOfWeek.Date <= x.End.Date);

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
                    context.Entry(entity).Reference(x => x.Monday).Load();
                    context.Entry(entity).Reference(x => x.Tuesday).Load();
                    context.Entry(entity).Reference(x => x.Wednesday).Load();
                    context.Entry(entity).Reference(x => x.Thursday).Load();
                    context.Entry(entity).Reference(x => x.Friday).Load();
                    context.Entry(entity).Reference(x => x.Saturday).Load();
                    context.Entry(entity).Reference(x => x.Sunday).Load();

                    if (entity.Monday != null)
                    {
                        context.Remove(entity.Monday);
                    }
                    if (entity.Monday != null)
                    {
                        context.Remove(entity.Tuesday);
                    }
                    if (entity.Monday != null)
                    {
                        context.Remove(entity.Wednesday);
                    }

                    if (entity.Monday != null)
                    {
                        context.Remove(entity.Thursday);
                    }
                    if (entity.Monday != null)
                    {
                        context.Remove(entity.Friday);
                    }
                    if (entity.Monday != null)
                    {
                        context.Remove(entity.Saturday);
                    }
                    if (entity.Monday != null)
                    {
                        context.Remove(entity.Sunday);
                    }

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

            DeleteDinnerCommand = new Lazy<DelegateCommand<DayDTO>>(() => new DelegateCommand<DayDTO>(day => 
            {
                using (var context = new CookingContext())
                {
                    var dayDb = context.Days.Where(x => x.ID == day.ID).Include(x => x.Dinner).Single();
                    dayDb.Dinner = null;
                    dayDb.DinnerWasCooked = false;
                    context.SaveChanges();
                }
                day.Dinner = null;
                day.DinnerWasCooked = false;
            }));

            SelectDinnerCommand = new Lazy<DelegateCommand<DayDTO>>(() => new DelegateCommand<DayDTO>(async day =>
            {
                var viewModel = new RecipeSelectViewModel();

                var dialog = new CustomDialog()
                {
                    Content = new RecipeSelectView()
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

                if (viewModel.DialogResultOk)
                {
                    day.Dinner = viewModel.Recipies.Value.SingleOrDefault(x => x.IsSelected);

                    if (day.Dinner != null)
                    {
                        using (var context = new CookingContext())
                        {
                            var dayDb = context.Days.Find(day.ID);
                            dayDb.DinnerID = day.Dinner.ID;
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }));

            MoveRecipe = new Lazy<DelegateCommand<DayDTO>>(() => new DelegateCommand<DayDTO>(async day => 
            {
                var viewModel = new MoveRecipeViewModel();

                var dialog = new CustomDialog()
                {
                    Content = new MoveRecipe()
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

                if (viewModel.DialogResultOk)
                {
                    if (day.Dinner != null)
                    {
                        using (var context = new CookingContext(useLazyLoading: true))
                        {
                            // Удаление дня на этой неделе
                            var week = context.Weeks.Find(CurrentWeek.ID);

                            if (CurrentWeek.MondayID == day.ID)
                            {
                                week.MondayID = null;
                                CurrentWeek.Monday = null;
                                CurrentWeek.MondayID = null;
                            }
                            else if (CurrentWeek.TuesdayID == day.ID)
                            {
                                week.TuesdayID = null;
                                CurrentWeek.Tuesday = null;
                                CurrentWeek.TuesdayID = null;
                            }
                            else if (CurrentWeek.WednesdayID == day.ID)
                            {
                                week.WednesdayID = null;
                                CurrentWeek.Wednesday = null;
                                CurrentWeek.WednesdayID = null;
                            }
                            else if (CurrentWeek.ThursdayID == day.ID)
                            {
                                week.ThursdayID = null;
                                CurrentWeek.Thursday = null;
                                CurrentWeek.ThursdayID = null;
                            }
                            else if (CurrentWeek.FridayID == day.ID)
                            {
                                week.FridayID = null;
                                CurrentWeek.Friday = null;
                                CurrentWeek.FridayID = null;
                            }
                            else if (CurrentWeek.SaturdayID == day.ID)
                            {
                                week.SaturdayID = null;
                                CurrentWeek.Saturday = null;
                                CurrentWeek.SaturdayID = null;
                            }
                            else if (CurrentWeek.SundayID == day.ID)
                            {
                                week.SundayID = null;
                                CurrentWeek.Sunday = null;
                                CurrentWeek.SundayID = null;
                            }

                            // Перенос дня на неделю вперёд
                            var dayOnNextWeek = WeekEnd.AddDays(1);
                            var nextWeek = context.Weeks.SingleOrDefault(x => x.Start.Date <= dayOnNextWeek.Date && dayOnNextWeek.Date <= x.End.Date);
                            if (nextWeek == null)
                            {
                                nextWeek = new Week()
                                {
                                    Start = FirstDayOfWeek(dayOnNextWeek),
                                    End = LastDayOfWeek(dayOnNextWeek)
                                };
                                context.Add(nextWeek);
                            }

                            var selectedDay = viewModel.DaysOfWeek.Single(x => x.IsSelected);
                            
                            switch (selectedDay.WeekDay)
                            {
                                case DayOfWeek.Monday:
                                    nextWeek.MondayID = day.ID;
                                    break;
                                case DayOfWeek.Tuesday:
                                    nextWeek.TuesdayID = day.ID;
                                    break;
                                case DayOfWeek.Wednesday:
                                    nextWeek.WednesdayID = day.ID;
                                    break;
                                case DayOfWeek.Thursday:
                                    nextWeek.ThursdayID = day.ID;
                                    break;
                                case DayOfWeek.Friday:
                                    nextWeek.FridayID = day.ID;
                                    break;
                                case DayOfWeek.Saturday:
                                    nextWeek.SaturdayID = day.ID;
                                    break;
                                case DayOfWeek.Sunday:
                                    nextWeek.SundayID = day.ID;
                                    break;
                            }

                            await context.SaveChangesAsync();
                        }
                    }
                }
            }));
        }


        public Lazy<DelegateCommand<DayDTO>> MoveRecipe { get; }
        public Lazy<DelegateCommand<DayDTO>> SelectDinnerCommand { get; }
        public Lazy<DelegateCommand<DayDTO>> DeleteDinnerCommand { get; }
        public Lazy<DelegateCommand<RecipeDTO>> ShowRecipe { get; }
        public Lazy<DelegateCommand> CreateShoppingListCommand { get; }
        public Lazy<DelegateCommand> CreateNewWeekCommand { get; }
        public Lazy<DelegateCommand> DeleteCommand { get; }
        public Lazy<DelegateCommand> SelectNextWeekCommand { get; }
        public Lazy<DelegateCommand> SelectPreviousWeekCommand { get; }
        

        private async void CreateWeek(DateTime dayOfWeek)
        {
            ShowGeneratedWeekViewModel showGeneratedWeekViewModel;
            WeekSettingsViewModel weekSettingsViewModel = null;
            do
            {
                weekSettingsViewModel = weekSettingsViewModel ?? new WeekSettingsViewModel(WeekStart, WeekEnd);

                var dialog = new CustomDialog()
                {
                    Title = "Фильтр для блюд на неделю",
                    Content = new WeekSettings()
                    {
                        DataContext = weekSettingsViewModel
                    }
                };

                var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

                await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

                do
                {
                    await dialog.WaitUntilUnloadedAsync();
                }
                while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

                if (!weekSettingsViewModel.IsDialogResultOK) return;

                var selectedDays = weekSettingsViewModel.Days.Skip(1).Where(x => x.IsSelected);
                GenerateRecipies(selectedDays);

                showGeneratedWeekViewModel = new ShowGeneratedWeekViewModel(WeekStart, WeekEnd, selectedDays);

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
                    using (var context = new CookingContext(useLazyLoading: true))
                    {
                        var newWeek = new Week
                        {
                            Start = WeekStart,
                            End = WeekEnd
                        };

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
            while (showGeneratedWeekViewModel.ReturnBack);
        }

        private void GenerateRecipies(IEnumerable<DayPlan> selectedDays)
        {
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

                    if (requiredTags.Count > 0)
                    {
                        var predicate = PredicateBuilder.False<Recipe>();

                        foreach (var tag in requiredTags)
                        {
                            predicate = predicate.Or(x => x.Tags.Any(ttag => ttag.Tag.ID == tag.ID));
                        }

                        query = query.Where(predicate);
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

                    if (day.MaxComplexity != null)
                    {
                        query = query.Where(x => x.Difficulty <= day.MaxComplexity);
                    }

                    if (day.MinRating != null)
                    {
                        query = query.Where(x => x.Rating >= day.MinRating);
                    }

                    var test = query.ToList();

                    if (day.OnlyNewRecipies)
                    {
                        test = test.Where(x => LastDayCooked.DayWhenLasWasCooked(x) == null).ToList();
                    }

                    day.RecipeAlternatives = test.OrderByDescending(x => LastDayCooked.DaysFromLasCook(x))
                                                 .Select(x => Mapper.Map<RecipeDTO>(x))
                                                 .ToList();

                    var selectedRecipies = selectedDays.Where(x => x.Recipe != null).Select(x => x.Recipe);
                    var recipiesNotSelectedYet = test.Where(x => !selectedRecipies.Any(selected => selected.ID == x.ID)).ToList();

                    if (recipiesNotSelectedYet.Count > 0)
                    {
                        day.Recipe = Mapper.Map<RecipeDTO>(recipiesNotSelectedYet.OrderByDescending(x => LastDayCooked.DaysFromLasCook(x)).First());
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
