using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
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
                            foreach (var ingredient in viewModel.Ingredients)
                            {
                                Recipe.Ingredients.Add(ingredient);
                            }
                        }
                    }
                }));

            RemoveIngredientGroupCommand = new Lazy<DelegateCommand<IngredientGroupDTO>>(
                () => new DelegateCommand<IngredientGroupDTO>(async (group) => {

                    var dialog = await DialogCoordinator.Instance.ShowMessageAsync(this, "Точно удалить?", null, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Да", NegativeButtonText = "Нет" });

                    if (dialog == MessageDialogResult.Affirmative)
                    {
                        Recipe.IngredientGroups.Remove(group);
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
                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

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
        public Lazy<DelegateCommand> AddIngredientGroupCommand { get; }

        
        public Lazy<DelegateCommand<IngredientGroupDTO>> AddIngredientToGroupCommand { get; }
        public Lazy<DelegateCommand<IngredientGroupDTO>> RemoveIngredientGroupCommand { get; }
        public Lazy<DelegateCommand<IngredientGroupDTO>> EditIngredientGroupCommand { get; }
        public Lazy<DelegateCommand<RecipeIngredientDTO>> EditIngredientCommand { get; }
        public Lazy<DelegateCommand<RecipeIngredientDTO>> RemoveIngredientCommand { get; }
        
        public RecipeDTO Recipe { get; set; }
    }
}