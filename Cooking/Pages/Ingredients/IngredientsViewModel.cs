using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Recepies;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Cooking.Pages.Ingredients
{
    public partial class IngredientsViewModel : INotifyPropertyChanged
    {
        public IngredientsViewModel()
        {
            Ingredients = new Lazy<ObservableCollection<IngredientMain>>(GetIngredients);
            ViewIngredientCommand = new Lazy<DelegateCommand<IngredientMain>>(() => new DelegateCommand<IngredientMain>(ingredient =>
            {
                if (Application.Current.MainWindow.DataContext is MainWindowViewModel mainWindowViewModel)
                {
                    mainWindowViewModel.SelectedMenuItem = mainWindowViewModel.MenuItems[1] as HamburgerMenuIconItem;
                    ((mainWindowViewModel.SelectedMenuItem.Tag as RecepiesView).DataContext as RecepiesViewModel).FilterText = $"#{ingredient.Name}";
                }
            }));
            AddCategoryCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(AddRecipe));
            DeleteCategoryCommand = new Lazy<DelegateCommand<IngredientMain>>(() => new DelegateCommand<IngredientMain>(cat => DeleteCategory(cat.ID)));
            EditCategoryCommand = new Lazy<DelegateCommand<IngredientMain>>(
                () => new DelegateCommand<IngredientMain>(async (ingredient) => {

                    var viewModel = new IngredientEditViewModel(Mapper.Map<IngredientMain>(ingredient));

                    var dialog = new CustomDialog()
                    {
                        Title = "Редактирование ингредиента",
                        Content = new IngredientEditView()
                        {
                            DataContext = viewModel
                        }
                    };
                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

                    if (viewModel.DialogResultOk)
                    {
                        using (var context = new CookingContext())
                        {
                            var existing = context.Ingredients.Find(ingredient.ID);
                            Mapper.Map(viewModel.Ingredient, existing);
                            context.SaveChanges();
                        }

                        var existingRecipe = Ingredients.Value.Single(x => x.ID == ingredient.ID);
                        Mapper.Map(viewModel.Ingredient, existingRecipe);
                    }
                }));
        }

        public async void DeleteCategory(Guid recipeId)
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
                });

            if (result == MessageDialogResult.Affirmative)
            {
                using (var context = new CookingContext())
                {
                    var category = await context.Ingredients.FindAsync(recipeId);
                    context.Ingredients.Remove(category);
                    context.SaveChanges();
                }

                Ingredients.Value.Remove(Ingredients.Value.Single(x => x.ID == recipeId));
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
                Ingredients.Value.Add(viewModel.Ingredient);
            }
        }

        public Lazy<ObservableCollection<IngredientMain>> Ingredients { get; }
        private ObservableCollection<IngredientMain> GetIngredients()
        {
            try
            {
                using (var Context = new CookingContext())
                {
                    var originalList = Context.Ingredients.ToList();
                    return new ObservableCollection<IngredientMain>(
                        originalList.OrderBy(x => x.Name).Select(x =>
                        {
                            var dto = Mapper.Map<IngredientMain>(x);
                            dto.PropertyChanged += Dto_PropertyChanged;
                            return dto;
                        })
                    );
                }
            }
            catch
            {
                return null;
            }
        }

        private void Dto_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            using (var Context = new CookingContext())
            {
                var dto = sender as IngredientMain;
                var original = Context.Ingredients.Find(dto.ID);
                Mapper.Map(dto, original);
                Context.SaveChanges();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> AddCategoryCommand { get; }
        
        public Lazy<DelegateCommand<IngredientMain>> ViewIngredientCommand { get; }
        public Lazy<DelegateCommand<IngredientMain>> EditCategoryCommand { get; }
        public Lazy<DelegateCommand<IngredientMain>> DeleteCategoryCommand { get; }
        
        public bool IsEditing { get; set; }
    }
}