using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using GongSolutions.Wpf.DragDrop;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class RecipeEditViewModel : INotifyPropertyChanged, IDropTarget
    {
        public bool DialogResultOk { get; set; }

        public RecipeEditViewModel(RecipeDTO category = null)
        {
            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

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
                            var newFileName = Path.Combine(dir.FullName, file.Name);
                            if (!File.Exists(newFileName))
                            {
                                File.Copy(openFileDialog.FileName, newFileName);
                            }
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

            AddIngredientToGroupCommand = new Lazy<DelegateCommand<IngredientGroupDTO>>(
                () => new DelegateCommand<IngredientGroupDTO>(async (group) => {
                    var viewModel = new RecipeIngredientEditViewModel() { IsCreation = true };

                    var dialog = new CustomDialog()
                    {
                        Title = "Добавление ингредиента",
                        Content = new RecipeIngredientEditView()
                        {
                            DataContext = viewModel
                        }
                    };
                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

                    if (viewModel.DialogResultOk)
                    {
                        if (group.Ingredients == null)
                        {
                            group.Ingredients = new ObservableCollection<RecipeIngredientDTO>();
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
                            Recipe.Ingredients = new ObservableCollection<RecipeIngredientDTO>();
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

            EditIngredientGroupCommand = new Lazy<DelegateCommand<IngredientGroupDTO>>(
                () => new DelegateCommand<IngredientGroupDTO>(async (group) => {
                    var viewModel = new IngredientGroupEditViewModel(Mapper.Map<IngredientGroupDTO>(group));

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
                        Recipe.IngredientGroups = Recipe.IngredientGroups ?? new ObservableCollection<IngredientGroupDTO>();
                        Recipe.IngredientGroups.Add(viewModel.IngredientGroup);
                    }
                }));

            AddTagCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var viewModel = new TagSelectEditViewModel(Recipe.Tags);

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

            EditIngredientCommand = new Lazy<DelegateCommand<RecipeIngredientDTO>>(
                () => new DelegateCommand<RecipeIngredientDTO>(async (ingredient) => {

                    var viewModel = new RecipeIngredientEditViewModel(Mapper.Map<RecipeIngredientDTO>(ingredient));

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

            RemoveIngredientCommand = new Lazy<DelegateCommand<RecipeIngredientDTO>>(
                () => new DelegateCommand<RecipeIngredientDTO>(ingredient => {

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

            Recipe = category ?? new RecipeDTO();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = System.Windows.DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }

        public async void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.TargetCollection != dropInfo.DragInfo.SourceCollection) return;
            if (!(dropInfo.Data is RecipeIngredientDTO ingredient)) return;
            if (!(dropInfo.TargetItem is RecipeIngredientDTO targetIngredient)) return;
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

            var backup = new List<RecipeIngredientDTO>(dropInfo.TargetCollection.Cast<RecipeIngredientDTO>());
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

            (dropInfo.TargetCollection as ObservableCollection<RecipeIngredientDTO>).Clear();
            foreach (var item in backup.OrderBy(x => x.Order))
            {
                (dropInfo.TargetCollection as ObservableCollection<RecipeIngredientDTO>).Add(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }
        public Lazy<DelegateCommand> ImageSearchCommand { get; }
        public Lazy<DelegateCommand> RemoveImageCommand { get; }
        public Lazy<DelegateCommand> AddIngredientCommand { get; }
        public Lazy<DelegateCommand> AddTagCommand { get; }
        public Lazy<DelegateCommand> AddIngredientGroupCommand { get; }

        public Lazy<DelegateCommand<TagDTO>> RemoveTagCommand { get; }

        public Lazy<DelegateCommand<IngredientGroupDTO>> AddIngredientToGroupCommand { get; }
        public Lazy<DelegateCommand<IngredientGroupDTO>> RemoveIngredientGroupCommand { get; }
        public Lazy<DelegateCommand<IngredientGroupDTO>> EditIngredientGroupCommand { get; }
        public Lazy<DelegateCommand<RecipeIngredientDTO>> EditIngredientCommand { get; }
        public Lazy<DelegateCommand<RecipeIngredientDTO>> RemoveIngredientCommand { get; }
        
        public RecipeDTO Recipe { get; set; }
    }
}