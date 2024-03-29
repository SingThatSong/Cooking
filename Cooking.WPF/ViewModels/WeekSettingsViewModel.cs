﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AutoMapper;
using Cooking.Data.Model;
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
/// View model for week settings.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class WeekSettingsViewModel : INavigationAware
{
    private readonly DialogService dialogService;
    private readonly IRegionManager regionManager;
    private readonly CRUDService<Tag> tagService;
    private readonly IContainerExtension container;
    private readonly RecipeService recipeService;
    private readonly ILocalization localization;
    private readonly IMapper mapper;
    private NavigationContext? navigationContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeekSettingsViewModel"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service dependency.</param>
    /// <param name="regionManager">Region manager for Prism navigation.</param>
    /// <param name="tagService">Tag service dependency.</param>
    /// <param name="container">IoC container.</param>
    /// <param name="recipeService">Recipe service dependency.</param>
    /// <param name="localization">Localization provider dependency.</param>
    /// <param name="mapper">Mapper dependency.</param>
    public WeekSettingsViewModel(DialogService dialogService,
                                 IRegionManager regionManager,
                                 CRUDService<Tag> tagService,
                                 IContainerExtension container,
                                 RecipeService recipeService,
                                 ILocalization localization,
                                 IMapper mapper)
    {
        this.dialogService = dialogService;
        this.regionManager = regionManager;
        this.tagService = tagService;
        this.container = container;
        this.recipeService = recipeService;
        this.localization = localization;
        this.mapper = mapper;

        Days = new List<DayPlan>()
            {
                new DayPlan(),
                new DayPlan { DayName = localization["Monday_Short"],    DayOfWeek = DayOfWeek.Monday },
                new DayPlan { DayName = localization["Tuesday_Short"],   DayOfWeek = DayOfWeek.Tuesday },
                new DayPlan { DayName = localization["Wednesday_Short"], DayOfWeek = DayOfWeek.Wednesday },
                new DayPlan { DayName = localization["Thursday_Short"],  DayOfWeek = DayOfWeek.Thursday },
                new DayPlan { DayName = localization["Friday_Short"],    DayOfWeek = DayOfWeek.Friday },
                new DayPlan { DayName = localization["Saturday_Short"],  DayOfWeek = DayOfWeek.Saturday },
                new DayPlan { DayName = localization["Sunday_Short"],    DayOfWeek = DayOfWeek.Sunday }
            };

        Days[0].PropertyChanged += OnHeaderValueChanged;
        AddMainIngredientCommand = new AsyncDelegateCommand<DayPlan>(AddMainIngredientAsync);
        AddDishTypesCommand = new AsyncDelegateCommand<DayPlan>(AddDishTypesAsync);
        AddCalorieTypesCommand = new AsyncDelegateCommand<DayPlan>(AddCalorieTypesAsync);
        CloseCommand = new DelegateCommand(Close, CanClose);
        OkCommand = new DelegateCommand(Ok);
    }

    /// <summary>
    /// Gets first day of a week.
    /// </summary>
    public DateTime WeekStart { get; private set; }

    /// <summary>
    /// Gets settings for days of a week.
    /// </summary>
    public List<DayPlan> Days { get; }

    /// <summary>
    /// Gets command to select required main ingredients.
    /// </summary>
    public AsyncDelegateCommand<DayPlan> AddMainIngredientCommand { get; }

    /// <summary>
    /// Gets command to select dish types.
    /// </summary>
    public AsyncDelegateCommand<DayPlan> AddDishTypesCommand { get; }

    /// <summary>
    /// Gets command to add calorie types.
    /// </summary>
    public AsyncDelegateCommand<DayPlan> AddCalorieTypesCommand { get; }

    /// <summary>
    /// Gets command to proceed with selected settings.
    /// </summary>
    public DelegateCommand OkCommand { get; }

    /// <summary>
    /// Gets command to return to previous view.
    /// </summary>
    public DelegateCommand CloseCommand { get; }

    /// <inheritdoc/>
    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        this.navigationContext = navigationContext;
        WeekStart = (DateTime)navigationContext.Parameters[nameof(WeekStart)];
    }

    /// <inheritdoc/>
    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        bool returned = navigationContext.NavigationService.Journal.CurrentEntry.Uri.OriginalString == nameof(GeneratedWeekView);

        if (returned)
        {
            // if returned from GeneratedWeekView - return old view, othrewise return new
            return true;
        }
        else
        {
            // We started new week creation - cached view should be deleted from region
            UserControl? view = navigationContext.NavigationService.Region.Views.OfType<UserControl>().FirstOrDefault(x => x.DataContext == this);
            if (view != null)
            {
                navigationContext.NavigationService.Region.Remove(view);
            }

            return false;
        }
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    private void Ok()
    {
        IEnumerable<DayPlan> selectedDays = Days.Skip(1).Where(x => x.IsSelected);
        GenerateRecipies(selectedDays);

        regionManager.NavigateMain(
             view: nameof(GeneratedWeekView),
             (nameof(GeneratedWeekViewModel.Days), selectedDays),
             (nameof(GeneratedWeekViewModel.WeekStart), WeekStart));
    }

    private void GenerateRecipies(IEnumerable<DayPlan> selectedDays)
    {
        // Clear current values
        foreach (DayPlan day in selectedDays)
        {
            day.Recipe = null;
            day.RecipeAlternatives = null;
        }

        foreach (DayPlan day in selectedDays)
        {
            var requiredTags = new List<Guid>();

            if (!day.NeededDishTypes.Contains(TagEdit.Any))
            {
                requiredTags.AddRange(day.NeededDishTypes.Select(x => x.ID));
            }

            if (!day.NeededMainIngredients.Contains(TagEdit.Any))
            {
                requiredTags.AddRange(day.NeededMainIngredients.Select(x => x.ID));
            }

            var requiredCalorieTyoes = new List<CalorieType>();
            if (!day.CalorieTypes.Contains(CalorieTypeSelection.Any))
            {
                requiredCalorieTyoes.AddRange(day.CalorieTypes.Select(x => x.CalorieType));
            }

            day.RecipeAlternatives = recipeService.GetRecipiesByParametersProjected<DayPlanRecipe>(requiredTags, requiredCalorieTyoes, day.MaxComplexity, day.MinRating, day.OnlyNewRecipies);

            IEnumerable<DayPlanRecipe> selectedRecipies = selectedDays.Where(x => x.Recipe != null).Select(x => x.Recipe!);
            var recipiesNotSelectedYet = day.RecipeAlternatives.Where(x => !selectedRecipies.Any(selected => selected!.ID == x.ID)).ToList();

            day.Recipe = recipiesNotSelectedYet.Count > 0
                            ? recipiesNotSelectedYet.OrderByDescending(x => recipeService.DaysFromLasCook(x.ID)).First()
                            : null;

            if (day.Recipe != null)
            {
                day.Garnish = day.Recipe.Garnishes.RandomElement();
            }
        }
    }

    private bool CanClose() => navigationContext?.NavigationService.Journal.CanGoBack ?? false;
    private void Close() => navigationContext!.NavigationService.Journal.GoBack();

    private async Task AddCalorieTypesAsync(DayPlan day)
    {
        var viewModel = new CalorieTypeSelectViewModel(dialogService, localization, day.CalorieTypes);
        await dialogService.ShowCustomLocalizedMessageAsync<CalorieTypeSelectView, CalorieTypeSelectViewModel>("CalorieTyoes", viewModel);

        if (viewModel.DialogResultOk)
        {
            var list = viewModel.AllValues.Where(x => x.IsSelected).ToList();

            if (list.Any(x => x != CalorieTypeSelection.Any))
            {
                list.Remove(CalorieTypeSelection.Any);
            }
            else if (list.Count == 0)
            {
                list.Add(CalorieTypeSelection.Any);
            }

            day.CalorieTypes = new ObservableCollection<CalorieTypeSelection>(list);
        }
    }

    private async Task AddDishTypesAsync(DayPlan day)
    {
        ObservableCollection<TagEdit>? tags = await GetTagsAsync(TagType.DishType, day.NeededDishTypes);

        if (tags != null)
        {
            day.NeededDishTypes = tags;
        }
    }

    private async Task AddMainIngredientAsync(DayPlan day)
    {
        ObservableCollection<TagEdit>? tags = await GetTagsAsync(TagType.MainIngredient, day.NeededMainIngredients);

        if (tags != null)
        {
            day.NeededMainIngredients = tags;
        }
    }

    private async Task<ObservableCollection<TagEdit>?> GetTagsAsync(TagType type, ObservableCollection<TagEdit> current)
    {
        List<TagEdit> allTags = tagService.GetProjected<TagEdit>(x => x.Type == type);

        allTags.Insert(0, TagEdit.Any);
        allTags[0].IsChecked = false;
        allTags.ForEach(x => x.Type = type);

        TagSelectViewModel viewModel = container.Resolve<TagSelectViewModel>(
            (typeof(IEnumerable<TagEdit>), current),
            (typeof(IList<TagEdit>), allTags)
        );

        string header = string.Format(localization.CurrentCulture, localization["CategoriesOf"] ?? "{0}", localization[type]);
        await dialogService.ShowCustomMessageAsync<TagSelectView, TagSelectViewModel>(header, viewModel);

        if (viewModel.DialogResultOk)
        {
            ObservableCollection<TagEdit>? list = viewModel.SelectedItems;

            if (list != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (list.Any(x => x != TagEdit.Any))
                    {
                        list.Remove(TagEdit.Any);
                    }
                    else if (list.Count == 0)
                    {
                        list.Add(TagEdit.Any);
                    }
                });

                return list;
            }
        }

        return null;
    }

    [SuppressPropertyChangedWarnings]
    private void OnHeaderValueChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is DayPlan dayPlan)
        {
            switch (e.PropertyName)
            {
                case nameof(DayPlan.CalorieTypes):
                    Days.ForEach(x => x.CalorieTypes = dayPlan.CalorieTypes);
                    break;
                case nameof(DayPlan.IsSelected):
                    Days.ForEach(x => x.IsSelected = dayPlan.IsSelected);
                    break;
                case nameof(DayPlan.MaxComplexity):
                    Days.ForEach(x => x.MaxComplexity = dayPlan.MaxComplexity);
                    break;
                case nameof(DayPlan.MinRating):
                    Days.ForEach(x => x.MinRating = dayPlan.MinRating);
                    break;
                case nameof(DayPlan.NeededDishTypes):
                    Days.ForEach(x => x.NeededDishTypes = dayPlan.NeededDishTypes);
                    break;
                case nameof(DayPlan.NeededMainIngredients):
                    Days.ForEach(x => x.NeededMainIngredients = dayPlan.NeededMainIngredients);
                    break;
                case nameof(DayPlan.OnlyNewRecipies):
                    Days.ForEach(x => x.OnlyNewRecipies = dayPlan.OnlyNewRecipies);
                    break;
            }
        }
    }
}
