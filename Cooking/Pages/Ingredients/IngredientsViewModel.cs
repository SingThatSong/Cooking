using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using Cooking.ServiceLayer;
using Data.Model;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class IngredientsViewModel
    {
        private readonly IRegionManager regionManager;
        private readonly DialogService dialogUtils;

        public ObservableCollection<IngredientEdit>? Ingredients { get; private set; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddIngredientCommand { get; }

        public DelegateCommand<IngredientEdit> ViewIngredientCommand { get; }
        public DelegateCommand<IngredientEdit> EditCategoryCommand { get; }
        public DelegateCommand<Guid> DeleteCategoryCommand { get; }
        public DelegateCommand LoadedCommand { get; }

        public IngredientsViewModel(IRegionManager regionManager, DialogService dialogUtils)
        {
            Debug.Assert(regionManager != null);
            Debug.Assert(dialogUtils != null);

            this.regionManager = regionManager;
            this.dialogUtils = dialogUtils;

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
            var viewModel = new IngredientEditViewModel(ingredient.MapTo<IngredientEdit>());
            await dialogUtils.ShowCustomMessageAsync<IngredientEditView, IngredientEditViewModel>("Редактирование ингредиента", viewModel).ConfigureAwait(false);
            
            if (viewModel.DialogResultOk)
            {
                await IngredientService.UpdateIngredientAsync(viewModel.Ingredient.MapTo<Ingredient>()).ConfigureAwait(false);
                var existingRecipe = Ingredients.Single(x => x.ID == ingredient.ID);
                viewModel.Ingredient.MapTo(existingRecipe);
            }
        }

        private void OnLoaded()
        {
            Debug.WriteLine("IngredientsViewModel.OnLoaded");
            Ingredients = IngredientService.GetIngredients<IngredientData>()
                                           .MapTo<ObservableCollection<IngredientEdit>>();
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
                await IngredientService.DeleteAsync(id).ConfigureAwait(true);
                Ingredients!.Remove(Ingredients.Single(x => x.ID == id));
            }
        }

        public async void AddRecipe()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<IngredientEditView, IngredientEditViewModel>("Новый ингредиент").ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var id = await IngredientService.CreateAsync(viewModel.Ingredient.MapTo<Ingredient>()).ConfigureAwait(false);
                viewModel.Ingredient.ID = id;
                Ingredients!.Add(viewModel.Ingredient);
            }
        }
    }
}