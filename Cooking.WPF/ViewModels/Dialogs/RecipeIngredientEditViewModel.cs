﻿using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Validation;
using Cooking.WPF.Views;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for editing ingredient in recipe.
    /// </summary>
    public partial class RecipeIngredientEditViewModel : OkCancelViewModel
    {
        private readonly DialogService dialogService;
        private readonly IngredientService ingredientService;
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeIngredientEditViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="ingredientService">Ingredient service dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        /// <param name="measureUnitService">Provider for a list of measurement units.</param>
        /// <param name="ingredient">Ingredient to edit.</param>
        public RecipeIngredientEditViewModel(DialogService dialogService,
                                             IngredientService ingredientService,
                                             ILocalization localization,
                                             MeasureUnitService measureUnitService,
                                             RecipeIngredientEdit ingredient)
            : base(dialogService)
        {
            this.dialogService = dialogService;
            this.ingredientService = ingredientService;
            this.localization = localization;

            Ingredient = ingredient;
            MeasurementUnits = measureUnitService.GetAll();

            AddMultipleCommand = new DelegateCommand(AddMultiple, canExecute: CanOk);
            RemoveIngredientCommand = new DelegateCommand<RecipeIngredientEdit>(RemoveIngredient);
            CreateIngredientCommand = new AsyncDelegateCommand(CreateIngredient);

            AllIngredients = ingredientService.GetAllProjected<IngredientEdit>();
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        /// <summary>
        /// Gets or sets a value indicating whether indication whether recipe ingredient is created.
        /// </summary>
        public bool IsCreation { get; set; }

        /// <summary>
        /// Gets values provider for measurement unit selection.
        /// </summary>
        public List<MeasureUnit> MeasurementUnits { get; }

        /// <summary>
        /// Gets localized caption for count.
        /// </summary>
        public string? CountCaption => localization.GetLocalizedString("Count");

        /// <summary>
        /// Gets localized caption for ingredient.
        /// </summary>
        public string? IngredientCaption => localization.GetLocalizedString("Ingredient");

        /// <summary>
        /// Gets localized caption for measurement unit.
        /// </summary>
        public string? MeasurementUnitCaption => localization.GetLocalizedString("MeasurementUnit");

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public DelegateCommand LoadedCommand { get; }

        /// <summary>
        /// Gets command for saving current ingredient in recipe to cache and creating a new one.
        /// </summary>
        public DelegateCommand AddMultipleCommand { get; }

        /// <summary>
        /// Gets command to delete ingredient in recipe from cache.
        /// </summary>
        public DelegateCommand<RecipeIngredientEdit> RemoveIngredientCommand { get; }

        /// <summary>
        /// Gets create new ingredient.
        /// </summary>
        public AsyncDelegateCommand CreateIngredientCommand { get; }

        /// <summary>
        /// Gets or sets crrently edited ingredient.
        /// </summary>
        public RecipeIngredientEdit Ingredient { get; set; }

        /// <summary>
        /// Gets cache for storing added ingredients.
        /// </summary>
        public ObservableCollection<RecipeIngredientEdit>? Ingredients { get; private set; }

        /// <summary>
        /// Gets all ingredients for dropdown.
        /// </summary>
        public List<IngredientEdit> AllIngredients { get; }

        /// <summary>
        /// Ingredient changed callback. Injected via PropertyChanged.
        /// </summary>
        public void OnIngredientChanged() => Ingredient.Ingredient = AllIngredients?.FirstOrDefault(x => x.ID == Ingredient.Ingredient?.ID);

        /// <inheritdoc/>
        protected override bool CanOk() => Ingredient.IsValid();

        // WARNING: this is a crunch
        // When we open ingredient creation dialog second+ time, validation cannot see Ingredient being a required property, but when we change it's value - everything is ok
        // There is no such behaviour when using navigation, so it seems it's something Mahapps-related
        private void OnLoaded()
        {
            IngredientEdit? backup = Ingredient.Ingredient;
            Ingredient.Ingredient = new IngredientEdit();
            Ingredient.Ingredient = backup;
        }

        private void RemoveIngredient(RecipeIngredientEdit i) => Ingredients!.Remove(i);

        private void AddMultiple()
        {
            Ingredients ??= new ObservableCollection<RecipeIngredientEdit>();
            Ingredient.Order = Ingredients.Count + 1;
            Ingredients.Add(Ingredient);
            Ingredient = new RecipeIngredientEdit();
        }

        private async Task CreateIngredient()
        {
            IngredientEditViewModel viewModel = await dialogService.ShowCustomMessageAsync<IngredientEditView, IngredientEditViewModel>(localization.GetLocalizedString("NewIngredient"));

            if (viewModel.DialogResultOk)
            {
                Guid id = await ingredientService.CreateAsync(viewModel.Ingredient);
                viewModel.Ingredient.ID = id;
                AllIngredients.Add(viewModel.Ingredient);
                Ingredient.Ingredient = viewModel.Ingredient;
            }
        }
    }
}