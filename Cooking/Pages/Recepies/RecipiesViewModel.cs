using AutoMapper;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class RecepiesViewModel : INotifyPropertyChanged
    {
        public RecepiesViewModel()
        {
            Recipies = new Lazy<ObservableCollection<RecipeDTO>>(GetCategories);
            ViewRecipeCommand = new Lazy<DelegateCommand<RecipeDTO>>(() => new DelegateCommand<RecipeDTO>(ViewRecipe));
            AddRecipeCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(AddRecipe));
            DeleteRecipeCommand = new Lazy<DelegateCommand<RecipeDTO>>(() => new DelegateCommand<RecipeDTO>(cat => DeleteCategory(cat.ID)));
            EditRecipeCommand = new Lazy<DelegateCommand<RecipeDTO>>(
                () => new DelegateCommand<RecipeDTO>(async (recipe) => {

                    using (var context = new CookingContext())
                    {
                        var existing = context.Recipies
                                              .Where(x => x.ID == recipe.ID)
                                              .Include(x => x.Ingredients)
                                                  .ThenInclude(x => x.Ingredient)
                                              .Include(x => x.Tags)
                                              .Single();

                        Mapper.Map(existing.Ingredients, recipe.Ingredients);
                        Mapper.Map(existing.Tags, recipe.Tags);
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
                                                  .Include(x => x.Ingredients)
                                                      .ThenInclude(x => x.Ingredient)
                                                  .Include(x => x.Tags)
                                                  .Single();
                            
                            Mapper.Map(viewModel.Recipe, existing);

                            for (int i = 0; i < existing.Ingredients.Count; i++)
                            {
                                var dbValue = context.RecipeIngredients.Find(existing.Ingredients[i].ID);
                                if (dbValue != null)
                                {
                                    existing.Ingredients[i] = dbValue;
                                }
                            }

                            for (int i = 0; i < existing.Tags.Count; i++)
                            {
                                if (existing.Tags[i] != null)
                                {
                                    existing.Tags[i] = context.Tags.Find(existing.Tags[i].ID);
                                }
                            }

                            context.SaveChanges();
                        }

                        var existingRecipe = Recipies.Value.Single(x => x.ID == recipe.ID);
                        Mapper.Map(viewModel.Recipe, existingRecipe);
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
                                      .Include(x => x.Ingredients)
                                          .ThenInclude(x => x.Ingredient)
                                      .Include(x => x.Tags)
                                      .Single();

                Mapper.Map(existing.Ingredients, recipe.Ingredients);
                Mapper.Map(existing.Tags, recipe.Tags);
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

                    for (int i = 0; i < recipe.Ingredients.Count; i++)
                    {
                        var dbValue = context.RecipeIngredients.Find(recipe.Ingredients[i].ID);
                        if (dbValue != null)
                        {
                            recipe.Ingredients[i] = dbValue;
                        }
                    }

                    for (int i = 0; i < recipe.Tags.Count; i++)
                    {
                        var dbValue = context.Tags.Find(recipe.Tags[i].ID);
                        if (dbValue != null)
                        {
                            recipe.Tags[i] = dbValue;
                        }
                    }

                    context.Add(recipe);
                    context.SaveChanges();
                }
                viewModel.Recipe.ID = recipe.ID;
                Recipies.Value.Add(viewModel.Recipe);
            }
        }

        public Lazy<ObservableCollection<RecipeDTO>> Recipies { get; }
        private ObservableCollection<RecipeDTO> GetCategories()
        {
            try
            {
                using (var Context = new CookingContext())
                {
                    var originalList = Context.Recipies.ToList();
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
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> AddRecipeCommand { get; }
        public Lazy<DelegateCommand<RecipeDTO>> ViewRecipeCommand { get; }
        public Lazy<DelegateCommand<RecipeDTO>> EditRecipeCommand { get; }
        public Lazy<DelegateCommand<RecipeDTO>> DeleteRecipeCommand { get; }
    }
}