using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Cooking.Pages.Recepies
{
    public partial class RecepiesViewModel : INotifyPropertyChanged
    {
        public RecepiesViewModel()
        {
            Recipies = new Lazy<ObservableCollection<RecipeDTO>>(GetRecipies);
            RecipiesSource = new CollectionViewSource() { Source = Recipies.Value };
            RecipiesSource.Filter += RecipiesSource_Filter;

            ViewRecipeCommand = new Lazy<DelegateCommand<RecipeDTO>>(() => new DelegateCommand<RecipeDTO>(ViewRecipe));
            AddRecipeCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(AddRecipe));
            DeleteRecipeCommand = new Lazy<DelegateCommand<RecipeDTO>>(() => new DelegateCommand<RecipeDTO>(cat => DeleteCategory(cat.ID)));
            EditRecipeCommand = new Lazy<DelegateCommand<RecipeDTO>>(
                () => new DelegateCommand<RecipeDTO>(async (recipe) => {

                    using (var context = new CookingContext())
                    {
                        var existing = context.Recipies
                                              .Where(x => x.ID == recipe.ID)
                                              .Include(x => x.IngredientGroups)
                                                  .ThenInclude(x => x.Ingredients)
                                                      .ThenInclude(x => x.Ingredient)
                                              .Include(x => x.Ingredients)
                                                  .ThenInclude(x => x.Ingredient)
                                              .Include(x => x.Tags)
                                                  .ThenInclude(x => x.Tag)
                                              .Single();

                        Mapper.Map(existing, recipe);
                    }

                    var viewModel = new RecipeEditViewModel(Mapper.Map<RecipeDTO>(recipe));

                    var dialog = new CustomDialog()
                    {
                        Title = "Редактирование рецепта",
                        Content = new RecipeEditView()
                        {
                            DataContext = viewModel
                        }
                    };
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

                    do
                    {
                        await dialog.WaitUntilUnloadedAsync();
                    }
                    while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

                    if (viewModel.DialogResultOk)
                    {
                        using (var context = new CookingContext())
                        {

                            var existing = context.Recipies
                                                  .Where(x => x.ID == recipe.ID)
                                                  .Include(x => x.IngredientGroups)
                                                      .ThenInclude(x => x.Ingredients)
                                                          .ThenInclude(x => x.Ingredient)
                                                  .Include(x => x.Ingredients)
                                                      .ThenInclude(x => x.Ingredient)
                                                  .Include(x => x.Tags)
                                                    .ThenInclude(x => x.Tag)
                                                  .Single();

                            Mapper.Map(viewModel.Recipe, existing);

                            context.SaveChanges();
                        }

                        var existingRecipe = Recipies.Value.Single(x => x.ID == recipe.ID);
                        Mapper.Map(viewModel.Recipe, existingRecipe);
                    }
                }));
        }

        private void RecipiesSource_Filter(object sender, FilterEventArgs e)
        {
            if (RecipeFilter == null)
                return;

            if(e.Item is RecipeDTO recipe)
            {
                e.Accepted = RecipeFilter.FilterRecipe(recipe);
            }
        }

        private string filterText;
        private RecipeFilter RecipeFilter { get; set; }
        public string FilterText
        {
            get => filterText;
            set
            {
                if(filterText != value)
                {
                    filterText = value;
                    RecipeFilter = new RecipeFilter(value);
                    RecipiesSource.View.Refresh();
                }
            }
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
                    var category = await context.Recipies.FindAsync(recipeId);
                    context.Recipies.Remove(category);
                    context.SaveChanges();
                }

                Recipies.Value.Remove(Recipies.Value.Single(x => x.ID == recipeId));
            }
        }

        public async void ViewRecipe(RecipeDTO recipe)
        {
            using (var context = new CookingContext())
            {
                var existing = context.Recipies
                                      .Where(x => x.ID == recipe.ID)
                                      .Include(x => x.IngredientGroups)
                                          .ThenInclude(x => x.Ingredients)
                                              .ThenInclude(x => x.Ingredient)
                                      .Include(x => x.Ingredients)
                                          .ThenInclude(x => x.Ingredient)
                                      .Include(x => x.Tags)
                                          .ThenInclude(x => x.Tag)
                                      .Single();

                Mapper.Map(existing, recipe);
            }

            var viewModel = new RecipeViewModel(recipe);

            var dialog = new CustomDialog()
            {
                Content = new RecipeView()
                {
                    DataContext = viewModel
                }
            };

            await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
            await dialog.WaitUntilUnloadedAsync();
        }

        public async void AddRecipe()
        {
            var viewModel = new RecipeEditViewModel();

            var dialog = new CustomDialog()
            {
                Title = "Новый рецепт",
                Content = new RecipeEditView()
                {
                    DataContext = viewModel
                }
            };

            var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

            await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

            do
            {
                await dialog.WaitUntilUnloadedAsync();
            }
            while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

            if (viewModel.DialogResultOk)
            {
                var recipe = Mapper.Map<Recipe>(viewModel.Recipe);
                using (var context = new CookingContext())
                {
                    if (recipe.IngredientGroups != null)
                    {
                        for (int i = 0; i < recipe.IngredientGroups.Count; i++)
                        {
                            if (recipe.Ingredients != null)
                            {
                                for (int j = 0; j < recipe.IngredientGroups[i].Ingredients.Count; j++)
                                {
                                    var dbValue = context.RecipeIngredients.Find(recipe.IngredientGroups[i].Ingredients[j].ID);
                                    if (dbValue != null)
                                    {
                                        recipe.IngredientGroups[i].Ingredients[j] = dbValue;
                                    }
                                }
                            }
                        }
                    }

                    if (recipe.Ingredients != null)
                    {
                        for (int i = 0; i < recipe.Ingredients.Count; i++)
                        {
                            var dbValue = context.RecipeIngredients.Find(recipe.Ingredients[i].ID);
                            if (dbValue != null)
                            {
                                recipe.Ingredients[i] = dbValue;
                            }
                        }
                    }

                    if (recipe.Tags != null)
                    {
                        for (int i = 0; i < recipe.Tags.Count; i++)
                        {
                            var dbValue = context.Tags.Find(recipe.Tags[i].Tag.ID);
                            if (dbValue != null)
                            {
                                recipe.Tags[i].Tag = dbValue;
                            }
                        }
                    }

                    context.Add(recipe);
                    context.SaveChanges();
                }
                viewModel.Recipe.ID = recipe.ID;
                Recipies.Value.Add(viewModel.Recipe);
            }
        }

        public CollectionViewSource RecipiesSource { get; }

        public Lazy<ObservableCollection<RecipeDTO>> Recipies { get; }
        private ObservableCollection<RecipeDTO> GetRecipies()
        {
            try
            {
                using (var Context = new CookingContext())
                {
                    var originalList = Context.Recipies
                                              .Include(x => x.Ingredients)
                                                  .ThenInclude(x => x.Ingredient)
                                              .Include(x => x.Tags)
                                                  .ThenInclude(x => x.Tag)
                                              .ToList();

                    return new ObservableCollection<RecipeDTO>(
                        originalList.OrderBy(x => x.Name).Select(x =>
                        {
                            var dto = Mapper.Map<RecipeDTO>(x);
                            return dto;
                        })
                    );
                }
            }
            catch
            {
                return new ObservableCollection<RecipeDTO>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> AddRecipeCommand { get; }
        public Lazy<DelegateCommand<RecipeDTO>> ViewRecipeCommand { get; }
        public Lazy<DelegateCommand<RecipeDTO>> EditRecipeCommand { get; }
        public Lazy<DelegateCommand<RecipeDTO>> DeleteRecipeCommand { get; }

        public bool IsEditing { get; set; }
    }
}