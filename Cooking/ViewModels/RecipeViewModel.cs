using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Events;
using Cooking.WPF.Helpers;
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

namespace Cooking.WPF.Views
{
    [AddINotifyPropertyChangedInterface]
    public partial class RecipeViewModel : IDropTarget, INavigationAware, IRegionMemberLifetime
    {
        private readonly DialogService dialogUtils;
        private readonly ImageService imageService;
        private readonly IContainerExtension container;
        private readonly RecipeService recipeService;
        private readonly IMapper mapper;
        private readonly IEventAggregator eventAggregator;
        private readonly ILocalization localization;
        private readonly IRegionManager regionManager;
        private IRegionNavigationJournal? journal;

        public bool IsEditing { get; set; }
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

        public bool IsRecipeCreation { get; set; }

        public RecipeEdit? Recipe { get; set; }
        private RecipeEdit? RecipeBackup { get; set; }

        public AsyncDelegateCommand ApplyChangesCommand { get; }

        public DelegateCommand CloseCommand { get; }
        public DelegateCommand ImageSearchCommand { get; }
        public DelegateCommand RemoveImageCommand { get; }
        public DelegateCommand AddIngredientCommand { get; }
        public DelegateCommand AddTagCommand { get; }
        public DelegateCommand AddIngredientGroupCommand { get; }

        public DelegateCommand<TagEdit> RemoveTagCommand { get; }
        public DelegateCommand<TagEdit> ViewTagCommand { get; }

        public DelegateCommand<IngredientGroupEdit> AddIngredientToGroupCommand { get; }
        public DelegateCommand<IngredientGroupEdit> RemoveIngredientGroupCommand { get; }
        public AsyncDelegateCommand<IngredientGroupEdit> EditIngredientGroupCommand { get; }
        public AsyncDelegateCommand<RecipeIngredientEdit> EditIngredientCommand { get; }
        public DelegateCommand<RecipeIngredientEdit> RemoveIngredientCommand { get; }
        public AsyncDelegateCommand<Guid> DeleteRecipeCommand { get; }

        public RecipeViewModel(DialogService dialogUtils,
                               ImageService imageService,
                               IContainerExtension container,
                               RecipeService recipeService,
                               IMapper mapper,
                               IEventAggregator eventAggregator,
                               ILocalization localization,
                               IRegionManager regionManager)
        {
            this.dialogUtils = dialogUtils;
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

            ImageSearchCommand = new DelegateCommand(ImageSearch);
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
                    if (group.Ingredients != null && group.Ingredients.Contains(ingredient))
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

            await dialogUtils.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>(localization.GetLocalizedString("EditIngredient"), viewModel);

            if (viewModel.DialogResultOk)
            {
                mapper.Map(viewModel.Ingredient, ingredient);
            }
        }

        private async void AddTag()
        {
            TagSelectViewModel viewModel = container.Resolve<TagSelectViewModel>();
            viewModel.SetTags(Recipe!.Tags, null);
            await dialogUtils.ShowCustomMessageAsync<TagSelectView, TagSelectViewModel>(localization.GetLocalizedString("AddTags"), viewModel);

            if (viewModel.DialogResultOk)
            {
                Recipe.Tags = new ObservableCollection<TagEdit>(viewModel.AllTags.Where(x => x.IsChecked));
            }
        }

        private async void AddIngredientGroup()
        {
            var viewModel = new IngredientGroupEditViewModel(dialogUtils, new IngredientGroupEdit());
            await dialogUtils.ShowCustomMessageAsync<IngredientGroupEditView, IngredientGroupEditViewModel>(localization.GetLocalizedString("AddIngredientsGroup"), viewModel)
                             .ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                Recipe!.IngredientGroups = Recipe.IngredientGroups ?? new ObservableCollection<IngredientGroupEdit>();
                Recipe.IngredientGroups.Add(viewModel.IngredientGroup);
            }
        }

        private async Task EditIngredientGroup(IngredientGroupEdit group)
        {
            var viewModel = new IngredientGroupEditViewModel(dialogUtils, group);
            await dialogUtils.ShowCustomMessageAsync<IngredientGroupEditView, IngredientGroupEditViewModel>(localization.GetLocalizedString("EditIngredientsGroup"), viewModel)
                             .ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                mapper.Map(viewModel.IngredientGroup, group);
            }
        }

        private void ViewTag(TagEdit tag)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecipeListViewModel.FilterText), $"{Consts.TagSymbol}\"{tag.Name}\"" }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(RecipeListView), parameters);
        }

        private void RemoveTag(TagEdit tag) => Recipe!.Tags!.Remove(tag);
        private void RemoveImage() => Recipe!.ImagePath = null;

        public bool KeepAlive => false;

        private bool CanRemoveImage() => Recipe?.ImagePath != null;

        public async void AddIngredient()
        {
            RecipeIngredientEditViewModel viewModel = container.Resolve<RecipeIngredientEditViewModel>();
            viewModel.IsCreation = true;

            await dialogUtils.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>(localization.GetLocalizedString("AddIngredient"), viewModel).ConfigureAwait(true);

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

        public async void AddIngredientToGroup(IngredientGroupEdit group)
        {
            RecipeIngredientEditViewModel viewModel = container.Resolve<RecipeIngredientEditViewModel>();
            viewModel.IsCreation = true;

            await dialogUtils.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>(localization.GetLocalizedString("AddIngredient"), viewModel).ConfigureAwait(true);

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

        public void ImageSearch() => Recipe!.ImagePath = imageService.ImageSearch();

        protected async Task ApplyChanges()
        {
            if (Recipe!.ID == Guid.Empty)
            {
                Recipe.ID = await recipeService.CreateAsync(mapper.Map<Recipe>(Recipe));
                eventAggregator.GetEvent<RecipeCreatedEvent>().Publish(Recipe);
            }
            else
            {
                await recipeService.UpdateAsync(mapper.Map<Recipe>(Recipe));
                eventAggregator.GetEvent<RecipeUpdatedEvent>().Publish(Recipe);
            }

            RecipeBackup = null;
            IsEditing = false;
            IsRecipeCreation = false;
        }

        public async Task DeleteRecipe(Guid recipeId) => await dialogUtils.ShowYesNoDialog(localization.GetLocalizedString("SureDelete", Recipe!.ID),
                                                                                           localization.GetLocalizedString("CannotUndo"),
                                                                                           successCallback: () => OnRecipeDeleted(recipeId))
                                                                          ;

        private async void OnRecipeDeleted(Guid recipeId)
        {
            await recipeService.DeleteAsync(recipeId);
            CloseCommand.Execute();
            eventAggregator.GetEvent<RecipeDeletedEvent>().Publish(recipeId);

            // Journal methods must be called from UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                journal?.GoBack();
            });
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.journal = navigationContext.NavigationService.Journal;
            var recipeId = navigationContext.Parameters[nameof(Recipe)] as Guid?;
            if (recipeId.HasValue)
            {
                Recipe = recipeService.GetProjected<RecipeEdit>(recipeId.Value, container.Resolve<IMapper>());
            }
            else
            {
                Recipe = new RecipeEdit();
                IsEditing = true;
                IsRecipeCreation = true;
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}