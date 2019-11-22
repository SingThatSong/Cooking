using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Recepies;
using Cooking.ServiceLayer;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Cooking.Pages.Ingredients
{
    [AddINotifyPropertyChangedInterface]
    public partial class IngredientsViewModel
    {
        public ObservableCollection<IngredientMain>? Ingredients { get; private set; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddIngredientCommand { get; }

        public DelegateCommand<IngredientMain> ViewIngredientCommand { get; }
        public DelegateCommand<IngredientMain> EditCategoryCommand { get; }
        public DelegateCommand<Guid> DeleteCategoryCommand { get; }
        public DelegateCommand LoadedCommand { get; }

        public IngredientsViewModel()
        {
            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);
            ViewIngredientCommand = new DelegateCommand<IngredientMain>(ViewIngredient);
            AddIngredientCommand = new DelegateCommand(AddRecipe);
            DeleteCategoryCommand = new DelegateCommand<Guid>(DeleteIngredient);
            EditCategoryCommand = new DelegateCommand<IngredientMain>(EditIngredient);
        }

        private void ViewIngredient(IngredientMain ingredient)
        {
            if (Application.Current.MainWindow.DataContext is MainWindowViewModel mainWindowViewModel)
            {
                if (mainWindowViewModel.MenuItems[1] is HamburgerMenuIconItem hamburgerMenuIconItem)
                {
                    mainWindowViewModel.SelectedMenuItem = hamburgerMenuIconItem;
                    if (mainWindowViewModel.SelectedMenuItem.Tag is RecepiesView recepiesView)
                    {
                        if (recepiesView.DataContext is RecepiesViewModel recepiesViewModel)
                        {
                            recepiesViewModel.FilterText = $"{Consts.IngredientSymbol}{ingredient.Name}";
                        }
                    }
                }
            }
        }

        private async void EditIngredient(IngredientMain ingredient)
        {
            var viewModel = new IngredientEditViewModel(ingredient.MapTo<IngredientMain>());
            await new DialogUtils(this).ShowCustomMessageAsync<IngredientEditView, IngredientEditViewModel>("Редактирование ингредиента", viewModel).ConfigureAwait(false);
            
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
                                           .MapTo<ObservableCollection<IngredientMain>>();
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
            var viewModel = await new DialogUtils(this).ShowCustomMessageAsync<IngredientEditView, IngredientEditViewModel>("Новый ингредиент").ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var id = await IngredientService.CreateAsync(viewModel.Ingredient.MapTo<Ingredient>()).ConfigureAwait(false);
                viewModel.Ingredient.ID = id;
                Ingredients!.Add(viewModel.Ingredient);
            }
        }
    }
}