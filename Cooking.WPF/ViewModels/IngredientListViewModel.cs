﻿using System;
using System.Collections.ObjectModel;
using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Events;
using Cooking.WPF.Services;
using Cooking.WPF.Views;
using Prism.Events;
using Prism.Regions;
using PropertyChanged;
using WPF.Commands;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// View model for ingredients list.
/// </summary>
[AddINotifyPropertyChangedInterface]
public partial class IngredientListViewModel
{
    // Dependencies
    private readonly IRegionManager regionManager;
    private readonly DialogService dialogService;
    private readonly CRUDService<Ingredient> ingredientService;
    private readonly IEventAggregator eventAggregator;
    private readonly IMapper mapper;
    private readonly ILocalization localization;

    /// <summary>
    /// Initializes a new instance of the <see cref="IngredientListViewModel"/> class.
    /// </summary>
    /// <param name="regionManager">Region manager for Prism navigation.</param>
    /// <param name="dialogService">Dialog service dependency.</param>
    /// <param name="ingredientService">Ingredient service dependency.</param>
    /// <param name="eventAggregator">Prism event aggregator dependency.</param>
    /// <param name="mapper">Mapper dependency.</param>
    /// <param name="localization">Localization provider dependency.</param>
    public IngredientListViewModel(IRegionManager regionManager,
                                   DialogService dialogService,
                                   CRUDService<Ingredient> ingredientService,
                                   IEventAggregator eventAggregator,
                                   IMapper mapper,
                                   ILocalization localization)
    {
        this.regionManager = regionManager;
        this.dialogService = dialogService;
        this.ingredientService = ingredientService;
        this.eventAggregator = eventAggregator;
        this.mapper = mapper;
        this.localization = localization;
        LoadedCommand = new AsyncDelegateCommand(OnLoaded, executeOnce: true);
        AddIngredientCommand = new AsyncDelegateCommand(AddIngredientAsync);
        EditIngredientCommand = new AsyncDelegateCommand<IngredientEdit>(EditIngredientAsync);
        ViewIngredientCommand = new DelegateCommand<IngredientEdit>(ViewIngredient);

        eventAggregator.GetEvent<IngredientDeletedEvent>().Subscribe(OnIngredientDeleted);
    }

    /// <summary>
    /// Gets list of all ingredients.
    /// </summary>
    public ObservableCollection<IngredientEdit>? Ingredients { get; private set; }

    /// <summary>
    /// Gets command to execute on loaded event.
    /// </summary>
    public AsyncDelegateCommand LoadedCommand { get; }

    /// <summary>
    /// Gets command to create new ingredient.
    /// </summary>
    public AsyncDelegateCommand AddIngredientCommand { get; }

    /// <summary>
    /// Gets command to view ingredient.
    /// </summary>
    public DelegateCommand<IngredientEdit> ViewIngredientCommand { get; }

    /// <summary>
    /// Gets command to edit ingredient.
    /// </summary>
    public AsyncDelegateCommand<IngredientEdit> EditIngredientCommand { get; }

    private Task OnLoaded()
    {
        List<IngredientEdit> dataDb = ingredientService.GetProjected<IngredientEdit>();
        Ingredients = new ObservableCollection<IngredientEdit>(dataDb);

        return Task.CompletedTask;
    }

    private void ViewIngredient(IngredientEdit ingredient)
    {
        regionManager.NavigateMain(
            view: nameof(RecipeListView),
            parameters: (nameof(RecipeListViewModel.FilterText), $"{Consts.IngredientSymbol}\"{ingredient.Name}\""));
    }

    private async Task AddIngredientAsync() => await dialogService.ShowOkCancelDialogAsync<IngredientEditView, IngredientEditViewModel>(localization["NewIngredient"],
                                                                                                                                        successCallback: async viewModel => await OnNewIngredientCreated(viewModel));

    private async Task EditIngredientAsync(IngredientEdit ingredient)
    {
        var viewModel = new IngredientEditViewModel(ingredientService, dialogService, localization, eventAggregator, mapper.Map<IngredientEdit>(ingredient));
        await dialogService.ShowOkCancelDialogAsync<IngredientEditView, IngredientEditViewModel>(localization["EditIngredient"],
                                                                                                 viewModel,
                                                                                                 successCallback: async viewModel => await OnIngredientEditedAsync(viewModel));
    }

    private async Task OnIngredientEditedAsync(IngredientEditViewModel viewModel)
    {
        await ingredientService.UpdateAsync(viewModel.Ingredient);
        IngredientEdit? existing = Ingredients?.Single(x => x.ID == viewModel.Ingredient.ID);
        if (existing != null)
        {
            mapper.Map(viewModel.Ingredient, existing);
        }
    }

    private async Task OnNewIngredientCreated(IngredientEditViewModel viewModel)
    {
        await ingredientService.CreateAsync(viewModel.Ingredient);
        Ingredients!.Add(viewModel.Ingredient);
    }

    private void OnIngredientDeleted(Guid id)
    {
        IngredientEdit item = Ingredients!.First(x => x.ID == id);
        Ingredients!.Remove(item);
    }
}
