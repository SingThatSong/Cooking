using AutoMapper;
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
                () => new DelegateCommand(async () => {
                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            AddMultipleCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    Ingredients = Ingredients ?? new List<RecipeIngredientDTO>();
                    Ingredient.Order = Ingredients.Count + 1;
                    Ingredients.Add(Ingredient);
                    Ingredient = new RecipeIngredientDTO();
                },
                canExecute: () => IsCreation));

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

        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }
        public Lazy<DelegateCommand> AddMultipleCommand { get; }
        public RecipeIngredientDTO Ingredient { get; set; }
        public List<RecipeIngredientDTO> Ingredients { get; set; }

        public List<IngredientDTO> AllIngredients { get; set; }

        public bool IsCreation { get; set; }

    }
}