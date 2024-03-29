﻿using System;
using System.Collections.ObjectModel;
using AutoMapper;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Events;
using Cooking.WPF.Services;
using Cooking.WPF.Validation;
using Cooking.WPF.Views;
using Fody;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using WPF.Commands;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// View model for recipe viewing and editing.
/// </summary>
[AddINotifyPropertyChangedInterface]
public partial class RecipeViewModel : INavigationAware, IRegionMemberLifetime
{
    private readonly DialogService dialogService;
    private readonly ImageService imageService;
    private readonly IContainerExtension container;
    private readonly RecipeService recipeService;
    private readonly IMapper mapper;
    private readonly IEventAggregator eventAggregator;
    private readonly ILocalization localization;
    private readonly IRegionManager regionManager;
    private IRegionNavigationJournal? journal;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecipeViewModel"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service dependency.</param>
    /// <param name="imageService">Service for working with images.</param>
    /// <param name="container">IoC container.</param>
    /// <param name="recipeService">Recipe service dependency.</param>
    /// <param name="mapper">Mapper dependency.</param>
    /// <param name="eventAggregator">Dependency on Prism event aggregator.</param>
    /// <param name="localization">Localization provider dependency.</param>
    /// <param name="regionManager">Region manager for Prism navigation.</param>
    public RecipeViewModel(DialogService dialogService,
                           ImageService imageService,
                           IContainerExtension container,
                           RecipeService recipeService,
                           IMapper mapper,
                           IEventAggregator eventAggregator,
                           ILocalization localization,
                           IRegionManager regionManager)
    {
        this.dialogService = dialogService;
        this.imageService = imageService;
        this.container = container;
        this.recipeService = recipeService;
        this.mapper = mapper;
        this.eventAggregator = eventAggregator;
        this.localization = localization;
        this.regionManager = regionManager;

        CloseCommand = new DelegateCommand(Close);
        ApplyChangesCommand = new AsyncDelegateCommand(ApplyChangesAsync, CanApplyChanges);
        DeleteRecipeCommand = new AsyncDelegateCommand<Guid>(DeleteRecipeAsync, canExecute: CanDeleteRecipe);

        ImageSearchCommand = new AsyncDelegateCommand(ImageSearchAsync);
        RemoveImageCommand = new DelegateCommand(RemoveImage, canExecute: CanRemoveImage);

        AddTagCommand = new AsyncDelegateCommand(AddTagAsync);
        RemoveTagCommand = new DelegateCommand<TagEdit>(RemoveTag);
        ViewTagCommand = new DelegateCommand<TagEdit>(ViewTag);

        AddIngredientGroupCommand = new AsyncDelegateCommand(AddIngredientGroupAsync);
        EditIngredientGroupCommand = new AsyncDelegateCommand<IngredientGroupEdit>(EditIngredientGroupAsync);
        AddIngredientToGroupCommand = new AsyncDelegateCommand<IngredientGroupEdit>(AddIngredientToGroupAsync);
        RemoveIngredientGroupCommand = new DelegateCommand<IngredientGroupEdit>(RemoveIngredientGroup);

        AddIngredientCommand = new AsyncDelegateCommand(AddIngredientAsync);
        EditIngredientCommand = new AsyncDelegateCommand<RecipeIngredientEdit>(EditIngredientAsync);
        RemoveIngredientCommand = new DelegateCommand<RecipeIngredientEdit>(RemoveIngredient);

        AddGarnishCommand = new AsyncDelegateCommand(AddGarnish);
        RemoveGarnishCommand = new DelegateCommand<RecipeEdit>(RemoveGarnish);
    }

    /// <summary>
    /// Gets or sets a value indicating whether vm is in creation mode.
    /// In creation mode recipe cannot be deleted or editing cannot be canceled. You can either save or cancel.
    /// </summary>
    public bool IsRecipeCreation { get; set; }

    /// <summary>
    /// Gets or sets recipe to be viewed or edited.
    /// </summary>
    [DoNotCheckEquality]
    public RecipeEdit? Recipe { get; set; }

    /// <summary>
    /// Gets command to apply changes to recipe.
    /// </summary>
    public AsyncDelegateCommand ApplyChangesCommand { get; }

    /// <summary>
    /// Gets close current view.
    /// </summary>
    public DelegateCommand CloseCommand { get; }

    /// <summary>
    /// Gets command to select image for a recipe.
    /// </summary>
    public AsyncDelegateCommand ImageSearchCommand { get; }

    /// <summary>
    /// Gets command to remove image for a recipe.
    /// </summary>
    public DelegateCommand RemoveImageCommand { get; }

    /// <summary>
    /// Gets command to add ingredient.
    /// </summary>
    public AsyncDelegateCommand AddIngredientCommand { get; }

    /// <summary>
    /// Gets command to add tag to a recipe.
    /// </summary>
    public AsyncDelegateCommand AddTagCommand { get; }

    /// <summary>
    /// Gets command to add garnish to a recipe.
    /// </summary>
    public AsyncDelegateCommand AddGarnishCommand { get; }

    /// <summary>
    /// Gets command to remove garnish from a recipe.
    /// </summary>
    public DelegateCommand<RecipeEdit> RemoveGarnishCommand { get; }

    /// <summary>
    /// Gets command to add ingredients group to a recipe.
    /// </summary>
    public AsyncDelegateCommand AddIngredientGroupCommand { get; }

    /// <summary>
    /// Gets command to remove tag from a recipe.
    /// </summary>
    public DelegateCommand<TagEdit> RemoveTagCommand { get; }

    /// <summary>
    /// Gets command to move to a list of recipies filtered by a tag.
    /// </summary>
    public DelegateCommand<TagEdit> ViewTagCommand { get; }

    /// <summary>
    /// Gets command to add ingredient to a group.
    /// </summary>
    public AsyncDelegateCommand<IngredientGroupEdit> AddIngredientToGroupCommand { get; }

    /// <summary>
    /// Gets command to remove ingredient from a group.
    /// </summary>
    public DelegateCommand<IngredientGroupEdit> RemoveIngredientGroupCommand { get; }

    /// <summary>
    /// Gets command to edit ingredient group.
    /// </summary>
    public AsyncDelegateCommand<IngredientGroupEdit> EditIngredientGroupCommand { get; }

    /// <summary>
    /// Gets command to edit ingredientin a recipe.
    /// </summary>
    public AsyncDelegateCommand<RecipeIngredientEdit> EditIngredientCommand { get; }

    /// <summary>
    /// Gets command to remove ingredient from a recipe.
    /// </summary>
    public DelegateCommand<RecipeIngredientEdit> RemoveIngredientCommand { get; }

    /// <summary>
    /// Gets command to delete current recipe.
    /// </summary>
    public AsyncDelegateCommand<Guid> DeleteRecipeCommand { get; }

    /// <summary>
    /// Gets or sets a value indicating whether current view model is in editing state.
    /// </summary>
    public bool IsEditing { get; set; }

    /// <inheritdoc/>
    public bool KeepAlive => true;

    /// <summary>
    /// Gets or sets backup of recipe for edit cancellation.
    /// </summary>
    public RecipeEdit? RecipeEdit { get; set; }

    /// <summary>
    /// Intercepted by PropertyChanged change of <see cref="IsEditing"/> property.
    /// Needed for move between edit states without saving using backup copy.
    /// </summary>
    public void OnIsEditingChanged()
    {
        if (IsEditing)
        {
            if (RecipeEdit == null)
            {
                RecipeEdit = mapper.Map<RecipeEdit>(Recipe);
            }
            else
            {
                mapper.Map(Recipe, RecipeEdit);
            }
        }
    }

    /// <inheritdoc/>
    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (Recipe != null)
        {
            return;
        }

        journal = navigationContext.NavigationService.Journal;
        var recipeID = navigationContext.Parameters[nameof(Recipe)] as Guid?;
        if (recipeID.HasValue)
        {
            Recipe = recipeService.GetMapped<RecipeEdit>(recipeID.Value);
        }
        else
        {
            Recipe = new RecipeEdit();
            IsEditing = true;
            IsRecipeCreation = true;
        }
    }

    /// <inheritdoc/>
    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        var recipeID = navigationContext.Parameters[nameof(Recipe)] as Guid?;
        return recipeID.HasValue && Recipe != null && recipeID == Recipe.ID;
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    private bool CanApplyChanges() => RecipeEdit!.IsValid();

    private bool CanDeleteRecipe(Guid arg) => !IsRecipeCreation;

    private void Close() => journal?.GoBack();

    private void RemoveIngredientGroup(IngredientGroupEdit arg) => RecipeEdit!.IngredientGroups.Remove(arg);

    private void RemoveIngredient(RecipeIngredientEdit ingredient)
    {
        if (RecipeEdit!.Ingredients.Contains(ingredient))
        {
            RecipeEdit.Ingredients.Remove(ingredient);
            return;
        }

        foreach (IngredientGroupEdit group in RecipeEdit.IngredientGroups)
        {
            if (group.Ingredients?.Contains(ingredient) == true)
            {
                group.Ingredients.Remove(ingredient);
                return;
            }
        }
    }

    private async Task EditIngredientAsync(RecipeIngredientEdit ingredient)
    {
        RecipeIngredientEditViewModel viewModel = container.Resolve<RecipeIngredientEditViewModel>();
        viewModel.Ingredient = ingredient;

        await dialogService.ShowCustomLocalizedMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("EditIngredient", viewModel);

        if (viewModel.DialogResultOk)
        {
            mapper.Map(viewModel.Ingredient, ingredient);
        }
    }

    [ConfigureAwait(true)]
    private async Task AddGarnish()
    {
        GarnishSelectViewModel viewModel = container.Resolve<GarnishSelectViewModel>((typeof(IEnumerable<RecipeEdit>), RecipeEdit!.Garnishes));
        await dialogService.ShowCustomLocalizedMessageAsync<GarnishSelectView, GarnishSelectViewModel>("AddGarnishes", viewModel);

        if (viewModel.DialogResultOk)
        {
            RecipeEdit!.Garnishes.Clear();
            RecipeEdit.Garnishes.AddRange(viewModel.SelectedItems);
        }
    }

    private void RemoveGarnish(RecipeEdit garnish)
    {
        RecipeEdit!.Garnishes.Remove(garnish);
    }

    private async Task AddTagAsync()
    {
        TagSelectViewModel viewModel = container.Resolve<TagSelectViewModel>(
            (typeof(IEnumerable<TagEdit>), RecipeEdit!.Tags)
        );
        await dialogService.ShowCustomLocalizedMessageAsync<TagSelectView, TagSelectViewModel>("AddTags", viewModel);

        if (viewModel.DialogResultOk)
        {
            RecipeEdit!.Tags = viewModel.SelectedItems;
        }
    }

    [ConfigureAwait(true)]
    private async Task AddIngredientGroupAsync()
    {
        var viewModel = new IngredientGroupEditViewModel(dialogService, new IngredientGroupEdit());
        await dialogService.ShowCustomLocalizedMessageAsync<IngredientGroupEditView, IngredientGroupEditViewModel>("AddIngredientsGroup", viewModel);

        if (viewModel.DialogResultOk)
        {
            RecipeEdit!.IngredientGroups.Add(viewModel.IngredientGroup);
        }
    }

    [ConfigureAwait(true)]
    private async Task EditIngredientGroupAsync(IngredientGroupEdit group)
    {
        var viewModel = new IngredientGroupEditViewModel(dialogService, group);
        await dialogService.ShowCustomLocalizedMessageAsync<IngredientGroupEditView, IngredientGroupEditViewModel>("EditIngredientsGroup", viewModel);

        if (viewModel.DialogResultOk)
        {
            mapper.Map(viewModel.IngredientGroup, group);
        }
    }

    private void ViewTag(TagEdit tag)
    {
        regionManager.NavigateMain(
            view: nameof(RecipeListView),
            parameters: (nameof(RecipeListViewModel.FilterText), $"{Consts.TagSymbol}\"{tag.Name}\""));
    }

    private void RemoveTag(TagEdit tag) => RecipeEdit!.Tags.Remove(tag);
    private void RemoveImage() => RecipeEdit!.ImagePath = null;

    private bool CanRemoveImage() => RecipeEdit?.ImagePath != null;

    [ConfigureAwait(true)]
    private async Task AddIngredientAsync()
    {
        RecipeIngredientEditViewModel viewModel = container.Resolve<RecipeIngredientEditViewModel>();
        viewModel.IsCreation = true;

        await dialogService.ShowCustomLocalizedMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("AddIngredient", viewModel);

        if (viewModel.DialogResultOk)
        {
            RecipeEdit!.Ingredients ??= new ObservableCollection<RecipeIngredientEdit>();

            if (viewModel.Ingredients != null)
            {
                foreach (RecipeIngredientEdit ingredient in viewModel.Ingredients)
                {
                    ingredient.Order += RecipeEdit.Ingredients.Count;
                    RecipeEdit.Ingredients.Add(ingredient);
                }
            }

            viewModel.Ingredient.Order = RecipeEdit.Ingredients.Count + 1;
            RecipeEdit.Ingredients.Add(viewModel.Ingredient);
        }
    }

    [ConfigureAwait(true)]
    private async Task AddIngredientToGroupAsync(IngredientGroupEdit group)
    {
        RecipeIngredientEditViewModel viewModel = container.Resolve<RecipeIngredientEditViewModel>();
        viewModel.IsCreation = true;

        await dialogService.ShowCustomLocalizedMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("AddIngredient", viewModel);

        if (viewModel.DialogResultOk)
        {
            group.Ingredients ??= new ObservableCollection<RecipeIngredientEdit>();

            if (viewModel.Ingredients != null)
            {
                foreach (RecipeIngredientEdit ingredient in viewModel.Ingredients)
                {
                    ingredient.Order += group.Ingredients.Count;
                    group.Ingredients.Add(ingredient);
                }
            }

            viewModel.Ingredient.Order = group.Ingredients.Count + 1;
            group.Ingredients.Add(viewModel.Ingredient);
        }
    }

    private Task ImageSearchAsync()
    {
        RecipeEdit!.ImagePath = imageService.UseImage();
        return Task.CompletedTask;
    }

    [ConfigureAwait(true)]
    private async Task ApplyChangesAsync()
    {
        if (RecipeEdit!.ID == Guid.Empty)
        {
            RecipeEdit.ID = await recipeService.CreateAsync(RecipeEdit);
            eventAggregator.GetEvent<RecipeCreatedEvent>().Publish(RecipeEdit);
        }
        else
        {
            await recipeService.UpdateAsync(RecipeEdit);
            eventAggregator.GetEvent<RecipeUpdatedEvent>().Publish(RecipeEdit);
        }

        IsEditing = false;
        IsRecipeCreation = false;

        mapper.Map(RecipeEdit, Recipe);
    }

    private async Task DeleteRecipeAsync(Guid recipeID) => await dialogService.ShowYesNoDialogAsync(localization.GetLocalizedString("SureDelete", RecipeEdit!.Name),
                                                                                                    localization["CannotUndo"],
                                                                                                    successCallback: async () => await OnRecipeDeletedAsync(recipeID));

    private async Task OnRecipeDeletedAsync(Guid recipeID)
    {
        // Journal methods must be called from UI thread
        await recipeService.DeleteAsync(recipeID).ConfigureAwait(continueOnCapturedContext: true);
        eventAggregator.GetEvent<RecipeDeletedEvent>().Publish(recipeID);

        journal?.GoBack();
    }
}
