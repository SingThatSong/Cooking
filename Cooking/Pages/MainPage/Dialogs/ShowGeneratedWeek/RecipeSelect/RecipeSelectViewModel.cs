﻿using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Dialogs;
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

        public RecipeSelectViewModel(DayPlan? day = null)
        {
            FilterContext = new FilterContext<RecipeEdit>().AddFilter(Consts.IngredientSymbol, HasIngredient)
                                                           .AddFilter(Consts.TagSymbol, HasTag);

            var dbRecipies = RecipeService.GetRecipies();

            _recipies = dbRecipies.Select(x => MapperService.Mapper.Map<RecipeSelect>(x)).ToList();

            RecipiesSource = new CollectionViewSource() { Source = _recipies };
            RecipiesSource.Filter += RecipiesSource_Filter;

            SelectRecipeCommand = new DelegateCommand<RecipeSelect>(recipe => {
                    foreach(var r in _recipies.Where(x => x.IsSelected))
                    {
                        r.IsSelected = false;
                    }

                    recipe.IsSelected = true;
                    SelectedRecipeID = recipe.ID;
                });

            ViewRecipeCommand = new DelegateCommand<RecipeEdit>(ViewRecipe);

            if (day != null)
            {
                var sb = new StringBuilder();

                if (day.NeededDishTypes != null && day.NeededDishTypes.Any(x => x.IsChecked && x.CanBeRemoved))
                {
                    sb.Append(Consts.TagSymbol);
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
                

                if (day.NeededMainIngredients != null && day.NeededMainIngredients.Any(x => x.IsChecked && x.CanBeRemoved))
                {
                    sb.Append(Consts.IngredientSymbol);
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

            if (e.Item is RecipeEdit recipe)
            {
                e.Accepted = FilterContext.Filter(recipe);
            }
        }

        private string? filterText;
        private FilterContext<RecipeEdit> FilterContext { get; set; }
        public string? FilterText
        {
            get => filterText;
            set
            {
                if (filterText != value && string.IsNullOrEmpty(value))
                {
                    filterText = value;
                    FilterContext.BuildExpression(value);
                    RecipiesSource.View.Refresh();
                }
            }
        }

        private bool HasTag(RecipeEdit recipe, string category)
        {
            return recipe.Tags != null && recipe.Tags.Any(x => x.Name?.ToUpperInvariant() == category.ToUpperInvariant());
        }

        private bool HasIngredient(RecipeEdit recipe, string category)
        {
            // Ищем среди ингредиентов
            if (recipe.Ingredients != null
                && recipe.Ingredients.Any(x => x.Ingredient?.Name?.ToUpperInvariant() == category.ToUpperInvariant()))
            {
                return true;
            }

            // Ищем среди групп ингредиентов
            if (recipe.IngredientGroups != null)
            {
                foreach (var group in recipe.IngredientGroups)
                {
                    if (group.Ingredients.Any(x => x.Ingredient?.Name?.ToUpperInvariant() == category.ToUpperInvariant()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async void ViewRecipe(RecipeEdit recipe)
        {
            var viewModel = new RecipeViewModel(recipe.ID);

            var dialog = new CustomDialog()
            {
                Content = new RecipeView()
                {
                    DataContext = viewModel
                }
            };

            await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog).ConfigureAwait(false);
            await dialog.WaitUntilUnloadedAsync().ConfigureAwait(false);
        }

        private readonly List<RecipeSelect> _recipies;

        public RecipeSelect? SelectedRecipe => _recipies.FirstOrDefault(x => x.IsSelected);

        public CollectionViewSource RecipiesSource { get; }

        public DelegateCommand<RecipeSelect> SelectRecipeCommand { get; }
        public DelegateCommand<RecipeEdit> ViewRecipeCommand { get; }
    }
}