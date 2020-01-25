using AutoMapper;
using Cooking.Data.Model.Plan;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// View model for viewing week schedule.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class WeekViewModel : INavigationAware
    {
        // Dependencies
        private readonly DialogService dialogService;
        private readonly DayService dayService;
        private readonly IRegionManager regionManager;
        private readonly IContainerExtension container;
        private readonly IMapper mapper;
        private readonly WeekService weekService;
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeekViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="regionManager">Region manager for Prism navigation.</param>
        /// <param name="container">IoC container.</param>
        /// <param name="dayService"><see cref="DayService"/> dependency.</param>
        /// <param name="mapper">Mapper dependency.</param>
        /// <param name="weekService">Week service dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        public WeekViewModel(DialogService dialogService,
                             IRegionManager regionManager,
                             IContainerExtension container,
                             DayService dayService,
                             IMapper mapper,
                             WeekService weekService,
                             ILocalization localization)
        {
            Debug.WriteLine("MainPageViewModel.ctor");

            this.dialogService = dialogService;
            this.regionManager = regionManager;
            this.container = container;
            this.dayService = dayService;
            this.mapper = mapper;
            this.weekService = weekService;
            this.localization = localization;

            LoadedCommand = new AsyncDelegateCommand(OnLoadedAsync, executeOnce: true);
            CreateNewWeekCommand = new DelegateCommand(CreateNewWeekAsync);
            CreateShoppingListCommand = new DelegateCommand(CreateShoppingList);
            DeleteCommand = new DelegateCommand(DeleteCurrentWeekAsync);
            SelectNextWeekCommand = new DelegateCommand(SelectNextWeekAsync);
            SelectPreviousWeekCommand = new DelegateCommand(SelectPreviousWeekAsync);
            ShowRecipeCommand = new DelegateCommand<Guid>(ShowRecipe);
            DeleteDinnerCommand = new DelegateCommand<Guid?>(DeleteDayAsync, canExecute: CanDeleteDay);
            SelectDinnerCommand = new DelegateCommand<DayOfWeek>(SelectDinner);
            MoveRecipeCommand = new DelegateCommand<Guid>(MoveRecipe);
        }

        /// <summary>
        /// Gets or sets first day of a week.
        /// </summary>
        public DateTime WeekStart { get; set; }

        /// <summary>
        /// Gets or sets last day of a week.
        /// </summary>
        public DateTime WeekEnd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether view model is in editing mode.
        /// </summary>
        public bool WeekEdit { get; set; }

        /// <summary>
        /// Gets or sets current week.
        /// </summary>
        public WeekEdit? CurrentWeek { get; set; }

        /// <summary>
        /// Gets localized caption for MoveRecipeToNextWeek.
        /// </summary>
        public string? MoveRecipeToNextWeekCaption => localization.GetLocalizedString("MoveRecipeToNextWeek");

        /// <summary>
        /// Gets localized caption for NewRecipe.
        /// </summary>
        public string? NewRecipeCaption => localization.GetLocalizedString("NewRecipe");

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public AsyncDelegateCommand LoadedCommand { get; }

        /// <summary>
        /// Gets command for showing shopping list for a week.
        /// </summary>
        public DelegateCommand CreateShoppingListCommand { get; }

        /// <summary>
        /// Gets command to generate new week schedule.
        /// </summary>
        public DelegateCommand CreateNewWeekCommand { get; }

        /// <summary>
        /// Gets command to delete current week.
        /// </summary>
        public DelegateCommand DeleteCommand { get; }

        /// <summary>
        /// Gets command to move to the next week.
        /// </summary>
        public DelegateCommand SelectNextWeekCommand { get; }

        /// <summary>
        /// Gets command to move to the previous week.
        /// </summary>
        public DelegateCommand SelectPreviousWeekCommand { get; }

        /// <summary>
        /// Gets command to show recipe's detail.
        /// </summary>
        public DelegateCommand<Guid> ShowRecipeCommand { get; }

        /// <summary>
        /// Gets command to move existing recipe to the next week.
        /// </summary>
        public DelegateCommand<Guid> MoveRecipeCommand { get; }

        /// <summary>
        /// Gets command to choose a recipe for a day.
        /// </summary>
        public DelegateCommand<DayOfWeek> SelectDinnerCommand { get; }

        /// <summary>
        /// Gets command to delete a day.
        /// </summary>
        public DelegateCommand<Guid?> DeleteDinnerCommand { get; }

        /// <inheritdoc/>
        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            bool? reloadWeek = navigationContext.Parameters[Consts.ReloadWeekParameter] as bool?;
            if (reloadWeek.HasValue && reloadWeek.Value)
            {
                CurrentWeek = await GetWeekAsync(WeekStart);
            }
        }

        /// <inheritdoc/>
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        /// <inheritdoc/>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private async Task<WeekEdit?> GetWeekAsync(DateTime dayOfWeek)
        {
            Debug.WriteLine("MainPageViewModel.GetWeekAsync");
            Week? weekData = await weekService.GetWeekAsync(dayOfWeek);
            if (weekData == null)
            {
                return null;
            }

            WeekEdit weekMain = mapper.Map<WeekEdit>(weekData);
            if (weekMain.Days != null)
            {
                foreach (DayEdit day in weekMain.Days)
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
            RecipeSelectViewModel viewModel = await dialogService.ShowCustomMessageAsync<RecipeSelect, RecipeSelectViewModel>();

            if (viewModel.DialogResultOk)
            {
                DayEdit day = CurrentWeek!.Days.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);

                if (day != null)
                {
                    await dayService.SetDinner(day.ID, viewModel.SelectedRecipe!.ID);
                }
                else
                {
                    await dayService.CreateDinner(CurrentWeek!.ID, viewModel.SelectedRecipe!.ID, dayOfWeek);
                }

                await ReloadCurrentWeek();
            }
        }

        private async Task ReloadCurrentWeek() => CurrentWeek = await GetWeekAsync(CurrentWeek!.Start);

        private async Task OnLoadedAsync()
        {
            Debug.WriteLine("MainPageViewModel.OnLoadedAsync");
            await SetWeekByDay(DateTime.Now);

            DateTime dayOnPreviousWeek = weekService.FirstDayOfWeek(DateTime.Now).AddDays(-1);
            bool prevWeekFilled = weekService.IsWeekFilled(dayOnPreviousWeek);

            if (!prevWeekFilled)
            {
                // Нужно напомнить о рецептах на прошедшей неделе
                await dialogService.ShowYesNoDialog(
                      localization.GetLocalizedString("ByTheWay"),
                      localization.GetLocalizedString("YouNeedToMoveRecipies"),
                      successCallback: () => SelectPreviousWeekCommand.Execute()
                );
            }
        }

        private async void MoveRecipe(Guid dayId)
        {
            Debug.WriteLine("MainPageViewModel.MoveRecipe");
            MoveRecipeViewModel viewModel = await dialogService.ShowCustomMessageAsync<MoveRecipeView, MoveRecipeViewModel>();

            if (viewModel.DialogResultOk)
            {
                await weekService.MoveDayToNextWeek(CurrentWeek!.ID, dayId, viewModel.SelectedDay!.Value);
                await ReloadCurrentWeek();
            }
        }

        private async void SelectPreviousWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.SelectPreviousWeekAsync");
            DateTime dayOnPreviousWeek = WeekStart.AddDays(-1);
            await SetWeekByDay(dayOnPreviousWeek);
        }

        private async void SelectNextWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.SelectNextWeekAsync");
            DateTime dayOnNextWeek = WeekEnd.AddDays(1);
            await SetWeekByDay(dayOnNextWeek);
        }

        private async Task SetWeekByDay(DateTime date)
        {
            Debug.WriteLine("MainPageViewModel.SetWeekByDay");
            CurrentWeek = await GetWeekAsync(date);
            WeekStart = weekService.FirstDayOfWeek(date);
            WeekEnd = weekService.LastDayOfWeek(date);
        }

        private void CreateShoppingList()
        {
            Debug.WriteLine("MainPageViewModel.CreateShoppingList");

            List<ShoppingListIngredientsGroup> allProducts = weekService.GetWeekShoppingList(CurrentWeek!.ID);
            var parameters = new NavigationParameters()
            {
                { nameof(ShoppingCartViewModel.List), allProducts }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(ShoppingCartView), parameters);
        }

        private async void DeleteDayAsync(Guid? dayId)
        {
            if (dayId != null)
            {
                Debug.WriteLine("MainPageViewModel.DeleteDayAsync");
                DayOfWeek dayOfWeek = CurrentWeek!.Days.Single(x => x.ID == dayId).DayOfWeek;
                await dialogService.ShowYesNoDialog(
                      localization.GetLocalizedString("SureDelete", localization.GetLocalizedString(dayOfWeek) ?? string.Empty),
                      localization.GetLocalizedString("CannotUndo"),
                      successCallback: () => OnDayDeleted(dayId.Value));
            }
        }

        private bool CanDeleteDay(Guid? day) => day.HasValue;
        private async void DeleteCurrentWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.DeleteCurrentWeekAsync");
            await dialogService.ShowYesNoDialog(
                  localization.GetLocalizedString("SureDelete", localization.GetLocalizedString("Week") ?? string.Empty),
                  localization.GetLocalizedString("CannotUndo"),
                  successCallback: OnCurrentWeekDeleted);
        }

        private void CreateNewWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.CreateNewWeekAsync");

            var parameters = new NavigationParameters
            {
                { nameof(WeekSettingsViewModel.WeekStart), WeekStart }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(WeekSettingsView), parameters);
        }

        private async void OnDayDeleted(Guid dayId)
        {
            await dayService.DeleteAsync(dayId);
            await ReloadCurrentWeek();
        }

        private async void OnCurrentWeekDeleted()
        {
            // call buisness function
            await weekService.DeleteAsync(CurrentWeek!.ID);

            // update state
            CurrentWeek = null;
        }
    }
}
