using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Validation;
using Cooking.WPF.Views;
using FluentValidation;
using PropertyChanged;
using Validar;
using WPF.Commands;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for editing ingredient in recipe.
    /// </summary>
    [InjectValidation]
    public partial class RecipeIngredientEditViewModel : OkCancelViewModel
    {
        private readonly DialogService dialogService;
        private readonly CRUDService<Ingredient> ingredientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeIngredientEditViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="ingredientService">Ingredient service dependency.</param>
        /// <param name="measureUnitService">Provider for a list of measurement units.</param>
        /// <param name="ingredient">Ingredient to edit.</param>
        public RecipeIngredientEditViewModel(DialogService dialogService,
                                             CRUDService<Ingredient> ingredientService,
                                             CRUDService<MeasureUnit> measureUnitService,
                                             RecipeIngredientEdit ingredient)
            : base(dialogService)
        {
            this.dialogService = dialogService;
            this.ingredientService = ingredientService;

            Ingredient = ingredient;
            MeasurementUnits = measureUnitService.GetAll();

            AddMultipleCommand = new DelegateCommand(AddMultiple, canExecute: CanOk);
            RemoveIngredientCommand = new DelegateCommand<RecipeIngredientEdit>(RemoveIngredient);
            CreateIngredientCommand = new AsyncDelegateCommand(CreateIngredientAsync);

            AllIngredients = ingredientService.GetAllProjected<IngredientEdit>(callAfterMap: false);
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
        [DoNotCheckEquality]
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

        private void RemoveIngredient(RecipeIngredientEdit i) => Ingredients!.Remove(i);

        private void AddMultiple()
        {
            Ingredients ??= new ObservableCollection<RecipeIngredientEdit>();
            Ingredient.Order = Ingredients.Count + 1;
            Ingredients.Add(Ingredient);
            Ingredient = new RecipeIngredientEdit();
        }

        private async Task CreateIngredientAsync()
        {
            IngredientEditViewModel viewModel = await dialogService.ShowCustomLocalizedMessageAsync<IngredientEditView, IngredientEditViewModel>("NewIngredient");

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