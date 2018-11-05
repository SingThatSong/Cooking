using AutoMapper;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class RecipeEditViewModel : INotifyPropertyChanged
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
                canExecuteMethod: () => Recipe?.ImagePath != null));

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
                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

                    if(viewModel.DialogResultOk)
                    {
                        if (Recipe.Ingredients == null)
                        {
                            Recipe.Ingredients = new ObservableCollection<RecipeIngredientDTO>();
                        }

                        Recipe.Ingredients.Add(viewModel.Ingredient);

                        if (viewModel.Ingredients != null)
                        {
                            Recipe.Ingredients.AddRange(viewModel.Ingredients);
                        }
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
                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

                    if (viewModel.DialogResultOk)
                    {
                        Recipe.Tags = new ObservableCollection<TagDTO>(viewModel.AllTags.Where(x => x.IsChecked));
                    }
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
                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

                    if (viewModel.DialogResultOk)
                    {
                        Mapper.Map(viewModel.Ingredient, ingredient);
                    }
                }));

            RemoveIngredientCommand = new Lazy<DelegateCommand<RecipeIngredientDTO>>(
                () => new DelegateCommand<RecipeIngredientDTO>(ingredient => {
                    Recipe.Ingredients.Remove(ingredient);  
                }));

            Recipe = category ?? new RecipeDTO();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }
        public Lazy<DelegateCommand> ImageSearchCommand { get; }
        public Lazy<DelegateCommand> RemoveImageCommand { get; }
        public Lazy<DelegateCommand> AddIngredientCommand { get; }
        public Lazy<DelegateCommand> AddTagCommand { get; }
        
        public Lazy<DelegateCommand<RecipeIngredientDTO>> EditIngredientCommand { get; }
        public Lazy<DelegateCommand<RecipeIngredientDTO>> RemoveIngredientCommand { get; }
        
        public RecipeDTO Recipe { get; set; }
    }
}