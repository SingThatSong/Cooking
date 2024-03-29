﻿using System;
using System.Collections.ObjectModel;
using AutoMapper;
using Cooking.Data.Model.Plan;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.Views;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using WPF.Commands;

namespace Cooking.WPF.ViewModels;

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
    private readonly AppSettings options;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeekViewModel"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service dependency.</param>
    /// <param name="regionManager">Region manager for Prism navigation.</param>
    /// <param name="container">IoC container.</param>
    /// <param name="dayService"><see cref="DayService"/> dependency.</param>
    /// <param name="mapper">Mapper dependency.</param>
    /// <param name="localization">Localization provider dependency.</param>
    /// <param name="options">Gets current application's settings.</param>
    public WeekViewModel(DialogService dialogService,
                         IRegionManager regionManager,
                         IContainerExtension container,
                         DayService dayService,
                         IMapper mapper,
                         ILocalization localization,
                         AppSettings options)
    {
        this.dialogService = dialogService;
        this.regionManager = regionManager;
        this.container = container;
        this.dayService = dayService;
        this.mapper = mapper;
        this.localization = localization;
        this.options = options;
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
    /// Gets week days.
    /// </summary>
    public ObservableCollection<DayDisplay>? CurrentWeek { get; private set; }

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
        List<Day>? weekDays = await dayService.GetWeekAsync(dayOfWeek);
        if (weekDays != null)
        {
            List<DayDisplay> weekDaysEdit = mapper.Map<List<DayDisplay>>(weekDays);

            foreach (DayDisplay day in weekDaysEdit)
            {
                day.PropertyChanged += async (sender, e) =>
                {
                    if (e.PropertyName == nameof(DayDisplay.DinnerWasCooked) && sender is DayDisplay dayChanged)
                    {
                        await dayService.SetDinnerWasCookedAsync(dayChanged.ID, dayChanged.DinnerWasCooked);
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
        regionManager.NavigateMain(
              view: nameof(RecipeView),
              parameters: (nameof(RecipeViewModel.Recipe), recipeID));
    }

    private async Task SelectDinnerAsync(DayOfWeek dayOfWeek)
    {
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
        await SetWeekByDayAsync(DateTime.Now);

        if (options.ShowLastWeekSuggestion)
        {
            DateTime dayOnPreviousWeek = dayService.FirstDayOfWeek(DateTime.Now).AddDays(-1);
            bool prevWeekFilled = await dayService.IsWeekFilledAsync(dayOnPreviousWeek);

            if (!prevWeekFilled)
            {
                // Reminder of recipies on a previous week
                await dialogService.ShowLocalizedYesNoDialogAsync(
                      "ByTheWay",
                      "YouNeedToMoveRecipies",
                      successCallback: () => SelectPreviousWeekCommand.Execute()
                );
            }
        }
    }

    private async Task MoveRecipeAsync(Guid dayID)
    {
        MoveRecipeViewModel viewModel = await dialogService.ShowCustomMessageAsync<MoveRecipeView, MoveRecipeViewModel>();

        if (viewModel.DialogResultOk)
        {
            await dayService.MoveDayToNextWeekAsync(dayID, viewModel.SelectedDay!.Value);
            await ReloadCurrentWeekAsync();
        }
    }

    private async Task SelectPreviousWeekAsync()
    {
        DateTime dayOnPreviousWeek = WeekStart.AddDays(-1);
        await SetWeekByDayAsync(dayOnPreviousWeek);
    }

    private async Task SelectNextWeekAsync()
    {
        DateTime dayOnNextWeek = WeekEnd.AddDays(1);
        await SetWeekByDayAsync(dayOnNextWeek);
    }

    private async Task SetWeekByDayAsync(DateTime date)
    {
        CurrentWeek = await GetWeekAsync(date);
        WeekStart = dayService.FirstDayOfWeek(date);
        WeekEnd = dayService.LastDayOfWeek(date);
    }

    private void CreateShoppingList()
    {
        List<ShoppingListIngredientsGroup> allProducts = dayService.GetWeekShoppingList(WeekStart, WeekEnd);

        ShoppingListIngredientsGroup? noCategoryGroup = allProducts.Find(x => x.IngredientGroupName == null);

        if (noCategoryGroup != null)
        {
            noCategoryGroup.IngredientGroupName = localization["NoCategory"];
        }

        regionManager.NavigateMain(
              view: nameof(ShoppingCartView),
              parameters: (nameof(ShoppingCartViewModel.List), allProducts));
    }

    private async Task DeleteDayAsync(Guid? dayID)
    {
        if (dayID != null)
        {
            DayOfWeek dayOfWeek = CurrentWeek!.Single(x => x.ID == dayID).DayOfWeek;
            await dialogService.ShowYesNoDialogAsync(
                  localization.GetLocalizedString("SureDelete", localization[dayOfWeek] ?? string.Empty),
                  localization["CannotUndo"],
                  successCallback: async () => await OnDayDeletedAsync(dayID.Value));
        }
    }

    private bool CanDeleteDay(Guid? day) => day.HasValue;

    private async Task DeleteCurrentWeekAsync()
    {
        await dialogService.ShowYesNoDialogAsync(
              localization.GetLocalizedString("SureDelete", localization["Week"] ?? string.Empty),
              localization["CannotUndo"],
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
        await dayService.DeleteWeekAsync(WeekStart);

        // update state
        CurrentWeek = null;
    }
}
