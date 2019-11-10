using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using Cooking.ServiceLayer.Projections;
using Data.Model;
using GongSolutions.Wpf.DragDrop;
using MahApps.Metro.Controls.Dialogs;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages.Recepies
{
    public partial class RecipeViewModel : OkCancelViewModel, IDropTarget
    {
        private readonly DialogUtils dialogUtils;

        public bool IsEditing { get; set; }
        public void OnIsEditingChanged()
        {
            if (IsEditing)
            {
                RecipeBackup = Recipe.MapTo<RecipeMain>();
            }
            else
            {
                if (RecipeBackup != null)
                {
                    Recipe = RecipeBackup.MapTo<RecipeMain>();
                }
            }
        }

        public RecipeMain Recipe { get; set; }
        private RecipeMain? RecipeBackup { get; set; }

        public AsyncDelegateCommand ApplyChangesCommand { get; }

        public DelegateCommand ImageSearchCommand { get; }
        public DelegateCommand RemoveImageCommand { get; }
        public AsyncDelegateCommand AddIngredientCommand { get; }
        public AsyncDelegateCommand AddTagCommand { get; }
        public AsyncDelegateCommand AddIngredientGroupCommand { get; }

        public DelegateCommand<TagDTO> RemoveTagCommand { get; }

        public DelegateCommand<IngredientGroupMain> AddIngredientToGroupCommand { get; }
        public DelegateCommand<IngredientGroupMain> RemoveIngredientGroupCommand { get; }
        public AsyncDelegateCommand<IngredientGroupMain> EditIngredientGroupCommand { get; }
        public AsyncDelegateCommand<RecipeIngredientMain> EditIngredientCommand { get; }
        public DelegateCommand<RecipeIngredientMain> RemoveIngredientCommand { get; }
        public AsyncDelegateCommand<Guid> DeleteRecipeCommand { get; }

        public RecipeViewModel() : this(null) { }

        public RecipeViewModel(Guid? recipeId)
        {
            if (recipeId.HasValue)
            {
                var recipeDb = RecipeService.GetProjection<RecipeFull>(recipeId.Value);
                Recipe = recipeDb.MapTo<RecipeMain>();
            }
            else
            {
                Recipe = new RecipeMain();
            }

            dialogUtils = new DialogUtils(this);

            ApplyChangesCommand         = new AsyncDelegateCommand(ApplyChanges);
            DeleteRecipeCommand         = new AsyncDelegateCommand<Guid>(DeleteRecipe);

            ImageSearchCommand          = new DelegateCommand(ImageSearch);
            RemoveImageCommand          = new DelegateCommand(RemoveImage, canExecute: CanRemoveImage);

            AddTagCommand               = new AsyncDelegateCommand(AddTag);
            RemoveTagCommand            = new DelegateCommand<TagDTO>(RemoveTag);

            AddIngredientGroupCommand   = new AsyncDelegateCommand(AddIngredientGroup);
            EditIngredientGroupCommand  = new AsyncDelegateCommand<IngredientGroupMain>(EditIngredientGroup);
            AddIngredientToGroupCommand = new DelegateCommand<IngredientGroupMain>(AddIngredientToGroup);
            RemoveIngredientGroupCommand = new DelegateCommand<IngredientGroupMain>(RemoveIngredientGroup);

            AddIngredientCommand        = new AsyncDelegateCommand(AddIngredient);
            EditIngredientCommand       = new AsyncDelegateCommand<RecipeIngredientMain>(EditIngredient);
            RemoveIngredientCommand     = new DelegateCommand<RecipeIngredientMain>(RemoveIngredient);
        }

        private void RemoveIngredientGroup(IngredientGroupMain arg) => Recipe.IngredientGroups!.Remove(arg);

        private void RemoveIngredient(RecipeIngredientMain ingredient)
        {

            if (Recipe.Ingredients != null && Recipe.Ingredients.Contains(ingredient))
            {
                Recipe.Ingredients.Remove(ingredient);
                return;
            }

            if (Recipe.IngredientGroups != null)
            {
                foreach (var group in Recipe.IngredientGroups)
                {
                    if (group.Ingredients.Contains(ingredient))
                    {
                        group.Ingredients.Remove(ingredient);
                        return;
                    }
                }
            }
        }

        private async Task EditIngredient(RecipeIngredientMain ingredient)
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("Изменение ингредиента", new RecipeIngredientEditViewModel(ingredient.MapTo<RecipeIngredientMain>()))
                                                       .ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                viewModel.Ingredient.MapTo(ingredient);
            }
        }

        private async Task AddTag()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<TagSelectView, TagSelectEditViewModel>("Добавление тегов", new TagSelectEditViewModel(Recipe.Tags, dialogUtils))
                                             .ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                Recipe.Tags = new ObservableCollection<TagDTO>(viewModel.AllTags.Where(x => x.IsChecked));
            }
        }

        private async Task AddIngredientGroup()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<IngredientGroupEdit, IngredientGroupEditViewModel>("Добавление группы ингредиентов")
                                             .ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                Recipe.IngredientGroups = Recipe.IngredientGroups ?? new ObservableCollection<IngredientGroupMain>();
                Recipe.IngredientGroups.Add(viewModel.IngredientGroup);
            }
        }

        private async Task EditIngredientGroup(IngredientGroupMain group)
        {
            var viewModel = new IngredientGroupEditViewModel(group.MapTo<IngredientGroupMain>());
            await dialogUtils.ShowCustomMessageAsync<IngredientGroupEdit, IngredientGroupEditViewModel>("Редактирование группы ингредиентов", viewModel)
                             .ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                viewModel.IngredientGroup.MapTo(group);
            }
        }

        private void RemoveTag(TagDTO tag) => Recipe.Tags!.Remove(tag);
        private void RemoveImage() => Recipe.ImagePath = null;
        private bool CanRemoveImage() => Recipe?.ImagePath != null;

        public async Task AddIngredient()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("Добавление ингредиента", new RecipeIngredientEditViewModel() { IsCreation = true })
                                             .ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                Recipe.Ingredients ??= new ObservableCollection<RecipeIngredientMain>();

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

        public async void AddIngredientToGroup(IngredientGroupMain group)
        {
            var vm = new RecipeIngredientEditViewModel() { IsCreation = true };
            var viewModel = await dialogUtils.ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("Добавление ингредиента", vm)
                                             .ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                group.Ingredients ??= new ObservableCollection<RecipeIngredientMain>();

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
            var image = ImageService.ImageSearch();

            if (image != null)
            {
                Recipe.ImagePath = image;
            }
        }

        protected async Task ApplyChanges()
        {
            if (Recipe.ID == Guid.Empty)
            {
                Recipe.ID = await RecipeService.CreateAsync(Recipe.MapTo<Recipe>()).ConfigureAwait(false);
            }
            else
            {
                await RecipeService.UpdateAsync(Recipe.MapTo<Recipe>()).ConfigureAwait(false);
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
                await RecipeService.Delete(recipeId).ConfigureAwait(false);
                CloseCommand.Execute();
            }
        }
    }
}