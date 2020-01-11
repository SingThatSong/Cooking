using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Cooking.WPF.Helpers;
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
        // Dependencies
        private readonly DialogService dialogUtils;
        private readonly DayService dayService;
        private readonly IRegionManager regionManager;
        private readonly IContainerExtension container;
        private readonly IMapper mapper;
        private readonly WeekService weekService;
        private readonly ILocalization localization;

        // State
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public bool WeekEdit { get; set; }
        public WeekEdit? CurrentWeek { get; set; }

        // Commands
        public AsyncDelegateCommand LoadedCommand { get; }
        public DelegateCommand CreateShoppingListCommand { get; }
        public DelegateCommand CreateNewWeekCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand SelectNextWeekCommand { get; }
        public DelegateCommand SelectPreviousWeekCommand { get; }
        public DelegateCommand<Guid> ShowRecipeCommand { get; }
        public DelegateCommand<Guid> MoveRecipeCommand { get; }
        public DelegateCommand<DayOfWeek> SelectDinnerCommand { get; }
        public DelegateCommand<Guid> DeleteDinnerCommand { get; }

        public MainViewModel(DialogService dialogUtils, 
                             IRegionManager regionManager, 
                             IContainerExtension container,
                             DayService dayService,
                             IMapper mapper,
                             WeekService weekService,
                             ILocalization localization)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(regionManager != null);
            Debug.Assert(container != null);
            Debug.Assert(dayService != null);
            Debug.Assert(mapper != null);
            Debug.Assert(weekService != null);
            Debug.Assert(localization != null);
            Debug.WriteLine("MainPageViewModel.ctor");

            this.dialogUtils            = dialogUtils;
            this.regionManager          = regionManager;
            this.container              = container;
            this.dayService             = dayService;
            this.mapper                 = mapper;
            this.weekService            = weekService;
            this.localization           = localization;

            LoadedCommand               = new AsyncDelegateCommand(OnLoadedAsync, executeOnce: true);
            CreateNewWeekCommand        = new DelegateCommand(CreateNewWeekAsync);
            CreateShoppingListCommand   = new DelegateCommand(CreateShoppingList);
            DeleteCommand               = new DelegateCommand(DeleteCurrentWeekAsync);
            SelectNextWeekCommand       = new DelegateCommand(SelectNextWeekAsync);
            SelectPreviousWeekCommand   = new DelegateCommand(SelectPreviousWeekAsync);
            ShowRecipeCommand           = new DelegateCommand<Guid>(ShowRecipe);
            DeleteDinnerCommand         = new DelegateCommand<Guid>(DeleteDayAsync);
            SelectDinnerCommand         = new DelegateCommand<DayOfWeek>(SelectDinner);
            MoveRecipeCommand           = new DelegateCommand<Guid>(MoveRecipe);
        }

        private async Task<WeekEdit?> GetWeekAsync(DateTime dayOfWeek)
        {
            Debug.WriteLine("MainPageViewModel.GetWeekAsync");
            var weekData = await weekService.GetWeekAsync(dayOfWeek).ConfigureAwait(false);
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

        private async void SelectDinner(DayOfWeek dayOfWeek)
        {
            Debug.WriteLine("MainPageViewModel.SelectDinner");
            var viewModel = await dialogUtils.ShowCustomMessageAsync<RecipeSelect, RecipeSelectViewModel>().ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var day = CurrentWeek!.Days.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);

                if (day != null)
                {
                    await dayService.SetDinner(day.ID, viewModel.SelectedRecipeID!.Value).ConfigureAwait(false);
                }
                else
                {
                    await dayService.CreateDinner(CurrentWeek!.ID, viewModel.SelectedRecipeID!.Value, dayOfWeek).ConfigureAwait(false);
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

            var dayOnPreviousWeek = weekService.FirstDayOfWeek(DateTime.Now).AddDays(-1);
            var prevWeekFilled    = weekService.IsWeekFilled(dayOnPreviousWeek);

            if (!prevWeekFilled)
            {
                // Нужно напомнить о рецептах на прошедшей неделе
                await dialogUtils.ShowYesNoDialog(
                      localization.GetLocalizedString("ByTheWay"),
                      localization.GetLocalizedString("YouNeedToMoveRecipies"),
                      successCallback: () => SelectPreviousWeekCommand.Execute()
                ).ConfigureAwait(false);
            }
        }

        private async void MoveRecipe(Guid dayId)
        {
            Debug.WriteLine("MainPageViewModel.MoveRecipe");
            var viewModel = await dialogUtils.ShowCustomMessageAsync<MoveRecipe, MoveRecipeViewModel>().ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var selectedDay = viewModel.DaysOfWeek.Single(x => x.IsSelected);
                await weekService.MoveDayToNextWeek(CurrentWeek!.ID, dayId, selectedDay.WeekDay).ConfigureAwait(false);
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
            WeekStart = weekService.FirstDayOfWeek(date);
            WeekEnd = weekService.LastDayOfWeek(date);
        }

        private void CreateShoppingList()
        {
            Debug.WriteLine("MainPageViewModel.CreateShoppingList");

            var allProducts = weekService.GetWeekIngredients(CurrentWeek!.ID);
            var parameters = new NavigationParameters()
            {
                { nameof(ShoppingCartViewModel.List), allProducts }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(ShoppingCartView), parameters);
        }

        private async void DeleteDayAsync(Guid dayId)
        {
            Debug.WriteLine("MainPageViewModel.DeleteDayAsync");
            await dialogUtils.ShowYesNoDialog(
                  "Точно?",
                  "Удаляем день?",
                  successCallback: () => OnDayDeleted(dayId)).ConfigureAwait(false);
        }


        private async void DeleteCurrentWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.DeleteCurrentWeekAsync");
            await dialogUtils.ShowYesNoDialog(
                  "Точно?",
                  "Удаляем неделю?",
                  successCallback: OnCurrentWeekDeleted).ConfigureAwait(false);
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

        #region Callbacks
        private async void OnDayDeleted(Guid dayId)
        {
            await dayService.DeleteAsync(dayId).ConfigureAwait(false);
            await ReloadCurrentWeek().ConfigureAwait(false);
        }

        private async void OnCurrentWeekDeleted()
        {
            // call buisness function
            await weekService.DeleteAsync(CurrentWeek!.ID).ConfigureAwait(false);
            // update state
            CurrentWeek = null;
        }
        #endregion

        #region Navigation methods
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
        #endregion
    }
}
