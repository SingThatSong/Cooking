using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using Cooking.ServiceLayer;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class IngredientsViewModel
    {
        private readonly IRegionManager regionManager;
        private readonly DialogService dialogUtils;
        private readonly IngredientService ingredientService;
        private readonly IMapper mapper;

        public ObservableCollection<IngredientEdit>? Ingredients { get; private set; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddIngredientCommand { get; }

        public DelegateCommand<IngredientEdit> ViewIngredientCommand { get; }
        public DelegateCommand<IngredientEdit> EditCategoryCommand { get; }
        public DelegateCommand<Guid> DeleteCategoryCommand { get; }
        public DelegateCommand LoadedCommand { get; }

        public IngredientsViewModel(IRegionManager regionManager, 
                                    DialogService dialogUtils, 
                                    IngredientService ingredientService,
                                    IMapper mapper)
        {
            Debug.Assert(regionManager != null);
            Debug.Assert(dialogUtils != null);
            Debug.Assert(ingredientService != null);
            Debug.Assert(mapper != null);

            this.regionManager = regionManager;
            this.dialogUtils = dialogUtils;
            this.ingredientService = ingredientService;
            this.mapper = mapper;
            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);
            ViewIngredientCommand = new DelegateCommand<IngredientEdit>(ViewIngredient);
            AddIngredientCommand = new DelegateCommand(AddRecipe);
            DeleteCategoryCommand = new DelegateCommand<Guid>(DeleteIngredient);
            EditCategoryCommand = new DelegateCommand<IngredientEdit>(EditIngredient);
        }

        private void ViewIngredient(IngredientEdit ingredient)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecepiesViewModel.FilterText), $"{Consts.IngredientSymbol}\"{ingredient.Name}\"" }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(Recepies), parameters);
        }

        private async void EditIngredient(IngredientEdit ingredient)
        {
            var viewModel = new IngredientEditViewModel(ingredientService, ingredient.MapTo<IngredientEdit>());
            await dialogUtils.ShowCustomMessageAsync<IngredientEditView, IngredientEditViewModel>("Редактирование ингредиента", viewModel).ConfigureAwait(false);
            
            if (viewModel.DialogResultOk)
            {
                await ingredientService.UpdateAsync(viewModel.Ingredient.MapTo<Ingredient>()).ConfigureAwait(false);
                var existingRecipe = Ingredients.Single(x => x.ID == ingredient.ID);
                viewModel.Ingredient.MapTo(existingRecipe);
            }
        }

        private void OnLoaded()
        {
            Debug.WriteLine("IngredientsViewModel.OnLoaded");
            var dataDb = ingredientService.GetProjected<IngredientEdit>(mapper);
            Ingredients = new ObservableCollection<IngredientEdit>(dataDb);
        }

        public async void DeleteIngredient(Guid id)
        {
            var result = await DialogCoordinator.Instance.ShowMessageAsync(
                this, 
                "Точно удалить?",
                "Восстановить будет нельзя",
                style: MessageDialogStyle.AffirmativeAndNegative,
                settings: new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Да",
                    NegativeButtonText = "Нет"
                }).ConfigureAwait(true);

            if (result == MessageDialogResult.Affirmative)
            {
                await ingredientService.DeleteAsync(id).ConfigureAwait(true);
                Ingredients!.Remove(Ingredients.Single(x => x.ID == id));
            }
        }

        public async void AddRecipe()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<IngredientEditView, IngredientEditViewModel>("Новый ингредиент").ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var id = await ingredientService.CreateAsync(viewModel.Ingredient.MapTo<Ingredient>()).ConfigureAwait(false);
                viewModel.Ingredient.ID = id;
                Ingredients!.Add(viewModel.Ingredient);
            }
        }
    }
}