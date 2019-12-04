using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Recepies;
using Cooking.ServiceLayer;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages.Ingredients
{
    [AddINotifyPropertyChangedInterface]
    public partial class RecipeIngredientEditViewModel : OkCancelViewModel
    {
        private readonly DialogUtils dialogUtils;

        public bool IsCreation { get; set; }
        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public DelegateCommand AddMultipleCommand { get; }
        public DelegateCommand<RecipeIngredientEdit> RemoveIngredientCommand { get; }

        public AsyncDelegateCommand AddCategoryCommand { get; }


        public RecipeIngredientEdit Ingredient { get; set; }
        public ObservableCollection<RecipeIngredientEdit>? Ingredients { get; private set; }

        public List<IngredientEdit> AllIngredients { get; }

        public RecipeIngredientEditViewModel() : this(null) { }

        public RecipeIngredientEditViewModel(RecipeIngredientEdit? ingredient = null)
        {
            dialogUtils = new DialogUtils(this);
            Ingredient = ingredient ?? new RecipeIngredientEdit();

            AddMultipleCommand = new DelegateCommand(AddMultiple, canExecute: () => IsCreation);
            RemoveIngredientCommand = new DelegateCommand<RecipeIngredientEdit>(RemoveIngredient);
            AddCategoryCommand = new AsyncDelegateCommand(AddRecipe);

            AllIngredients = IngredientService.GetIngredients<IngredientData>()
                                              .MapTo<List<IngredientEdit>>();
        }

        private void RemoveIngredient(RecipeIngredientEdit i) => Ingredients!.Remove(i);

        private void AddMultiple()
        {
            Ingredients ??= new ObservableCollection<RecipeIngredientEdit>();
            Ingredient.Order = Ingredients.Count + 1;
            Ingredients.Add(Ingredient);
            Ingredient = new RecipeIngredientEdit();
        }

        private async Task AddRecipe()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<IngredientEditView, IngredientEditViewModel>("Новый ингредиент")
                                             .ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var id = await IngredientService.CreateAsync(viewModel.Ingredient.MapTo<Ingredient>())
                                                .ConfigureAwait(false);
                viewModel.Ingredient.ID = id;
                AllIngredients.Add(viewModel.Ingredient);
                Ingredient.Ingredient = viewModel.Ingredient;
            }
        }
    }
}