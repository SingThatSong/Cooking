using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;

using Cooking.ServiceLayer;
using Data.Model;
using PropertyChanged;
using ServiceLayer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages.Ingredients
{
    [AddINotifyPropertyChangedInterface]
    public partial class RecipeIngredientEditViewModel : OkCancelViewModel
    {
        private readonly DialogService dialogUtils;
        private readonly IngredientService ingredientService;

        public bool IsCreation { get; set; }
        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

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
                                             RecipeIngredientEdit? ingredient = null)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(ingredientService != null);

            this.dialogUtils = dialogUtils;
            this.ingredientService = ingredientService;
            Ingredient = ingredient ?? new RecipeIngredientEdit();

            AddMultipleCommand = new DelegateCommand(AddMultiple, canExecute: () => IsCreation);
            RemoveIngredientCommand = new DelegateCommand<RecipeIngredientEdit>(RemoveIngredient);
            AddCategoryCommand = new AsyncDelegateCommand(AddRecipe);

            AllIngredients = ingredientService.GetProjected<IngredientEdit>(mapper);
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
                var id = await ingredientService.CreateAsync(viewModel.Ingredient.MapTo<Ingredient>())
                                                .ConfigureAwait(false);
                viewModel.Ingredient.ID = id;
                AllIngredients.Add(viewModel.Ingredient);
                Ingredient.Ingredient = viewModel.Ingredient;
            }
        }
    }
}