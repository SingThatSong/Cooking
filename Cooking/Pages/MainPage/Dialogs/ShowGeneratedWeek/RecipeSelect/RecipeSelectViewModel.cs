using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Dialogs;
using Cooking.ServiceLayer.Projections;
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
            FilterContext = new FilterContext<RecipeSelect>().AddFilter(Consts.IngredientSymbol, HasIngredient)
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
                    foreach (var dishType in day.NeededDishTypes)
                    {
                        sb.Append($"{Consts.TagSymbol}\"{dishType.Name}\"");

                        if (dishType != day.NeededDishTypes.Last())
                        {
                            sb.Append($" or ");
                        }
                    }
                }

                if (day.NeededMainIngredients != null && day.NeededMainIngredients.Any(x => x.IsChecked && x.CanBeRemoved))
                {
                    var needEnd = false;
                    if (sb.Length > 0)
                    {
                        if (day.NeededDishTypes != null && day.NeededDishTypes.Count > 1)
                        {
                            sb.Insert(0, '(');
                            sb.Append(")");
                        }

                        sb.Append(" and ");

                        if (day.NeededMainIngredients.Count > 1)
                        {
                            sb.Append("(");
                            needEnd = true;
                        }
                    }

                    foreach (var mainIngredient in day.NeededMainIngredients)
                    {
                        sb.Append($"{Consts.TagSymbol}\"{mainIngredient.Name}\"");

                        if (mainIngredient != day.NeededMainIngredients.Last())
                        {
                            sb.Append($" or ");
                        }
                    }

                    if (needEnd)
                    {
                        sb.Append(')');
                    }
                }

                FilterText = sb.ToString();
            }
        }

        private bool built = false;
        private void RecipiesSource_Filter(object sender, FilterEventArgs e)
        {
            if (FilterContext == null || !built)
                return;

            if (e.Item is RecipeSelect recipe)
            {
                e.Accepted = FilterContext.Filter(recipe);
            }
        }

        private string? filterText;
        private FilterContext<RecipeSelect> FilterContext { get; set; }
        public string? FilterText
        {
            get => filterText;
            set
            {
                if (filterText != value && !string.IsNullOrEmpty(value))
                {
                    built = true;
                    FilterContext.BuildExpression(value);
                    RecipiesSource.View.Refresh();
                }
                else
                {
                    built = false;
                }

                filterText = value;
            }
        }

        // TODO: duplicate from RecipiesViewModel.cs
        private readonly Dictionary<Guid, RecipeFull> recipeCache = new Dictionary<Guid, RecipeFull>();
        private bool HasTag(RecipeSelect recipe, string category)
        {
            RecipeFull recipeDb;

            if (recipeCache.ContainsKey(recipe.ID))
            {
                recipeDb = recipeCache[recipe.ID];
            }
            else
            {
                recipeDb = RecipeService.GetProjection<RecipeFull>(recipe.ID);
                recipeCache.Add(recipe.ID, recipeDb);
            }

            return recipeDb.Tags != null && recipeDb.Tags.Any(x => x.Name.ToUpperInvariant() == category.ToUpperInvariant());
        }

        private bool HasIngredient(RecipeSelect recipe, string category)
        {
            RecipeFull recipeDb;

            if (recipeCache.ContainsKey(recipe.ID))
            {
                recipeDb = recipeCache[recipe.ID];
            }
            else
            {
                recipeDb = RecipeService.GetProjection<RecipeFull>(recipe.ID);
                recipeCache.Add(recipe.ID, recipeDb);
            }

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