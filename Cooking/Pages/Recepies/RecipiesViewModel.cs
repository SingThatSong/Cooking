using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Mappings;
using Cooking.ServiceLayer;
using Data.Context;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using Plafi;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace Cooking.Pages.Recepies
{
    [AddINotifyPropertyChangedInterface]
    public partial class RecepiesViewModel
    {
        public CollectionViewSource RecipiesSource { get; }
        public ObservableCollection<RecipeSelect> Recipies { get; }

        public DelegateCommand AddRecipeCommand { get; }
        public DelegateCommand<Guid> ViewRecipeCommand { get; }
        public DelegateCommand<RecipeMain> EditRecipeCommand { get; }
        public DelegateCommand<Guid> DeleteRecipeCommand { get; }

        public bool IsEditing { get; set; }

        public RecepiesViewModel()
        {
            FilterContext = new FilterContext<RecipeSelect>().AddFilter("#", HasIngredient)
                                                             .AddFilter("$", HasTag);

            dialogUtils = new DialogUtils(this);
            Recipies = GetRecipies();

            RecipiesSource = new CollectionViewSource() { Source = Recipies };
            RecipiesSource.Filter += RecipiesSource_Filter;


            DeleteRecipeCommand = new DelegateCommand<Guid>(DeleteRecipe);

            ViewRecipeCommand = new DelegateCommand<Guid>(ViewRecipe);
            AddRecipeCommand = new DelegateCommand(AddRecipe);
            EditRecipeCommand = new DelegateCommand<RecipeMain>(async (recipe) => {

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

                var viewModel = new RecipeEditViewModel(recipe);
                await dialogUtils.ShowCustomMessageAsync<RecipeEditView, RecipeEditViewModel>("Редактирование рецепта", viewModel);

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

                        if (existing.IngredientGroups != null)
                        {
                            for (int i = 0; i < existing.IngredientGroups.Count; i++)
                            {
                                if (existing.Ingredients != null)
                                {
                                    for (int j = 0; j < existing.IngredientGroups[i].Ingredients.Count; j++)
                                    {
                                        var dbValue = context.RecipeIngredients.Find(existing.IngredientGroups[i].Ingredients[j].ID);
                                        if (dbValue != null)
                                        {
                                            existing.IngredientGroups[i].Ingredients[j] = dbValue;
                                        }
                                    }
                                }
                            }
                        }

                        if (existing.Ingredients != null)
                        {
                            for (int i = 0; i < existing.Ingredients.Count; i++)
                            {
                                var dbValue = context.RecipeIngredients.Find(existing.Ingredients[i].ID);
                                if (dbValue != null)
                                {
                                    existing.Ingredients[i] = dbValue;
                                }
                            }
                        }

                        if (existing.Tags != null)
                        {
                            for (int i = 0; i < existing.Tags.Count; i++)
                            {
                                var dbValue = context.Tags.Find(existing.Tags[i].Tag.ID);
                                if (dbValue != null)
                                {
                                    existing.Tags[i].Tag = dbValue;
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                }
            });
        }

        private void RecipiesSource_Filter(object sender, FilterEventArgs e)
        {
            if (FilterContext == null || !expressionBuilt)
                return;

            if (e.Item is RecipeSelect recipe)
            {
                e.Accepted = FilterContext.Filter(recipe);
            }
        }

        private string filterText;
        private readonly DialogUtils dialogUtils;
        private bool expressionBuilt = false;
        private FilterContext<RecipeSelect> FilterContext { get; set; }
        public string FilterText
        {
            get => filterText;
            set
            {
                if (filterText != value)
                {
                    filterText = value; 
                    FilterContext.BuildExpression(value);
                    expressionBuilt = true;
                    RecipiesSource.View.Refresh();
                }
            }
        }
        private bool HasTag(RecipeSelect recipe, string category)
        {
            var recipeDb = RecipeService.GetRecipe<RecipeFull>(recipe.ID).Result;
            return recipeDb.Tags != null && recipeDb.Tags.Any(x => x.Name.ToUpperInvariant() == category.ToUpperInvariant());
        }

        private bool HasIngredient(RecipeSelect recipe, string category)
        {
            var recipeDb = RecipeService.GetRecipe<RecipeFull>(recipe.ID).Result;

            // Ищем среди ингредиентов
            if (recipeDb.Ingredients != null
                && recipeDb.Ingredients.Any(x => x.Ingredient.Name.ToUpperInvariant() == category.ToUpperInvariant()))
            {
                return true;
            }

            // Ищем среди групп ингредиентов
            if (recipeDb.IngredientGroups != null)
            {
                foreach (var group in recipeDb.IngredientGroups)
                {
                    if (group.Ingredients.Any(x => x.Ingredient.Name.ToUpperInvariant() == category.ToUpperInvariant()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async void DeleteRecipe(Guid recipeId)
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
                await RecipeService.DeleteRecipe(recipeId);
                Recipies.Remove(Recipies.Single(x => x.ID == recipeId));
            }
        }

        public async void ViewRecipe(Guid recipeId)
        {
            await dialogUtils.ShowCustomMessageAsync<RecipeView, RecipeViewModel>(content: new RecipeViewModel(recipeId));
        }

        public async void AddRecipe()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<RecipeView, RecipeEditViewModel>("Новый рецепт");

            if (viewModel.DialogResultOk)
            {
                using (var context = new CookingContext())
                {
                    var recipe = MappingsHelper.MapToRecipe(viewModel.Recipe, context);
                    context.Add(recipe);
                    context.SaveChanges();

                    viewModel.Recipe.ID = recipe.ID;
                    //Recipies.Value.Add(viewModel.Recipe);
                }
            }
        }

        private ObservableCollection<RecipeSelect> GetRecipies()
        {
            var revcipiesDb = RecipeService.GetRecipies();
            return new ObservableCollection<RecipeSelect>(revcipiesDb.Select(x => MapperService.Mapper.Map<RecipeSelect>(x)));
        }
    }
}