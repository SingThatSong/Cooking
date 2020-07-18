using AutoMapper;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Events;
using Cooking.WPF.Services;
using Cooking.WPF.Views;
using GongSolutions.Wpf.DragDrop;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for recipe viewing and editing.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class RecipeViewModel : IDropTarget, INavigationAware, IRegionMemberLifetime
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
            ApplyChangesCommand = new AsyncDelegateCommand(ApplyChanges);
            DeleteRecipeCommand = new AsyncDelegateCommand<Guid>(DeleteRecipe, canExecute: CanDeleteRecipe);

            ImageSearchCommand = new AsyncDelegateCommand(ImageSearch);
            RemoveImageCommand = new DelegateCommand(RemoveImage, canExecute: CanRemoveImage);

            AddTagCommand = new DelegateCommand(AddTag);
            RemoveTagCommand = new DelegateCommand<TagEdit>(RemoveTag);
            ViewTagCommand = new DelegateCommand<TagEdit>(ViewTag);

            AddIngredientGroupCommand = new DelegateCommand(AddIngredientGroup);
            EditIngredientGroupCommand = new AsyncDelegateCommand<IngredientGroupEdit>(EditIngredientGroup);
            AddIngredientToGroupCommand = new DelegateCommand<IngredientGroupEdit>(AddIngredientToGroup);
            RemoveIngredientGroupCommand = new DelegateCommand<IngredientGroupEdit>(RemoveIngredientGroup);

            AddIngredientCommand = new DelegateCommand(AddIngredient);
            EditIngredientCommand = new AsyncDelegateCommand<RecipeIngredientEdit>(EditIngredient);
            RemoveIngredientCommand = new DelegateCommand<RecipeIngredientEdit>(RemoveIngredient);
        }

        /// <summary>
        /// Gets or sets a value indicating whether vm is in creation mode.
        /// In creation mode recipe cannot be deleted or editing cannot be canceled. You can either save or cancel.
        /// </summary>
        public bool IsRecipeCreation { get; set; }

        /// <summary>
        /// Gets or sets recipe to be viewed or edited.
        /// </summary>
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
        public DelegateCommand AddIngredientCommand { get; }

        /// <summary>
        /// Gets command to add tag to a recipe.
        /// </summary>
        public DelegateCommand AddTagCommand { get; }

        /// <summary>
        /// Gets command to add ingredients group to a recipe.
        /// </summary>
        public DelegateCommand AddIngredientGroupCommand { get; }

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
        public DelegateCommand<IngredientGroupEdit> AddIngredientToGroupCommand { get; }

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
        public bool KeepAlive => false;

        /// <summary>
        /// Gets or sets backup of recipe for edit cancellation.
        /// </summary>
        private RecipeEdit? RecipeBackup { get; set; }

        /// <summary>
        /// Intercepted by PropertyChanged change of <see cref="IsEditing"/> property.
        /// Needed for move between edit states without saving using backup copy.
        /// </summary>
        public void OnIsEditingChanged()
        {
            if (IsEditing)
            {
                RecipeBackup = mapper.Map<RecipeEdit>(Recipe);
            }
            else if (RecipeBackup != null)
            {
                Recipe = mapper.Map<RecipeEdit>(RecipeBackup);
            }
        }

        /// <inheritdoc/>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            journal = navigationContext.NavigationService.Journal;
            var recipeId = navigationContext.Parameters[nameof(Recipe)] as Guid?;
            if (recipeId.HasValue)
            {
                Recipe = recipeService.GetMapped<RecipeEdit>(recipeId.Value);
            }
            else
            {
                Recipe = new RecipeEdit();
                IsEditing = true;
                IsRecipeCreation = true;
            }
        }

        /// <inheritdoc/>
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        /// <inheritdoc/>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private bool CanDeleteRecipe(Guid arg) => !IsRecipeCreation;

        private void Close() => journal?.GoBack();

        private void RemoveIngredientGroup(IngredientGroupEdit arg) => Recipe!.IngredientGroups!.Remove(arg);

        private void RemoveIngredient(RecipeIngredientEdit ingredient)
        {
            if (Recipe!.Ingredients != null && Recipe.Ingredients.Contains(ingredient))
            {
                Recipe.Ingredients.Remove(ingredient);
                return;
            }

            if (Recipe.IngredientGroups != null)
            {
                foreach (IngredientGroupEdit group in Recipe.IngredientGroups)
                {
                    if (group.Ingredients?.Contains(ingredient) == true)
                    {
                        group.Ingredients.Remove(ingredient);
                        return;
                    }
                }
            }
        }

        private async Task EditIngredient(RecipeIngredientEdit ingredient)
        {
            RecipeIngredientEditViewModel viewModel = container.Resolve<RecipeIngredientEditViewModel>();
            viewModel.Ingredient = ingredient;

            await dialogService.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>(localization.GetLocalizedString("EditIngredient"), viewModel);

            if (viewModel.DialogResultOk)
            {
                mapper.Map(viewModel.Ingredient, ingredient);
            }
        }

        private async void AddTag()
        {
            TagSelectViewModel viewModel = container.Resolve<TagSelectViewModel>();
            viewModel.SetTags(Recipe!.Tags, null);
            await dialogService.ShowCustomMessageAsync<TagSelectView, TagSelectViewModel>(localization.GetLocalizedString("AddTags"), viewModel);

            if (viewModel.DialogResultOk && viewModel.AllTags != null)
            {
                Recipe.Tags = new ObservableCollection<TagEdit>(viewModel.AllTags.Where(x => x.IsChecked));
            }
        }

        private async void AddIngredientGroup()
        {
            var viewModel = new IngredientGroupEditViewModel(dialogService, new IngredientGroupEdit());
            await dialogService.ShowCustomMessageAsync<IngredientGroupEditView, IngredientGroupEditViewModel>(localization.GetLocalizedString("AddIngredientsGroup"), viewModel)
                             .ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                Recipe!.IngredientGroups = Recipe.IngredientGroups ?? new ObservableCollection<IngredientGroupEdit>();
                Recipe.IngredientGroups.Add(viewModel.IngredientGroup);
            }
        }

        private async Task EditIngredientGroup(IngredientGroupEdit group)
        {
            var viewModel = new IngredientGroupEditViewModel(dialogService, group);
            await dialogService.ShowCustomMessageAsync<IngredientGroupEditView, IngredientGroupEditViewModel>(localization.GetLocalizedString("EditIngredientsGroup"), viewModel)
                             .ConfigureAwait(true);

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

        private void RemoveTag(TagEdit tag) => Recipe!.Tags!.Remove(tag);
        private void RemoveImage() => Recipe!.ImagePath = null;

        private bool CanRemoveImage() => Recipe?.ImagePath != null;

        private async void AddIngredient()
        {
            RecipeIngredientEditViewModel viewModel = container.Resolve<RecipeIngredientEditViewModel>();
            viewModel.IsCreation = true;

            await dialogService.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>(localization.GetLocalizedString("AddIngredient"), viewModel).ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                Recipe!.Ingredients ??= new ObservableCollection<RecipeIngredientEdit>();

                if (viewModel.Ingredients != null)
                {
                    foreach (RecipeIngredientEdit ingredient in viewModel.Ingredients)
                    {
                        ingredient.Order += Recipe.Ingredients.Count;
                        Recipe.Ingredients.Add(ingredient);
                    }
                }

                viewModel.Ingredient.Order = Recipe.Ingredients.Count + 1;
                Recipe.Ingredients.Add(viewModel.Ingredient);
            }
        }

        private async void AddIngredientToGroup(IngredientGroupEdit group)
        {
            RecipeIngredientEditViewModel viewModel = container.Resolve<RecipeIngredientEditViewModel>();
            viewModel.IsCreation = true;

            await dialogService.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>(localization.GetLocalizedString("AddIngredient"), viewModel).ConfigureAwait(true);

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

        private async Task ImageSearch() => await Task.Run(() => Recipe!.ImagePath = imageService.UseImage());

        private async Task ApplyChanges()
        {
            if (Recipe!.ID == Guid.Empty)
            {
                Recipe.ID = await recipeService.CreateAsync(Recipe);
                eventAggregator.GetEvent<RecipeCreatedEvent>().Publish(Recipe);
            }
            else
            {
                await recipeService.UpdateAsync(Recipe);
                eventAggregator.GetEvent<RecipeUpdatedEvent>().Publish(Recipe);
            }

            RecipeBackup = null;
            IsEditing = false;
            IsRecipeCreation = false;
        }

        private async Task DeleteRecipe(Guid recipeId) => await dialogService.ShowYesNoDialog(localization.GetLocalizedString("SureDelete", Recipe!.Name),
                                                                                           localization.GetLocalizedString("CannotUndo"),
                                                                                           successCallback: () => OnRecipeDeleted(recipeId));

        private async void OnRecipeDeleted(Guid recipeId)
        {
            await recipeService.DeleteAsync(recipeId);
            eventAggregator.GetEvent<RecipeDeletedEvent>().Publish(recipeId);

            // Journal methods must be called from UI thread
            Application.Current.Dispatcher.Invoke(() => journal?.GoBack());
        }
    }
}