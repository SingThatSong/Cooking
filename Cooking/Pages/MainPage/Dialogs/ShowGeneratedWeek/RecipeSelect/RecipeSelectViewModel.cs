using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.MainPage.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using Plafi;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Cooking.Pages.Recepies
{
    public partial class RecipeSelectViewModel : OkCancelViewModel
    {
        public RecipeSelectViewModel() : this(null)
        {

        }

        public Guid SelectedRecipeID { get; set; }

        public RecipeSelectViewModel(DayPlan day = null)
        {
            FilterContext = new FilterContext<RecipeMain>().AddFilter("#", HasIngredient)
                                                           .AddFilter("$", HasTag);

            var dbRecipies = RecipeService.GetRecipies();

            Recipies = dbRecipies.Select(x => MapperService.Mapper.Map<RecipeSelect>(x)).ToList();

            RecipiesSource = new CollectionViewSource() { Source = Recipies };
            RecipiesSource.Filter += RecipiesSource_Filter;

            SelectRecipeCommand = new DelegateCommand<RecipeSelect>(recipe => {
                    foreach(var r in Recipies.Where(x => x.IsSelected))
                    {
                        r.IsSelected = false;
                    }

                    recipe.IsSelected = true;
                    SelectedRecipeID = recipe.ID;
                });

            ViewRecipeCommand = new DelegateCommand<RecipeMain>(ViewRecipe);

            if (day != null)
            {
                var sb = new StringBuilder();

                if (day.NeededDishTypes != null && day.NeededDishTypes.Where(x => x.IsChecked && x.CanBeRemoved).Count() > 0)
                {
                    sb.Append("~");
                    foreach (var dishType in day.NeededDishTypes)
                    {
                        sb.Append(dishType.Name + ",");
                    }
                }

                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(" ");
                }
                

                if (day.NeededMainIngredients != null && day.NeededMainIngredients.Where(x => x.IsChecked && x.CanBeRemoved).Count() > 0)
                {
                    sb.Append("#");
                    foreach (var mainIngredient in day.NeededMainIngredients)
                    {
                        sb.Append(mainIngredient.Name + ",");
                    }
                }

                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }

                FilterText = sb.ToString();
            }
        }

        private void RecipiesSource_Filter(object sender, FilterEventArgs e)
        {
            if (FilterContext == null)
                return;

            if (e.Item is RecipeMain recipe)
            {
                e.Accepted = FilterContext.Filter(recipe);
            }
        }

        private string filterText;
        private FilterContext<RecipeMain> FilterContext { get; set; }
        public string FilterText
        {
            get => filterText;
            set
            {
                if (filterText != value && value != "")
                {
                    filterText = value;
                    FilterContext.BuildExpression(value);
                    RecipiesSource.View.Refresh();
                }
            }
        }

        private bool HasTag(RecipeMain recipe, string category)
        {
            return recipe.Tags != null && recipe.Tags.Any(x => x.Name.ToUpperInvariant() == category.ToUpperInvariant());
        }

        private bool HasIngredient(RecipeMain recipe, string category)
        {
            // Ищем среди ингредиентов
            if (recipe.Ingredients != null
                && recipe.Ingredients.Any(x => x.Ingredient.Name.ToUpperInvariant() == category.ToUpperInvariant()))
            {
                return true;
            }

            // Ищем среди групп ингредиентов
            if (recipe.IngredientGroups != null)
            {
                foreach (var group in recipe.IngredientGroups)
                {
                    if (group.Ingredients.Any(x => x.Ingredient.Name.ToUpperInvariant() == category.ToUpperInvariant()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async void ViewRecipe(RecipeMain recipe)
        {
            var viewModel = new RecipeViewModel(recipe.ID);

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

        public List<RecipeSelect> Recipies;

        public CollectionViewSource RecipiesSource { get; }

        public DelegateCommand<RecipeSelect> SelectRecipeCommand { get; }
        public DelegateCommand<RecipeMain> ViewRecipeCommand { get; }
    }
}