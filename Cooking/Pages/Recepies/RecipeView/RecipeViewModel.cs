using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using Cooking.ServiceLayer;
using GongSolutions.Wpf.DragDrop;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class RecipeViewModel : OkCancelViewModel, IDropTarget
    {
        public bool IsEditing { get; set; }

        public RecipeMain Recipe { get; set; }
        private RecipeMain RecipeBackup { get; set; }
        public void OnIsEditingChanged()
        {
            if (IsEditing)
            {
                RecipeBackup = Recipe.MapTo<RecipeMain>();
            }
            else
            {
                Recipe = RecipeBackup.MapTo<RecipeMain>();
            }
        }

        public RecipeViewModel() : this(null) { }

        public RecipeViewModel(Guid? recipeId)
        {
            if (recipeId.HasValue)
            {
                var recipeDb = RecipeService.GetRecipe<RecipeFull>(recipeId.Value).Result;
                Recipe = MapperService.Mapper.Map<RecipeMain>(recipeDb);
            }
            else
            {
                Recipe = new RecipeMain();
            }

            DeleteRecipeCommand = new DelegateCommand<Guid>(DeleteRecipe);
            ImageSearchCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(() => {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Title = "Поиск изображения для рецепта"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        if (File.Exists(openFileDialog.FileName))
                        {
                            var dir = Directory.CreateDirectory("Images");
                            var file = new FileInfo(openFileDialog.FileName);
                            var newFilePath = Path.Combine(dir.FullName, file.Name);
                            if (File.Exists(newFilePath))
                            {
                                File.Delete(newFilePath);
                            }

                            File.Copy(openFileDialog.FileName, newFilePath);
                            Recipe.ImagePath = $@"Images/{file.Name}";
                        }
                    }
                }
                ));
            RemoveImageCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(() => {
                    Recipe.ImagePath = null;
                },
                canExecute: () => Recipe?.ImagePath != null));

            AddIngredientToGroupCommand = new Lazy<DelegateCommand<IngredientGroupMain>>(
                () => new DelegateCommand<IngredientGroupMain>(async (group) => {
                    var viewModel = new RecipeIngredientEditViewModel() { IsCreation = true };

                    var dialog = new CustomDialog()
                    {
                        Title = "Добавление ингредиента",
                        Content = new RecipeIngredientEditView()
                        {
                            DataContext = viewModel
                        }
                    };

                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

                    do
                    {
                        await dialog.WaitUntilUnloadedAsync();
                    }
                    while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

                    if (viewModel.DialogResultOk)
                    {
                        if (group.Ingredients == null)
                        {
                            group.Ingredients = new ObservableCollection<RecipeIngredientMain>();
                        }

                        var normalizeCount = group.Ingredients.Count;

                        if (viewModel.Ingredients != null)
                        {
                            foreach (var ingredient in viewModel.Ingredients)
                            {
                                ingredient.Order += normalizeCount;
                                group.Ingredients.Add(ingredient);
                            }
                        }

                        viewModel.Ingredient.Order = group.Ingredients.Count + 1;
                        group.Ingredients.Add(viewModel.Ingredient);
                    }
                }));

            AddIngredientCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var viewModel = new RecipeIngredientEditViewModel() { IsCreation = true };

                    var dialog = new CustomDialog()
                    {
                        Title = "Добавление ингредиента",
                        Content = new RecipeIngredientEditView()
                        {
                            DataContext = viewModel
                        }
                    };

                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

                    do
                    {
                        await dialog.WaitUntilUnloadedAsync();
                    }
                    while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

                    if (viewModel.DialogResultOk)
                    {
                        if (Recipe.Ingredients == null)
                        {
                            Recipe.Ingredients = new ObservableCollection<RecipeIngredientMain>();
                        }

                        var normalizeCount = Recipe.Ingredients.Count;

                        if (viewModel.Ingredients != null)
                        {
                            foreach (var ingredient in viewModel.Ingredients)
                            {
                                ingredient.Order += normalizeCount;
                                Recipe.Ingredients.Add(ingredient);
                            }
                        }

                        viewModel.Ingredient.Order = Recipe.Ingredients.Count + 1;
                        Recipe.Ingredients.Add(viewModel.Ingredient);
                    }
                }));

            EditIngredientGroupCommand = new Lazy<DelegateCommand<IngredientGroupMain>>(
                () => new DelegateCommand<IngredientGroupMain>(async (group) => {
                    var viewModel = new IngredientGroupEditViewModel(Mapper.Map<IngredientGroupMain>(group));

                    var dialog = new CustomDialog()
                    {
                        Title = "Редактирование группы ингредиентов",
                        Content = new IngredientGroupEdit()
                        {
                            DataContext = viewModel
                        }
                    };
                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

                    if (viewModel.DialogResultOk)
                    {
                        Mapper.Map(viewModel.IngredientGroup, group);
                    }
                }));


            AddIngredientGroupCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var viewModel = new IngredientGroupEditViewModel();

                    var dialog = new CustomDialog()
                    {
                        Title = "Добавление группы ингредиентов",
                        Content = new IngredientGroupEdit()
                        {
                            DataContext = viewModel
                        }
                    };

                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

                    do
                    {
                        await dialog.WaitUntilUnloadedAsync();
                    }
                    while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

                    if (viewModel.DialogResultOk)
                    {
                        Recipe.IngredientGroups = Recipe.IngredientGroups ?? new ObservableCollection<IngredientGroupMain>();
                        Recipe.IngredientGroups.Add(viewModel.IngredientGroup);
                    }
                }));

            AddTagCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var viewModel = new TagSelectEditViewModel(Recipe.Tags, new DialogUtils(this));

                    var dialog = new CustomDialog()
                    {
                        Title = "Добавление тегов",
                        Content = new TagSelectView()
                        {
                            DataContext = viewModel
                        }
                    };

                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

                    do
                    {
                        await dialog.WaitUntilUnloadedAsync();
                    }
                    while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

                    if (viewModel.DialogResultOk)
                    {
                        IEnumerable<TagDTO> tags = new List<TagDTO>();
                        if (viewModel.MainIngredients != null)
                        {
                            tags = tags.Union(viewModel.MainIngredients.Where(x => x.IsChecked));
                        }

                        if (viewModel.DishTypes != null)
                        {
                            tags = tags.Union(viewModel.DishTypes.Where(x => x.IsChecked));
                        }

                        if (viewModel.Occasions != null)
                        {
                            tags = tags.Union(viewModel.Occasions.Where(x => x.IsChecked));
                        }

                        if (viewModel.Sources != null)
                        {
                            tags = tags.Union(viewModel.Sources.Where(x => x.IsChecked));
                        }

                        Recipe.Tags = new ObservableCollection<TagDTO>(tags);
                    }
                }));

            RemoveTagCommand = new Lazy<DelegateCommand<TagDTO>>(
                () => new DelegateCommand<TagDTO>(tag =>
            {
                Recipe.Tags.Remove(tag);
            }));

            EditIngredientCommand = new Lazy<DelegateCommand<RecipeIngredientMain>>(
                () => new DelegateCommand<RecipeIngredientMain>(async (ingredient) => {

                    var viewModel = new RecipeIngredientEditViewModel(Mapper.Map<RecipeIngredientMain>(ingredient));

                    var dialog = new CustomDialog()
                    {
                        Title = "Изменение ингредиента",
                        Content = new RecipeIngredientEditView()
                        {
                            DataContext = viewModel
                        }
                    };

                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

                    do
                    {
                        await dialog.WaitUntilUnloadedAsync();
                    }
                    while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

                    if (viewModel.DialogResultOk)
                    {
                        Mapper.Map(viewModel.Ingredient, ingredient);
                    }
                }));

            RemoveIngredientCommand = new Lazy<DelegateCommand<RecipeIngredientMain>>(
                () => new DelegateCommand<RecipeIngredientMain>(ingredient => {

                    if (Recipe.Ingredients != null && Recipe.Ingredients.Contains(ingredient))
                    {
                        Recipe.Ingredients.Remove(ingredient);
                        return;
                    }

                    if (Recipe.IngredientGroups != null)
                    {
                        foreach(var group in Recipe.IngredientGroups)
                        {
                            if(group.Ingredients.Contains(ingredient))
                            {
                                group.Ingredients.Remove(ingredient);
                                return;
                            }
                        }
                    }
                }));

        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = System.Windows.DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.TargetCollection != dropInfo.DragInfo.SourceCollection) return;
            if (!(dropInfo.Data is RecipeIngredientMain ingredient)) return;
            if (!(dropInfo.TargetItem is RecipeIngredientMain targetIngredient)) return;
            if (dropInfo.Data == dropInfo.TargetItem) return;

            var oldOrder = ingredient.Order;

            if (dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem))
            {
                if (targetIngredient.Order < oldOrder)
                {
                    ingredient.Order = targetIngredient.Order + 1;
                }
                else
                {
                    ingredient.Order = targetIngredient.Order;
                }
            }
            else if (dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.BeforeTargetItem))
            {
                if (targetIngredient.Order > oldOrder)
                {
                    ingredient.Order = targetIngredient.Order - 1;
                }
                else
                {
                    ingredient.Order = targetIngredient.Order;
                }
            }
            else
            { }

            var backup = new List<RecipeIngredientMain>(dropInfo.TargetCollection.Cast<RecipeIngredientMain>());
            if (ingredient.Order < oldOrder)
            {
                foreach (var item in backup)
                {
                    if (ingredient != item && ingredient.Order <= item.Order && item.Order < oldOrder)
                    {
                        item.Order++;
                    }
                }
            }
            else
            {
                foreach (var item in backup)
                {
                    if (ingredient != item && item.Order > oldOrder && item.Order <= ingredient.Order)
                    {
                        item.Order--;
                    }
                }
            }

            (dropInfo.TargetCollection as ObservableCollection<RecipeIngredientMain>).Clear();
            foreach (var item in backup.OrderBy(x => x.Order))
            {
                (dropInfo.TargetCollection as ObservableCollection<RecipeIngredientMain>).Add(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> ImageSearchCommand { get; }
        public Lazy<DelegateCommand> RemoveImageCommand { get; }
        public Lazy<DelegateCommand> AddIngredientCommand { get; }
        public Lazy<DelegateCommand> AddTagCommand { get; }
        public Lazy<DelegateCommand> AddIngredientGroupCommand { get; }

        public Lazy<DelegateCommand<TagDTO>> RemoveTagCommand { get; }

        public Lazy<DelegateCommand<IngredientGroupMain>> AddIngredientToGroupCommand { get; }
        public Lazy<DelegateCommand<IngredientGroupMain>> RemoveIngredientGroupCommand { get; }
        public Lazy<DelegateCommand<IngredientGroupMain>> EditIngredientGroupCommand { get; }
        public Lazy<DelegateCommand<RecipeIngredientMain>> EditIngredientCommand { get; }
        public Lazy<DelegateCommand<RecipeIngredientMain>> RemoveIngredientCommand { get; }
        public DelegateCommand<Guid> DeleteRecipeCommand { get; }


        public async void DeleteRecipe(Guid recipeId)
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
                });

            if (result == MessageDialogResult.Affirmative)
            {
                await RecipeService.DeleteRecipe(recipeId);
                CloseCommand.Execute();
            }
        }
    }
}