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
        public DelegateCommand<RecipeIngredientMain> RemoveIngredientCommand { get; }

        public AsyncDelegateCommand AddCategoryCommand { get; }


        public RecipeIngredientMain Ingredient { get; set; }
        public ObservableCollection<RecipeIngredientMain>? Ingredients { get; private set; }

        public List<IngredientMain> AllIngredients { get; }

        public RecipeIngredientEditViewModel() : this(null) { }

        public RecipeIngredientEditViewModel(RecipeIngredientMain? ingredient = null)
        {
            dialogUtils = new DialogUtils(this);
            Ingredient = ingredient ?? new RecipeIngredientMain();

            AddMultipleCommand = new DelegateCommand(AddMultiple, canExecute: () => IsCreation);
            RemoveIngredientCommand = new DelegateCommand<RecipeIngredientMain>(RemoveIngredient);
            AddCategoryCommand = new AsyncDelegateCommand(AddRecipe);

            AllIngredients = IngredientService.GetIngredients<IngredientData>()
                                              .MapTo<List<IngredientMain>>();
        }

        private void RemoveIngredient(RecipeIngredientMain i) => Ingredients!.Remove(i);

        private void AddMultiple()
        {
            Ingredients ??= new ObservableCollection<RecipeIngredientMain>();
            Ingredient.Order = Ingredients.Count + 1;
            Ingredients.Add(Ingredient);
            Ingredient = new RecipeIngredientMain();
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