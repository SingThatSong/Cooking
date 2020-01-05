using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.ServiceLayer;
using MahApps.Metro.Controls.Dialogs;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : INavigationAware
    {
        private readonly DialogService dialogUtils;
        private readonly IRegionManager regionManager;
        private readonly IContainerExtension container;
        private readonly DayService dayService;
        private readonly IMapper mapper;

        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public bool WeekEdit { get; set; }
        public WeekEdit? CurrentWeek { get; set; }

        public AsyncDelegateCommand LoadedCommand { get; }
        public DelegateCommand CreateShoppingListCommand { get; }
        public DelegateCommand CreateNewWeekCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand SelectNextWeekCommand { get; }
        public DelegateCommand SelectPreviousWeekCommand { get; }
        public DelegateCommand<Guid> ShowRecipeCommand { get; }
        public DelegateCommand<Guid> MoveRecipeCommand { get; }
        public DelegateCommand<string> SelectDinnerCommand { get; }
        public DelegateCommand<Guid> DeleteDinnerCommand { get; }

        public MainViewModel(DialogService dialogUtils, 
                             IRegionManager regionManager, 
                             IContainerExtension container,
                             DayService dayService,
                             IMapper mapper)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(regionManager != null);
            Debug.Assert(container != null);
            Debug.Assert(dayService != null);
            Debug.Assert(mapper != null);
            Debug.WriteLine("MainPageViewModel.ctor");

            this.dialogUtils            = dialogUtils;
            this.regionManager          = regionManager;
            this.container              = container;
            this.dayService             = dayService;
            this.mapper                 = mapper;

            LoadedCommand               = new AsyncDelegateCommand(OnLoadedAsync, executeOnce: true);
            CreateNewWeekCommand        = new DelegateCommand(CreateNewWeekAsync);
            CreateShoppingListCommand   = new DelegateCommand(CreateShoppingList);
            DeleteCommand               = new DelegateCommand(DeleteCurrentWeekAsync);
            SelectNextWeekCommand       = new DelegateCommand(SelectNextWeekAsync);
            SelectPreviousWeekCommand   = new DelegateCommand(SelectPreviousWeekAsync);
            ShowRecipeCommand           = new DelegateCommand<Guid>(ShowRecipe);
            DeleteDinnerCommand         = new DelegateCommand<Guid>(DeleteDayAsync);
            SelectDinnerCommand         = new DelegateCommand<string>(SelectDinner);
            MoveRecipeCommand           = new DelegateCommand<Guid>(MoveRecipe);
        }

        private async Task<WeekEdit?> GetWeekAsync(DateTime dayOfWeek)
        {
            Debug.WriteLine("MainPageViewModel.GetWeekAsync");
            var weekData = await WeekService.GetWeekAsync(dayOfWeek).ConfigureAwait(false);
            if (weekData == null)
            {
                return null;
            }

            var weekMain = mapper.Map<WeekEdit>(weekData);
            if (weekMain.Days != null)
            {
                foreach (var day in weekMain.Days)
                {
                    day.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == nameof(DayEdit.DinnerWasCooked))
                        {
                            if (sender is DayEdit dayChanged)
                            {
                                dayService.SetDinnerWasCooked(dayChanged.ID, dayChanged.DinnerWasCooked);
                            }
                        }
                    };
                }
            }
            
            return weekMain;
        }
        
        private void ShowRecipe(Guid recipeId)
        {
            Debug.WriteLine("MainPageViewModel.ShowRecipe"); 
            var parameters = new NavigationParameters()
            {
                { nameof(RecipeViewModel.Recipe), recipeId }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(RecipeView), parameters);
        }

        private async void SelectDinner(string dayName)
        {
            Debug.WriteLine("MainPageViewModel.SelectDinner");
            var viewModel = await dialogUtils.ShowCustomMessageAsync<RecipeSelect, RecipeSelectViewModel>().ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var dayOfWeek = WeekService.GetDayOfWeek(dayName);
                var day = CurrentWeek!.Days.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);

                if (day != null)
                {
                    await dayService.SetDinner(day.ID, viewModel.SelectedRecipeID).ConfigureAwait(false);
                }
                else
                {
                    await dayService.CreateDinner(CurrentWeek!.ID, viewModel.SelectedRecipeID, dayOfWeek).ConfigureAwait(false);
                }

                await ReloadCurrentWeek().ConfigureAwait(false);
            }
        }

        private async Task ReloadCurrentWeek()
        {
            CurrentWeek = await GetWeekAsync(CurrentWeek!.Start).ConfigureAwait(false);
        }

        private async Task OnLoadedAsync()
        {
            Debug.WriteLine("MainPageViewModel.OnLoadedAsync");
            await SetWeekByDay(DateTime.Now).ConfigureAwait(false);

            var dayOnPreviousWeek = WeekService.FirstDayOfWeek(DateTime.Now).AddDays(-1);
            var prevWeekFilled    = WeekService.IsWeekFilled(dayOnPreviousWeek);

            if (!prevWeekFilled)
            {
                // Нужно напомнить о рецептах на прошедшей неделе
                var result = await dialogUtils.DialogCoordinator.ShowMessageAsync(dialogUtils.ViewModel,
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
                await WeekService.MoveDayToNextWeek(CurrentWeek!.ID, dayId, selectedDay.WeekDay).ConfigureAwait(false);
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

        private void CreateShoppingList()
        {
            Debug.WriteLine("MainPageViewModel.CreateShoppingList");

            var allProducts = WeekService.GetWeekIngredients(CurrentWeek!.ID);
            var parameters = new NavigationParameters()
            {
                { nameof(ShoppingCartViewModel.List), allProducts }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(ShoppingCartView), parameters);
        }

        private async void DeleteDayAsync(Guid dayId)
        {
            Debug.WriteLine("MainPageViewModel.DeleteDayAsync");
            var result = await dialogUtils.DialogCoordinator.ShowMessageAsync(dialogUtils.ViewModel,
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
                await dayService.DeleteAsync(dayId).ConfigureAwait(false);
                await ReloadCurrentWeek().ConfigureAwait(false);
            }
        }

        private async void DeleteCurrentWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.DeleteCurrentWeekAsync");
            var result = await dialogUtils.DialogCoordinator.ShowMessageAsync(dialogUtils.ViewModel,
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
                await WeekService.DeleteWeekAsync(CurrentWeek!.ID).ConfigureAwait(false);
                CurrentWeek = null;
            }
        }

        private void CreateNewWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.CreateNewWeekAsync");

            var parameters = new NavigationParameters
            {
                { nameof(WeekSettingsViewModel.WeekStart), WeekStart }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(WeekSettings), parameters);
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var reloadWeek = navigationContext.Parameters[Consts.ReloadWeekParameter] as bool?;
            if (reloadWeek.HasValue && reloadWeek.Value)
            {
                CurrentWeek = await GetWeekAsync(WeekStart).ConfigureAwait(false);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
