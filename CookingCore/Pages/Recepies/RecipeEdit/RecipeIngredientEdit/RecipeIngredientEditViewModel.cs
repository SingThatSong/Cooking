﻿using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
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

        public RecipeIngredientEditViewModel(RecipeIngredientDTO ingredient = null)
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
                    Ingredients = Ingredients ?? new ObservableCollection<RecipeIngredientDTO>();
                    Ingredient.Order = Ingredients.Count + 1;
                    Ingredients.Add(Ingredient);
                    Ingredient = new RecipeIngredientDTO();
                },
                canExecute: () => IsCreation));
            RemoveIngredientCommand = new Lazy<DelegateCommand<RecipeIngredientDTO>>(
                () => new DelegateCommand<RecipeIngredientDTO>(i => Ingredients.Remove(i))
            );

            AddCategoryCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(AddRecipe));

            using (var context = new CookingContext())
            {
                AllIngredients = context.Ingredients.Select(x => Mapper.Map<IngredientDTO>(x)).ToList();
            }
            if (ingredient != null)
            {
                Ingredient = ingredient;

                if (Ingredient.Ingredient != null)
                {
                    Ingredient.Ingredient = AllIngredients.SingleOrDefault(x => x.ID == Ingredient.Ingredient.ID);
                }

                if (Ingredient.MeasureUnit != null)
                {
                    Ingredient.MeasureUnit = MeasurementUnits.Single(x => x.ID == Ingredient.MeasureUnit.ID);
                }
            }
            else
            {
                Ingredient = new RecipeIngredientDTO();
            }
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
                var category = Mapper.Map<Ingredient>(viewModel.Ingredient);
                using (var context = new CookingContext())
                {
                    context.Add(category);
                    context.SaveChanges();
                }
                viewModel.Ingredient.ID = category.ID;
                AllIngredients.Add(viewModel.Ingredient);
                Ingredient.Ingredient = viewModel.Ingredient;
            }
        }

        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }
        public Lazy<DelegateCommand> AddMultipleCommand { get; }
        public Lazy<DelegateCommand<RecipeIngredientDTO>> RemoveIngredientCommand { get; }

        public Lazy<DelegateCommand> AddCategoryCommand { get; }


        public RecipeIngredientDTO Ingredient { get; set; }
        public ObservableCollection<RecipeIngredientDTO> Ingredients { get; set; }

        public List<IngredientDTO> AllIngredients { get; set; }

        public bool IsCreation { get; set; }

    }
}