using AutoMapper;
using Cooking.Data.Model.Plan;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.Views;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.ViewModels
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
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeekViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="regionManager">Region manager for Prism navigation.</param>
        /// <param name="container">IoC container.</param>
        /// <param name="dayService"><see cref="DayService"/> dependency.</param>
        /// <param name="mapper">Mapper dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        public WeekViewModel(DialogService dialogService,
                             IRegionManager regionManager,
                             IContainerExtension container,
                             DayService dayService,
                             IMapper mapper,
                             ILocalization localization)
        {
            Debug.WriteLine("MainPageViewModel.ctor");

            this.dialogService = dialogService;
            this.regionManager = regionManager;
            this.container = container;
            this.dayService = dayService;
            this.mapper = mapper;
            this.localization = localization;

            CreateNewWeekCommand = new DelegateCommand(CreateNewWeek);
            CreateShoppingListCommand = new DelegateCommand(CreateShoppingList);
            DeleteCommand = new AsyncDelegateCommand(DeleteCurrentWeekAsync);
            DeleteDinnerCommand = new AsyncDelegateCommand<Guid?>(DeleteDayAsync, canExecute: CanDeleteDay);
            LoadedCommand = new AsyncDelegateCommand(OnLoadedAsync, executeOnce: true);
            MoveRecipeCommand = new AsyncDelegateCommand<Guid>(MoveRecipeAsync);
            SelectDinnerCommand = new AsyncDelegateCommand<DayOfWeek>(SelectDinnerAsync);
            SelectNextWeekCommand = new AsyncDelegateCommand(SelectNextWeekAsync);
            SelectPreviousWeekCommand = new AsyncDelegateCommand(SelectPreviousWeekAsync);
            ShowRecipeCommand = new DelegateCommand<Guid>(ShowRecipe);
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
        /// Gets week days.
        /// </summary>
        public ObservableCollection<DayDisplay>? CurrentWeek { get; private set; }

        /// <summary>
        /// Gets localized caption for MoveRecipeToNextWeek.
        /// </summary>
        public string? MoveRecipeToNextWeekCaption => localization.GetLocalizedString("MoveRecipeToNextWeek");

        /// <summary>
        /// Gets localized caption for NewRecipe.
        /// </summary>
        public string? NewRecipeCaption => localization.GetLocalizedString("NewRecipe");

        /// <summary>
        /// Gets localized caption for Delete.
        /// </summary>
        public string? DeleteCaption => localization.GetLocalizedString("Delete");

        /// <summary>
        /// Gets localized caption for Replace.
        /// </summary>
        public string? ReplaceCaption => localization.GetLocalizedString("Replace");

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
        public AsyncDelegateCommand DeleteCommand { get; }

        /// <summary>
        /// Gets command to move to the next week.
        /// </summary>
        public AsyncDelegateCommand SelectNextWeekCommand { get; }

        /// <summary>
        /// Gets command to move to the previous week.
        /// </summary>
        public AsyncDelegateCommand SelectPreviousWeekCommand { get; }

        /// <summary>
        /// Gets command to show recipe's detail.
        /// </summary>
        public DelegateCommand<Guid> ShowRecipeCommand { get; }

        /// <summary>
        /// Gets command to move existing recipe to the next week.
        /// </summary>
        public AsyncDelegateCommand<Guid> MoveRecipeCommand { get; }

        /// <summary>
        /// Gets command to choose a recipe for a day.
        /// </summary>
        public AsyncDelegateCommand<DayOfWeek> SelectDinnerCommand { get; }

        /// <summary>
        /// Gets command to delete a day.
        /// </summary>
        public AsyncDelegateCommand<Guid?> DeleteDinnerCommand { get; }

        /// <inheritdoc/>
        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            bool? reloadWeek = navigationContext.Parameters[Consts.ReloadWeekParameter] as bool?;
            if (reloadWeek == true)
            {
                await ReloadCurrentWeekAsync();
            }
        }

        /// <inheritdoc/>
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        /// <inheritdoc/>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private async Task<ObservableCollection<DayDisplay>?> GetWeekAsync(DateTime dayOfWeek)
        {
            Debug.WriteLine("MainPageViewModel.GetWeekAsync");
            List<Day>? weekDays = await dayService.GetWeekAsync(dayOfWeek);
            if (weekDays != null)
            {
                List<DayDisplay> weekDaysEdit = mapper.Map<List<DayDisplay>>(weekDays);

                foreach (DayDisplay day in weekDaysEdit)
                {
                    day.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == nameof(DayDisplay.DinnerWasCooked))
                        {
                            if (sender is DayDisplay dayChanged)
                            {
                                dayService.SetDinnerWasCooked(dayChanged.ID, dayChanged.DinnerWasCooked);
                            }
                        }
                    };
                }

                return new ObservableCollection<DayDisplay>(weekDaysEdit);
            }
            else
            {
                return null;
            }
        }

        private void ShowRecipe(Guid recipeID)
        {
            Debug.WriteLine("MainPageViewModel.ShowRecipe");

            regionManager.NavigateMain(
                  view: nameof(RecipeView),
                  parameters: (nameof(RecipeViewModel.Recipe), recipeID));
        }

        private async Task SelectDinnerAsync(DayOfWeek dayOfWeek)
        {
            Debug.WriteLine("MainPageViewModel.SelectDinner");
            RecipeSelectViewModel viewModel = await dialogService.ShowCustomMessageAsync<RecipeSelectView, RecipeSelectViewModel>();

            if (viewModel.DialogResultOk)
            {
                DayDisplay? day = CurrentWeek!.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);

                if (day != null)
                {
                    await dayService.SetDinnerAsync(day.ID, viewModel.SelectedRecipe!.ID);
                }
                else
                {
                    await dayService.CreateDinnerAsync(WeekStart, viewModel.SelectedRecipe!.ID, dayOfWeek);
                }

                await ReloadCurrentWeekAsync();
            }
        }

        private async Task ReloadCurrentWeekAsync() => CurrentWeek = await GetWeekAsync(WeekStart);

        private async Task OnLoadedAsync()
        {
            Debug.WriteLine("MainPageViewModel.OnLoadedAsync");
            await SetWeekByDayAsync(DateTime.Now);

            DateTime dayOnPreviousWeek = dayService.FirstDayOfWeek(DateTime.Now).AddDays(-1);
            bool prevWeekFilled = await dayService.IsWeekFilledAsync(dayOnPreviousWeek);

            if (!prevWeekFilled)
            {
                // Reminder of recipies on a previous week
                await dialogService.ShowYesNoDialogAsync(
                      localization.GetLocalizedString("ByTheWay"),
                      localization.GetLocalizedString("YouNeedToMoveRecipies"),
                      successCallback: () => SelectPreviousWeekCommand.Execute()
                );
            }
        }

        private async Task MoveRecipeAsync(Guid dayID)
        {
            Debug.WriteLine("MainPageViewModel.MoveRecipe");
            MoveRecipeViewModel viewModel = await dialogService.ShowCustomMessageAsync<MoveRecipeView, MoveRecipeViewModel>();

            if (viewModel.DialogResultOk)
            {
                await dayService.MoveDayToNextWeekAsync(dayID, viewModel.SelectedDay!.Value);
                await ReloadCurrentWeekAsync();
            }
        }

        private async Task SelectPreviousWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.SelectPreviousWeekAsync");
            DateTime dayOnPreviousWeek = WeekStart.AddDays(-1);
            await SetWeekByDayAsync(dayOnPreviousWeek);
        }

        private async Task SelectNextWeekAsync()
        {
            Debug.WriteLine("MainPageViewModel.SelectNextWeekAsync");
            DateTime dayOnNextWeek = WeekEnd.AddDays(1);
            await SetWeekByDayAsync(dayOnNextWeek);
        }

        private async Task SetWeekByDayAsync(DateTime date)
        {
            Debug.WriteLine("MainPageViewModel.SetWeekByDay");
            CurrentWeek = await GetWeekAsync(date);
            WeekStart = dayService.FirstDayOfWeek(date);
            WeekEnd = dayService.LastDayOfWeek(date);
        }

        private void CreateShoppingList()
        {
            Debug.WriteLine("MainPageViewModel.CreateShoppingList");

            List<ShoppingListIngredientsGroup> allProducts = dayService.GetWeekShoppingList(WeekStart, WeekEnd, localization);

            ShoppingListIngredientsGroup? noCategoryGroup = allProducts.Find(x => x.IngredientGroupName == null);

            if (noCategoryGroup != null)
            {
                noCategoryGroup.IngredientGroupName = localization.GetLocalizedString("NoCategory");
            }

            regionManager.NavigateMain(
                  view: nameof(ShoppingCartView),
                  parameters: (nameof(ShoppingCartViewModel.List), allProducts));
        }

        private async Task DeleteDayAsync(Guid? dayID)
        {
            if (dayID != null)
            {
                Debug.WriteLine("MainPageViewModel.DeleteDayAsync");
                DayOfWeek dayOfWeek = CurrentWeek!.Single(x => x.ID == dayID).DayOfWeek;
                await dialogService.ShowYesNoDialogAsync(
                      localization.GetLocalizedString("SureDelete", localization.GetLocalizedString(dayOfWeek) ?? string.Empty),
                      localization.GetLocalizedString("CannotUndo"),
                      successCallback: () => OnDayDeletedAsync(dayID.Value));
            }
        }

        private bool CanDeleteDay(Guid? day) => day.HasValue;

        private async Task DeleteCurrentWeekAsync()
        {
            await dialogService.ShowYesNoDialogAsync(
                  localization.GetLocalizedString("SureDelete", localization.GetLocalizedString("Week") ?? string.Empty),
                  localization.GetLocalizedString("CannotUndo"),
                  successCallback: async () => await OnCurrentWeekDeletedAsync());
        }

        private void CreateNewWeek()
        {
            regionManager.NavigateMain(
                  view: nameof(WeekSettingsView),
                  parameters: (nameof(WeekSettingsViewModel.WeekStart), WeekStart));
        }

        private async Task OnDayDeletedAsync(Guid dayID)
        {
            await dayService.DeleteAsync(dayID);
            await ReloadCurrentWeekAsync();
        }

        private async Task OnCurrentWeekDeletedAsync()
        {
            // call buisness function
            await dayService.DeleteWeekAsync(WeekStart, WeekEnd);

            // update state
            CurrentWeek = null;
        }
    }
}
