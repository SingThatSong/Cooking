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
using System.Collections.ObjectModel;
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

            LoadedCommand = new AsyncDelegateCommand(OnLoadedAsync, executeOnce: true);
            CreateNewWeekCommand = new DelegateCommand(CreateNewWeek);
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
        /// Gets or sets week days.
        /// </summary>
        [AlsoNotifyFor(nameof(Monday), nameof(Tuesday), nameof(Wednesday), nameof(Thursday), nameof(Friday), nameof(Saturday), nameof(Sunday))]
        public ObservableCollection<DayEdit>? CurrentWeek { get; set; }

        // TODO: Get rid of these properties

        /// <summary>
        /// Gets monday.
        /// </summary>
        public DayEdit? Monday => CurrentWeek?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Monday);

        /// <summary>
        /// Gets tuesday.
        /// </summary>
        public DayEdit? Tuesday => CurrentWeek?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Tuesday);

        /// <summary>
        /// Gets wednesday.
        /// </summary>
        public DayEdit? Wednesday => CurrentWeek?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Wednesday);

        /// <summary>
        /// Gets thursday.
        /// </summary>
        public DayEdit? Thursday => CurrentWeek?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Thursday);

        /// <summary>
        /// Gets friday.
        /// </summary>
        public DayEdit? Friday => CurrentWeek?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Friday);

        /// <summary>
        /// Gets saturday.
        /// </summary>
        public DayEdit? Saturday => CurrentWeek?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Saturday);

        /// <summary>
        /// Gets sunday.
        /// </summary>
        public DayEdit? Sunday => CurrentWeek?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Sunday);

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
            if (reloadWeek == true)
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

        private async Task<ObservableCollection<DayEdit>?> GetWeekAsync(DateTime dayOfWeek)
        {
            Debug.WriteLine("MainPageViewModel.GetWeekAsync");
            List<Day>? weekDays = await dayService.GetWeekAsync(dayOfWeek);
            if (weekDays != null)
            {
                List<DayEdit> weekDaysEdit = mapper.Map<List<DayEdit>>(weekDays);

                foreach (DayEdit day in weekDaysEdit)
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

                return new ObservableCollection<DayEdit>(weekDaysEdit);
            }
            else
            {
                return null;
            }
        }

        private void ShowRecipe(Guid recipeId)
        {
            Debug.WriteLine("MainPageViewModel.ShowRecipe");

            regionManager.NavigateMain(
                  view: nameof(RecipeView),
                  parameters: (nameof(RecipeViewModel.Recipe), recipeId));
        }

        private async void SelectDinner(DayOfWeek dayOfWeek)
        {
            Debug.WriteLine("MainPageViewModel.SelectDinner");
            RecipeSelectViewModel viewModel = await dialogService.ShowCustomMessageAsync<RecipeSelect, RecipeSelectViewModel>();

            if (viewModel.DialogResultOk)
            {
                DayEdit? day = CurrentWeek!.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);

                if (day != null)
                {
                    await dayService.SetDinner(day.ID, viewModel.SelectedRecipe!.ID);
                }
                else
                {
                    await dayService.CreateDinner(WeekStart, viewModel.SelectedRecipe!.ID, dayOfWeek);
                }

                await ReloadCurrentWeek();
            }
        }

        private async Task ReloadCurrentWeek() => CurrentWeek = await GetWeekAsync(WeekStart);

        private async Task OnLoadedAsync()
        {
            Debug.WriteLine("MainPageViewModel.OnLoadedAsync");
            await SetWeekByDay(DateTime.Now);

            DateTime dayOnPreviousWeek = dayService.FirstDayOfWeek(DateTime.Now).AddDays(-1);
            bool prevWeekFilled = await dayService.IsWeekFilled(dayOnPreviousWeek);

            if (!prevWeekFilled)
            {
                // Reminder of recipies on a previous week
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
                await dayService.MoveDayToNextWeek(dayId, viewModel.SelectedDay!.Value);
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
            WeekStart = dayService.FirstDayOfWeek(date);
            WeekEnd = dayService.LastDayOfWeek(date);
        }

        private void CreateShoppingList()
        {
            Debug.WriteLine("MainPageViewModel.CreateShoppingList");

            List<ShoppingListIngredientsGroup> allProducts = dayService.GetWeekShoppingList(WeekStart, WeekEnd);

            ShoppingListIngredientsGroup? noCategoryGroup = allProducts.Find(x => x.IngredientGroupName == null);

            if (noCategoryGroup != null)
            {
                noCategoryGroup.IngredientGroupName = localization.GetLocalizedString("NoCategory");
            }

            regionManager.NavigateMain(
                  view: nameof(ShoppingCartView),
                  parameters: (nameof(ShoppingCartViewModel.List), allProducts));
        }

        private async void DeleteDayAsync(Guid? dayId)
        {
            if (dayId != null)
            {
                Debug.WriteLine("MainPageViewModel.DeleteDayAsync");
                DayOfWeek dayOfWeek = CurrentWeek!.Single(x => x.ID == dayId).DayOfWeek;
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

        private void CreateNewWeek()
        {
            Debug.WriteLine("MainPageViewModel.CreateNewWeek");

            regionManager.NavigateMain(
                  view: nameof(WeekSettingsView),
                  parameters: (nameof(WeekSettingsViewModel.WeekStart), WeekStart));
        }

        private async void OnDayDeleted(Guid dayId)
        {
            await dayService.DeleteAsync(dayId);
            await ReloadCurrentWeek();
        }

        private async void OnCurrentWeekDeleted()
        {
            // call buisness function
            await dayService.DeleteWeekAsync(WeekStart, WeekEnd);

            // update state
            CurrentWeek = null;
        }
    }
}
