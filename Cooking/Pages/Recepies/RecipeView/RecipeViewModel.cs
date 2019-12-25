using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.Projections;
using Data.Model;
using GongSolutions.Wpf.DragDrop;
using MahApps.Metro.Controls.Dialogs;
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

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class RecipeViewModel : IDropTarget, INavigationAware, IRegionMemberLifetime
    {
        private readonly DialogService dialogUtils;
        private readonly ImageService imageService;
        private readonly IContainerExtension container;
        private readonly RecipeService recipeService;
        private readonly IMapper mapper;
        private IRegionNavigationJournal? journal;

        public bool IsEditing { get; set; }
        public void OnIsEditingChanged()
        {
            if (IsEditing)
            {
                RecipeBackup = mapper.Map<RecipeEdit>(Recipe);
            }
            else
            {
                if (RecipeBackup != null)
                {
                    Recipe = mapper.Map<RecipeEdit>(RecipeBackup);
                }
            }
        }

        public RecipeEdit? Recipe { get; set; }
        private RecipeEdit? RecipeBackup { get; set; }

        public AsyncDelegateCommand ApplyChangesCommand { get; }

        public DelegateCommand CloseCommand { get; }
        public DelegateCommand ImageSearchCommand { get; }
        public DelegateCommand RemoveImageCommand { get; }
        public AsyncDelegateCommand AddIngredientCommand { get; }
        public AsyncDelegateCommand AddTagCommand { get; }
        public AsyncDelegateCommand AddIngredientGroupCommand { get; }

        public DelegateCommand<TagEdit> RemoveTagCommand { get; }

        public DelegateCommand<DTO.IngredientGroupEdit> AddIngredientToGroupCommand { get; }
        public DelegateCommand<DTO.IngredientGroupEdit> RemoveIngredientGroupCommand { get; }
        public AsyncDelegateCommand<DTO.IngredientGroupEdit> EditIngredientGroupCommand { get; }
        public AsyncDelegateCommand<RecipeIngredientEdit> EditIngredientCommand { get; }
        public DelegateCommand<RecipeIngredientEdit> RemoveIngredientCommand { get; }
        public AsyncDelegateCommand<Guid> DeleteRecipeCommand { get; }

        public RecipeViewModel(DialogService dialogUtils, 
                               ImageService imageService, 
                               IContainerExtension container, 
                               RecipeService recipeService, 
                               IMapper mapper)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(imageService != null);
            Debug.Assert(container != null);
            Debug.Assert(recipeService != null);
            Debug.Assert(mapper != null);

            this.dialogUtils            = dialogUtils;
            this.imageService           = imageService;
            this.container              = container;
            this.recipeService          = recipeService;
            this.mapper                 = mapper;

            CloseCommand                = new DelegateCommand(Close);
            ApplyChangesCommand         = new AsyncDelegateCommand(ApplyChanges);
            DeleteRecipeCommand         = new AsyncDelegateCommand<Guid>(DeleteRecipe);

            ImageSearchCommand          = new DelegateCommand(ImageSearch);
            RemoveImageCommand          = new DelegateCommand(RemoveImage, canExecute: CanRemoveImage);

            AddTagCommand               = new AsyncDelegateCommand(AddTag);
            RemoveTagCommand            = new DelegateCommand<TagEdit>(RemoveTag);

            AddIngredientGroupCommand   = new AsyncDelegateCommand(AddIngredientGroup);
            EditIngredientGroupCommand = new AsyncDelegateCommand<DTO.IngredientGroupEdit>(this.EditIngredientGroup);
            AddIngredientToGroupCommand = new DelegateCommand<DTO.IngredientGroupEdit>(this.AddIngredientToGroup);
            RemoveIngredientGroupCommand = new DelegateCommand<DTO.IngredientGroupEdit>(this.RemoveIngredientGroup);

            AddIngredientCommand        = new AsyncDelegateCommand(AddIngredient);
            EditIngredientCommand       = new AsyncDelegateCommand<RecipeIngredientEdit>(EditIngredient);
            RemoveIngredientCommand     = new DelegateCommand<RecipeIngredientEdit>(RemoveIngredient);
        }

        private void Close()
        {
            journal.GoBack();
        }

        private void RemoveIngredientGroup(DTO.IngredientGroupEdit arg) => Recipe!.IngredientGroups!.Remove(arg);

        private void RemoveIngredient(RecipeIngredientEdit ingredient)
        {
            if (Recipe!.Ingredients != null && Recipe.Ingredients.Contains(ingredient))
            {
                Recipe.Ingredients.Remove(ingredient);
                return;
            }

            if (Recipe.IngredientGroups != null)
            {
                foreach (var group in Recipe.IngredientGroups)
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
            var viewModel = container.Resolve<RecipeIngredientEditViewModel>();
            viewModel.Ingredient = ingredient;

            await dialogUtils.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("Изменение ингредиента", viewModel).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                mapper.Map(viewModel.Ingredient, ingredient);
            }
        }

        private async Task AddTag()
        {
            var viewModel = container.Resolve<TagSelectViewModel>();
            viewModel.SetTags(Recipe.Tags, null);
            await dialogUtils.ShowCustomMessageAsync<TagSelect, TagSelectViewModel>("Добавление тегов", viewModel).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                Recipe.Tags = new ObservableCollection<TagEdit>(viewModel.AllTags.Where(x => x.IsChecked));
            }
        }

        private async Task AddIngredientGroup()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<IngredientGroupEdit, IngredientGroupEditViewModel>("Добавление группы ингредиентов")
                                             .ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                Recipe.IngredientGroups = Recipe.IngredientGroups ?? new ObservableCollection<DTO.IngredientGroupEdit>();
                Recipe.IngredientGroups.Add(viewModel.IngredientGroup);
            }
        }

        private async Task EditIngredientGroup(DTO.IngredientGroupEdit group)
        {
            var viewModel = new IngredientGroupEditViewModel(dialogUtils, mapper.Map<DTO.IngredientGroupEdit>(group));
            await dialogUtils.ShowCustomMessageAsync<IngredientGroupEdit, IngredientGroupEditViewModel>("Редактирование группы ингредиентов", viewModel)
                             .ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                mapper.Map(viewModel.IngredientGroup, group);
            }
        }

        private void RemoveTag(TagEdit tag) => Recipe.Tags!.Remove(tag);
        private void RemoveImage() => Recipe.ImagePath = null;


        public bool KeepAlive => false;

        private bool CanRemoveImage()
        {
            return Recipe?.ImagePath != null;
        }

        public async Task AddIngredient()
        {
            var viewModel = container.Resolve<RecipeIngredientEditViewModel>();
            viewModel.IsCreation = true;

            await dialogUtils.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("Добавление ингредиента", viewModel).ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                Recipe.Ingredients ??= new ObservableCollection<RecipeIngredientEdit>();

                if (viewModel.Ingredients != null)
                {
                    foreach (var ingredient in viewModel.Ingredients)
                    {
                        ingredient.Order += Recipe.Ingredients.Count;
                        Recipe.Ingredients.Add(ingredient);
                    }
                }

                viewModel.Ingredient.Order = Recipe.Ingredients.Count + 1;
                Recipe.Ingredients.Add(viewModel.Ingredient);
            }
        }

        public async void AddIngredientToGroup(DTO.IngredientGroupEdit group)
        {
            var viewModel = container.Resolve<RecipeIngredientEditViewModel>();
            viewModel.IsCreation = true;

            await dialogUtils.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("Добавление ингредиента", viewModel).ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                group.Ingredients ??= new ObservableCollection<RecipeIngredientEdit>();

                if (viewModel.Ingredients != null)
                {
                    foreach (var ingredient in viewModel.Ingredients)
                    {
                        ingredient.Order += group.Ingredients.Count;
                        group.Ingredients.Add(ingredient);
                    }
                }

                viewModel.Ingredient.Order = group.Ingredients.Count + 1;
                group.Ingredients.Add(viewModel.Ingredient);
            }
        }

        public void ImageSearch()
        {
            var image = imageService.ImageSearch();

            if (image != null)
            {
                Recipe.ImagePath = image;
            }
        }

        protected async Task ApplyChanges()
        {
            if (Recipe.ID == Guid.Empty)
            {
                Recipe.ID = await recipeService.CreateAsync(mapper.Map<Recipe>(Recipe)).ConfigureAwait(false);
            }
            else
            {
                await recipeService.UpdateAsync(mapper.Map<Recipe>(Recipe)).ConfigureAwait(false);
            }
            RecipeBackup = null;
            IsEditing = false;
        }

        public async Task DeleteRecipe(Guid recipeId)
        {
            var result = await DialogCoordinator.Instance.ShowMessageAsync(
                this,
                "Точно удалить?",
                "Восстановить будет нельзя",
                style: MessageDialogStyle.AffirmativeAndNegative,
                settings: new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Да",
                    NegativeButtonText = "Нет"
                }).ConfigureAwait(false);

            if (result == MessageDialogResult.Affirmative)
            {
                await recipeService.DeleteAsync(recipeId).ConfigureAwait(false);
                CloseCommand.Execute();
            }
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
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}