using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Data;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// View model for recipe selection from a list of recipies.
    /// </summary>
    public partial class RecipeSelectViewModel : OkCancelViewModel
    {
        private readonly List<RecipeListViewDto> recipies;
        private readonly RecipeService recipeService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;
        private string? filterText;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeSelectViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="recipeService">Recipe service dependency.</param>
        /// <param name="mapper">Mapper dependency.</param>
        /// <param name="localization">Localization service dependency.</param>
        /// <param name="day">Day, which settings will be user for filtering.</param>
        public RecipeSelectViewModel(DialogService dialogService,
                                     RecipeService recipeService,
                                     IMapper mapper,
                                     ILocalization localization,
                                     DayPlan? day = null)
            : base(dialogService)
        {
            this.recipeService = recipeService;
            this.mapper = mapper;
            this.localization = localization;

            recipies = recipeService.GetAllProjected<RecipeListViewDto>(mapper);

            RecipiesSource = new CollectionViewSource() { Source = recipies };

            if (day != null)
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
                            sb.Append(")");
                        }

                        sb.Append(" and ");

                        if (day.NeededMainIngredients.Count > 1)
                        {
                            sb.Append("(");
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
        public CollectionViewSource RecipiesSource { get; }

        /// <summary>
        /// Gets caption for search help placeholder.
        /// </summary>
        public string? SearchHelpTextCaption => localization.GetLocalizedString("SearchHelpText", Consts.IngredientSymbol, Consts.TagSymbol);

        /// <summary>
        /// Gets or sets filter text value.
        /// </summary>
        public string? FilterText
        {
            get => filterText;
            set
            {
                if (filterText != value)
                {
                    filterText = value;

                    recipies.Clear();

                    Expression<Func<Recipe, bool>> filterExpression = RecipeFiltrator.Instance.Value.GetExpression(value);
                    List<RecipeListViewDto> newEntries = recipeService.GetProjected<RecipeListViewDto>(filterExpression, mapper);

                    recipies.AddRange(newEntries);

                    RecipiesSource.View.Refresh();
                }
            }
        }

        /// <inheritdoc/>
        protected override bool CanOk() => SelectedRecipe != null;
    }
}