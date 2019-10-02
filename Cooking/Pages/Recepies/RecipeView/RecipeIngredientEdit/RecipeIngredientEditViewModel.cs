using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Ingredients
{
    public partial class RecipeIngredientEditViewModel : INotifyPropertyChanged
    {
        public bool DialogResultOk { get; set; }

        public RecipeIngredientEditViewModel() : this(null) { }

        public RecipeIngredientEditViewModel(RecipeIngredientMain? ingredient = null)
        {
            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(() =>
                {
                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            AddMultipleCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(() =>
                {
                    Ingredients ??= new ObservableCollection<RecipeIngredientMain>();
                    Ingredient.Order = Ingredients.Count + 1;
                    Ingredients.Add(Ingredient);
                    Ingredient = new RecipeIngredientMain();
                },
                canExecute: () => IsCreation));
            RemoveIngredientCommand = new Lazy<DelegateCommand<RecipeIngredientMain>>(
                () => new DelegateCommand<RecipeIngredientMain>(i => Ingredients.Remove(i))
            );

            AddCategoryCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(AddRecipe));
            AllIngredients = IngredientService.GetIngredients<IngredientData>()
                                              .MapTo<List<IngredientMain>>();

            Ingredient = ingredient ?? new RecipeIngredientMain();
        }

        public async void AddRecipe()
        {
            var viewModel = new IngredientEditViewModel();

            var dialog = new CustomDialog()
            {
                Title = "Новый ингредиент",
                Content = new IngredientEditView()
                {
                    DataContext = viewModel
                }
            };

            await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
            await dialog.WaitUntilUnloadedAsync();

            if (viewModel.DialogResultOk)
            {
                var id = await IngredientService.CreateAsync(viewModel.Ingredient.MapTo<Ingredient>());
                viewModel.Ingredient.ID = id;
                AllIngredients.Add(viewModel.Ingredient);
                Ingredient.Ingredient = viewModel.Ingredient;
            }
        }

        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }
        public Lazy<DelegateCommand> AddMultipleCommand { get; }
        public Lazy<DelegateCommand<RecipeIngredientMain>> RemoveIngredientCommand { get; }

        public Lazy<DelegateCommand> AddCategoryCommand { get; }


        public RecipeIngredientMain Ingredient { get; set; }
        public ObservableCollection<RecipeIngredientMain> Ingredients { get; set; }

        public List<IngredientMain> AllIngredients { get; set; }

        public bool IsCreation { get; set; }

    }
}