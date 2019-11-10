using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Dialogs;
using Cooking.Pages.Recepies;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public class MainPageViewModel
    {
        private readonly DialogUtils dialogUtils;

        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public bool WeekEdit { get; set; }
        public WeekMain? CurrentWeek { get; set; }

        public DelegateCommand LoadedCommand { get; }
        public DelegateCommand CreateShoppingListCommand { get; }
        public DelegateCommand CreateNewWeekCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand SelectNextWeekCommand { get; }
        public DelegateCommand SelectPreviousWeekCommand { get; }
        public DelegateCommand<Guid> ShowRecipeCommand { get; }
        public DelegateCommand<Guid> MoveRecipeCommand { get; }
        public DelegateCommand<string> SelectDinnerCommand { get; }
        public DelegateCommand<Guid> DeleteDinnerCommand { get; }

        public MainPageViewModel()
        {
            Debug.WriteLine("MainPageViewModel.ctor");
            dialogUtils                 = new DialogUtils(this);
            LoadedCommand               = new DelegateCommand(OnLoadedAsync, executeOnce: true);
            CreateNewWeekCommand        = new DelegateCommand(CreateNewWeekAsync);
            CreateShoppingListCommand   = new DelegateCommand(CreateShoppingListAsync);
            DeleteCommand               = new DelegateCommand(DeleteCurrentWeekAsync);
            SelectNextWeekCommand       = new DelegateCommand(SelectNextWeekAsync);
            SelectPreviousWeekCommand   = new DelegateCommand(SelectPreviousWeekAsync);
            ShowRecipeCommand           = new DelegateCommand<Guid>(ShowRecipeAsync);
            DeleteDinnerCommand         = new DelegateCommand<Guid>(DeleteDayAsync);
            SelectDinnerCommand         = new DelegateCommand<string>(SelectDinner);
            MoveRecipeCommand           = new DelegateCommand<Guid>(MoveRecipe);
        }

        private async Task<WeekMain?> GetWeekAsync(DateTime dayOfWeek)
        {
            Debug.WriteLine("MainPageViewModel.GetWeekAsync");
            var weekData = await WeekService.GetWeekAsync(dayOfWeek).ConfigureAwait(false);
            if (weekData == null)
            {
                return null;
            }

            var weekMain = MapperService.Mapper.Map<WeekMain>(weekData);
            if (weekMain == null)
            {
                return null;
            }

            foreach (var day in weekMain.Days)
            {
                day.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(DayMain.DinnerWasCooked))
                    {
                        if (sender is DayMain dayChanged)
                        {
                            DayService.SetDinnerWasCooked(dayChanged.ID, dayChanged.DinnerWasCooked);
                        }
                    }
                };
            }
            
            return weekMain;
        }
        
        private async void ShowRecipeAsync(Guid recipeId)
        {
            Debug.WriteLine("MainPageViewModel.ShowRecipeAsync");
            await dialogUtils.ShowCustomMessageAsync<RecipeView, RecipeViewModel>(content: new RecipeViewModel(recipeId)).ConfigureAwait(false);
        }

        private async void SelectDinner(string dayName)
        {
            Debug.WriteLine("MainPageViewModel.SelectDinner");
            var viewModel = await dialogUtils.ShowCustomMessageAsync<RecipeSelectView, RecipeSelectViewModel>().ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var dayOfWeek = WeekService.GetDayOfWeek(dayName);
                var day = CurrentWeek.Days.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);

                if (day != null)
                {
                    await DayService.SetDinner(day.ID, viewModel.SelectedRecipeID).ConfigureAwait(false);
                }
                else
                {
                    await DayService.CreateDinner(CurrentWeek.ID, viewModel.SelectedRecipeID, dayOfWeek).ConfigureAwait(false);
                }

                await ReloadCurrentWeek().ConfigureAwait(false);
            }
        }

        private async Task ReloadCurrentWeek()
        {
            CurrentWeek = await GetWeekAsync(CurrentWeek.Start).ConfigureAwait(false);
        }

        private async void OnLoadedAsync()
        {
            Debug.WriteLine("MainPageViewModel.OnLoadedAsync");
            await SetWeekByDay(DateTime.Now).ConfigureAwait(false);

            var dayOnPreviousWeek = WeekService.FirstDayOfWeek(DateTime.Now).AddDays(-1);
            var prevWeekFilled    = WeekService.IsWeekFilled(dayOnPreviousWeek);

            if (!prevWeekFilled)
            {
                // Нужно напомнить о рецептах на прошедшей неделе
                var result = await DialogCoordinator.Instance.ShowMessageAsync(this,
                      "Кстати",
                      "За прошлую неделю есть рецепты, которые не были приготовлены. Рекомендуем их удалить или перенести",
                      MessageDialogStyle.AffirmativeAndNegative,
                      new MetroDialogSettings()
                      {
                          AffirmativeButtonText = "Перейти к предыдущей неделе",
                          NegativeButtonText = "Закрыть"
                      }).ConfigureAwait(false);

                if (result == MessageDialogResult.Affirmative)
                {
                    SelectPreviousWeekCommand.Execute();
                }
            }
        }

        private async void MoveRecipe(Guid dayId)
        {
            Debug.WriteLine("MainPageViewModel.MoveRecipe");
            var viewModel = await dialogUtils.ShowCustomMessageAsync<MoveRecipe, MoveRecipeViewModel>().ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var selectedDay = viewModel.DaysOfWeek.Single(x => x.IsSelected);
                await WeekService.MoveDayToNextWeek(CurrentWeek.ID, dayId, selectedDay.WeekDay).ConfigureAwait(false);
                await ReloadCurrentWeek().ConfigureAwait(false);
            }
        }

        private async void SelectPreviousWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.SelectPreviousWeekAsync");
            var dayOnPreviousWeek = WeekStart.AddDays(-1);
            await SetWeekByDay(dayOnPreviousWeek).ConfigureAwait(false);
        }

        private async void SelectNextWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.SelectNextWeekAsync");
            var dayOnNextWeek = WeekEnd.AddDays(1);
            await SetWeekByDay(dayOnNextWeek).ConfigureAwait(false);
        }

        private async Task SetWeekByDay(DateTime date)
        {
            Debug.WriteLine("MainPageViewModel.SetWeekByDay");
            CurrentWeek = await GetWeekAsync(date).ConfigureAwait(false);
            WeekStart = WeekService.FirstDayOfWeek(date);
            WeekEnd = WeekService.LastDayOfWeek(date);
        }

        private async void CreateShoppingListAsync()
        {
            Debug.WriteLine("MainPageViewModel.CreateShoppingListAsync");
            var allProducts = WeekService.GetWeekIngredients(CurrentWeek.ID);

            await dialogUtils.ShowCustomMessageAsync<ShoppingCartView, ShoppingCartViewModel>(content: new ShoppingCartViewModel(allProducts)).ConfigureAwait(false);
        }

        private async void DeleteDayAsync(Guid dayId)
        {
            Debug.WriteLine("MainPageViewModel.DeleteDayAsync");
            var result = await DialogCoordinator.Instance.ShowMessageAsync(this,
                    "Точно?",
                    "Удаляем день?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Да",
                        NegativeButtonText = "Отмена"
                    }).ConfigureAwait(false);

            if (result == MessageDialogResult.Affirmative)
            {
                await DayService.DeleteDay(dayId).ConfigureAwait(false);
                await ReloadCurrentWeek().ConfigureAwait(false);
            }
        }

        private async void DeleteCurrentWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.DeleteCurrentWeekAsync");
            var result = await DialogCoordinator.Instance.ShowMessageAsync(this,
                    "Точно?",
                    "Удаляем неделю?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Да",
                        NegativeButtonText = "Отмена"
                    }).ConfigureAwait(false);

            if (result == MessageDialogResult.Affirmative)
            {
                await WeekService.DeleteWeekAsync(CurrentWeek.ID).ConfigureAwait(false);
                CurrentWeek = null;
            }
        }

        private async void CreateNewWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.CreateNewWeekAsync");
            ShowGeneratedWeekViewModel showGeneratedWeekViewModel;
            WeekSettingsViewModel weekSettingsViewModel = new WeekSettingsViewModel(WeekStart, WeekEnd, dialogUtils);
            do
            {
                var viewModel = await dialogUtils.ShowCustomMessageAsync<WeekSettings, WeekSettingsViewModel>(
                                        "Фильтр для блюд на неделю", 
                                        weekSettingsViewModel
                                      ).ConfigureAwait(false);
                
                if (!weekSettingsViewModel.DialogResultOk) return;

                var selectedDays = weekSettingsViewModel.Days.Skip(1).Where(x => x.IsSelected);
                GenerateRecipies(selectedDays);

                showGeneratedWeekViewModel = await dialogUtils.ShowCustomMessageAsync<ShowGeneratedWeekView, ShowGeneratedWeekViewModel>(
                                                    "Сгенерированные рецепты",
                                                    new ShowGeneratedWeekViewModel(WeekStart, WeekEnd, selectedDays)
                                                  ).ConfigureAwait(false);

                if (showGeneratedWeekViewModel.DialogResultOk)
                {
                    var daysDictionary = showGeneratedWeekViewModel.Days.ToDictionary(x => x.DayOfWeek, x => x.SpecificRecipe?.ID ?? x.Recipe?.ID);

                    await WeekService.CreateWeekAsync(WeekStart, daysDictionary).ConfigureAwait(false);
                    CurrentWeek = await GetWeekAsync(WeekStart).ConfigureAwait(false);
                }
            }
            while (showGeneratedWeekViewModel.ReturnBack);
        }

        private void GenerateRecipies(IEnumerable<DayPlan> selectedDays)
        {
            Debug.WriteLine("MainPageViewModel.GenerateRecipies");
            foreach (var day in selectedDays)
            {
                var requiredTags = new List<Guid>();

                if (!day.NeededDishTypes.Contains(TagDTO.Any))
                {
                    requiredTags.AddRange(day.NeededDishTypes.Select(x => x.ID));
                }

                if (!day.NeededMainIngredients.Contains(TagDTO.Any))
                {
                    requiredTags.AddRange(day.NeededMainIngredients.Select(x => x.ID));
                }

                var requiredCalorieTyoes = new List<CalorieType>();
                if (!day.CalorieTypes.Contains(CalorieTypeSelection.Any))
                {
                    requiredCalorieTyoes.AddRange(day.CalorieTypes.Select(x => x.CalorieType));
                }

                day.RecipeAlternatives = RecipeService.GetRecipiesByParameters(requiredTags, requiredCalorieTyoes, day.MaxComplexity, day.MinRating, day.OnlyNewRecipies);

                var selectedRecipies = selectedDays.Where(x => x.Recipe != null).Select(x => x.Recipe);
                var recipiesNotSelectedYet = day.RecipeAlternatives.Where(x => !selectedRecipies.Any(selected => selected.ID == x.ID)).ToList();

                if (recipiesNotSelectedYet.Count > 0)
                { 
                    day.Recipe = recipiesNotSelectedYet.OrderByDescending(x => RecipeService.DaysFromLasCook(x.ID)).First();
                }
            }
        }
    }
}
