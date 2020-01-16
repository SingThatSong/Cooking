using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.WPF.Helpers;
using Cooking.Data.Model;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages.Ingredients
{
    public partial class RecipeIngredientEditViewModel : OkCancelViewModel
    {
        private readonly DialogService dialogUtils;
        private readonly IngredientService ingredientService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        public bool IsCreation { get; set; }
        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public string? CountCaption => localization.GetLocalizedString("Count");
        public string? IngredientCaption => localization.GetLocalizedString("Ingredient");
        public string? MeasurementUnitCaption => localization.GetLocalizedString("MeasurementUnit");
        public DelegateCommand LoadedCommand { get; }
        public DelegateCommand AddMultipleCommand { get; }
        public DelegateCommand<RecipeIngredientEdit> RemoveIngredientCommand { get; }

        public AsyncDelegateCommand AddCategoryCommand { get; }


        public RecipeIngredientEdit Ingredient { get; set; }
        public void OnIngredientChanged()
        {
            Ingredient.Ingredient = AllIngredients?.FirstOrDefault(x => x.ID == Ingredient.Ingredient?.ID);
        }
        public ObservableCollection<RecipeIngredientEdit>? Ingredients { get; private set; }

        public List<IngredientEdit> AllIngredients { get; }

        public RecipeIngredientEditViewModel(DialogService dialogUtils, 
                                             IngredientService ingredientService, 
                                             IMapper mapper,
                                             ILocalization localization,
                                             RecipeIngredientEdit? ingredient = null) : base(dialogUtils)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(ingredientService != null);
            Debug.Assert(mapper != null);
            Debug.Assert(localization != null);

            this.dialogUtils = dialogUtils;
            this.ingredientService = ingredientService;
            this.mapper = mapper;
            this.localization = localization;
            Ingredient = ingredient ?? new RecipeIngredientEdit();

            AddMultipleCommand = new DelegateCommand(AddMultiple, canExecute: CanMultipleOk);
            RemoveIngredientCommand = new DelegateCommand<RecipeIngredientEdit>(RemoveIngredient);
            AddCategoryCommand = new AsyncDelegateCommand(AddRecipe);

            AllIngredients = ingredientService.GetProjected<IngredientEdit>(mapper);
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        // WARNING: this is a crunch
        // When we open ingredient creation dialog second+ time, validation cannot see Ingredient being a required property, but when we change it's value - everything is ok
        // There is no such behaviour when using navigation, so it seems it's something Mahapps-related
        private void OnLoaded()
        {
            IngredientEdit? backup = Ingredient.Ingredient;
            Ingredient.Ingredient = new IngredientEdit();
            Ingredient.Ingredient = backup;
        }

        protected override bool CanOk()
        {
            if (Ingredient is INotifyDataErrorInfo dataErrorInfo)
            {
                return !dataErrorInfo.HasErrors;
            }
            else
            {
                return true;
            }
        }

        private bool CanMultipleOk() => CanOk() && IsCreation;

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
            IngredientEditViewModel viewModel = await dialogUtils.ShowCustomMessageAsync<IngredientEditView, IngredientEditViewModel>(localization.GetLocalizedString("NewIngredient"))
                                             .ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                Guid id = await ingredientService.CreateAsync(mapper.Map<Ingredient>(viewModel.Ingredient))
                                                 .ConfigureAwait(false);
                viewModel.Ingredient.ID = id;
                AllIngredients.Add(viewModel.Ingredient);
                Ingredient.Ingredient = viewModel.Ingredient;
            }
        }
    }
}