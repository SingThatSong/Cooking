using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for recipe selection from a list of recipies.
    /// </summary>
    public class RecipeSelectViewModel : OkCancelViewModel
    {
        private readonly List<RecipeListViewDto> recipies;
        private readonly RecipeService recipeService;
        private readonly RecipeFiltrator recipeFiltrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeSelectViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="recipeService">Recipe service dependency.</param>
        /// <param name="recipeFiltrator">RecipeFiltrator service dependency.</param>
        /// <param name="day">Day, which settings will be user for filtering.</param>
        /// <param name="garnishSelect">Select garnish.</param>
        public RecipeSelectViewModel(DialogService dialogService,
                                     RecipeService recipeService,
                                     RecipeFiltrator recipeFiltrator,
                                     DayPlan? day = null,
                                     bool garnishSelect = false)
            : base(dialogService)
        {
            this.recipeService = recipeService;
            this.recipeFiltrator = recipeFiltrator;
            if (garnishSelect && day != null)
            {
                List<Guid> possibleGarnishes = day.Recipe!.Garnishes.ConvertAll(x => x.ID);
                recipies = recipeService.GetProjected<RecipeListViewDto>(x => possibleGarnishes.Contains(x.ID));
            }
            else
            {
                recipies = recipeService.GetAllProjected<RecipeListViewDto>();
            }

            // CollectionViewSource must be created on UI thread
            Application.Current.Dispatcher.Invoke(() => RecipiesSource = new CollectionViewSource() { Source = recipies });

            if (!garnishSelect && day != null)
            {
                // TODO: Prepare string separately
                var sb = new StringBuilder();

                if (day.NeededDishTypes?.Any(x => x.IsChecked && x.CanBeRemoved) == true)
                {
                    foreach (TagEdit dishType in day.NeededDishTypes)
                    {
                        sb.Append(Consts.TagSymbol)
                          .Append('"')
                          .Append(dishType.Name)
                          .Append('"');

                        if (dishType != day.NeededDishTypes.Last())
                        {
                            sb.Append(" or ");
                        }
                    }
                }

                if (day.NeededMainIngredients?.Any(x => x.IsChecked && x.CanBeRemoved) == true)
                {
                    bool needEnd = false;
                    if (sb.Length > 0)
                    {
                        if (day.NeededDishTypes?.Count > 1)
                        {
                            sb.Insert(0, '(');
                            sb.Append(')');
                        }

                        sb.Append(" and ");

                        if (day.NeededMainIngredients.Count > 1)
                        {
                            sb.Append('(');
                            needEnd = true;
                        }
                    }

                    foreach (TagEdit mainIngredient in day.NeededMainIngredients)
                    {
                        sb.Append(Consts.TagSymbol)
                          .Append('\"')
                          .Append(mainIngredient.Name)
                          .Append('\"');

                        if (mainIngredient != day.NeededMainIngredients.Last())
                        {
                            sb.Append(" or ");
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

        /// <summary>
        /// Gets or sets selected recipe.
        /// </summary>
        public RecipeListViewDto? SelectedRecipe { get; set; }

        /// <summary>
        /// Gets recipies collection to choose from.
        /// </summary>
        public CollectionViewSource? RecipiesSource { get; private set; }

        /// <summary>
        /// Gets or sets filter text value.
        /// </summary>
        public string? FilterText { get; set; }

        /// <inheritdoc/>
        protected override bool CanOk() => SelectedRecipe != null;

        /// <summary>
        /// Callback to be called when <see cref="FilterText"/> changed.
        /// Calling code injected by PropertyChanged.Fody.
        /// </summary>
#pragma warning disable IDE0051, RCS1213
        private void OnFilterTextChanged()
        {
            recipies.Clear();
            List<RecipeListViewDto> newEntries;

            if (!string.IsNullOrEmpty(FilterText))
            {
                Expression<Func<Recipe, bool>> filterExpression = recipeFiltrator.Instance.Value.GetExpression(FilterText);
                newEntries = recipeService.GetProjectedClientside<RecipeListViewDto>(filterExpression.Compile());
            }
            else
            {
                newEntries = recipeService.GetMapped<RecipeListViewDto>();
            }

            recipies.AddRange(newEntries);

            // CollectionViewSource must be updated on UI thread
            Application.Current.Dispatcher.Invoke(() => RecipiesSource?.View.Refresh());
        }
#pragma warning restore IDE0051, RCS1213
    }
}