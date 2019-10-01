using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using Cooking.ServiceLayer;
using Data.Model;
using GongSolutions.Wpf.DragDrop;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class RecipeViewModel : OkCancelViewModel, IDropTarget
    {
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
        private RecipeMain RecipeBackup { get; set; }

        public DelegateCommand ImageSearchCommand { get; }
        public DelegateCommand RemoveImageCommand { get; }
        public DelegateCommand AddIngredientCommand { get; }
        public DelegateCommand AddTagCommand { get; }
        public DelegateCommand AddIngredientGroupCommand { get; }

        public DelegateCommand<TagDTO> RemoveTagCommand { get; }

        public DelegateCommand<IngredientGroupMain> AddIngredientToGroupCommand { get; }
        public DelegateCommand<IngredientGroupMain> RemoveIngredientGroupCommand { get; }
        public DelegateCommand<IngredientGroupMain> EditIngredientGroupCommand { get; }
        public DelegateCommand<RecipeIngredientMain> EditIngredientCommand { get; }
        public DelegateCommand<RecipeIngredientMain> RemoveIngredientCommand { get; }
        public DelegateCommand<Guid> DeleteRecipeCommand { get; }

        public RecipeViewModel() : this(null) { }

        public RecipeViewModel(Guid? recipeId)
        {
            if (recipeId.HasValue)
            {
                var recipeDb = RecipeService.GetRecipe<RecipeFull>(recipeId.Value);

                Recipe = recipeDb.MapTo<RecipeMain>();
            }
            else
            {
                Recipe = new RecipeMain();
            }

            OkCommand = new DelegateCommand(Ok);
            DeleteRecipeCommand = new DelegateCommand<Guid>(DeleteRecipe);

            ImageSearchCommand = new DelegateCommand(ImageSearch);
            RemoveImageCommand = new DelegateCommand(
                () => Recipe.ImagePath = null,
                canExecute: () => Recipe?.ImagePath != null
            );

            AddIngredientToGroupCommand = new DelegateCommand<IngredientGroupMain>(AddIngredientToGroup);
            AddIngredientCommand = new DelegateCommand(AddIngredient);
            EditIngredientGroupCommand = new DelegateCommand<IngredientGroupMain>(async (group) => {

                var viewModel = new IngredientGroupEditViewModel(group.MapTo<IngredientGroupMain>());
                await new DialogUtils(this).ShowCustomMessageAsync<IngredientGroupEdit, IngredientGroupEditViewModel>("Редактирование группы ингредиентов", viewModel);

                if (viewModel.DialogResultOk)
                {
                    viewModel.IngredientGroup.MapTo(group);
                }
            });


            AddIngredientGroupCommand = new DelegateCommand(async () => {
                var viewModel = await new DialogUtils(this).ShowCustomMessageAsync<IngredientGroupEdit, IngredientGroupEditViewModel>("Добавление группы ингредиентов");

                if (viewModel.DialogResultOk)
                {
                    Recipe.IngredientGroups = Recipe.IngredientGroups ?? new ObservableCollection<IngredientGroupMain>();
                    Recipe.IngredientGroups.Add(viewModel.IngredientGroup);
                }
            });

            AddTagCommand = new DelegateCommand(async () => {
                var dialogUtils = new DialogUtils(this);
                var viewModel = await dialogUtils.ShowCustomMessageAsync<TagSelectView, TagSelectEditViewModel>("Добавление тегов", new TagSelectEditViewModel(Recipe.Tags, dialogUtils));

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
            });

            RemoveTagCommand = new DelegateCommand<TagDTO>(tag =>
            {
                Recipe.Tags.Remove(tag);
            });

            EditIngredientCommand = new DelegateCommand<RecipeIngredientMain>(async (ingredient) => {
                var viewModel = await new DialogUtils(this).ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("Изменение ингредиента", new RecipeIngredientEditViewModel(ingredient.MapTo<RecipeIngredientMain>()));

                if (viewModel.DialogResultOk)
                {
                    viewModel.Ingredient.MapTo(ingredient);
                }
            });

            RemoveIngredientCommand = new DelegateCommand<RecipeIngredientMain>(ingredient => {

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
            });

        }

        public async void AddIngredient()
        {

            var viewModel = await new DialogUtils(this).ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("Добавление ингредиента", new RecipeIngredientEditViewModel() { IsCreation = true });

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
            var viewModel = await new DialogUtils(this).ShowCustomMessageAsync<RecipeIngredientEditView, RecipeIngredientEditViewModel>("Добавление ингредиента", new RecipeIngredientEditViewModel() { IsCreation = true });

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
            if (!(dropInfo.TargetCollection is ObservableCollection<RecipeIngredientMain> targetCollection)) return;
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

            var backup = targetCollection.MapTo<List<RecipeIngredientMain>>();
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

            targetCollection.Clear();
            foreach (var item in backup.OrderBy(x => x.Order))
            {
                targetCollection.Add(item);
            }
        }


        protected override async void Ok()
        {
            await RecipeService.UpdateAsync(Recipe.MapTo<Recipe>());

            RecipeBackup = null;
            IsEditing = false;
        }


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