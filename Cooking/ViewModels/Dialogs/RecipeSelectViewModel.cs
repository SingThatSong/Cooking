﻿using AutoMapper;
using Cooking.DTO;
using Cooking.Pages.Dialogs;
using Cooking.ServiceLayer;
using Cooking.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WPFLocalizeExtension.Engine;

namespace Cooking.Pages
{
    public partial class RecipeSelectViewModel : OkCancelViewModel
    {
        public RecipeSelectDto? SelectedRecipe { get; set; }
        public Guid? SelectedRecipeID => SelectedRecipe?.ID;

        public string? SearchHelpText
        {
            get
            {
                var localizedText = localization.GetLocalizedString("SearchHelpText");
                if (localizedText != null)
                {
                    localizedText = string.Format(LocalizeDictionary.Instance.Culture, localizedText, Consts.IngredientSymbol, Consts.TagSymbol);
                }
                return localizedText;
            }
        }

        public RecipeSelectViewModel(DialogService dialogService, 
                                     RecipeService recipeService, 
                                     IMapper mapper, 
                                     RecipeFiltrator recipeFiltrator,
                                     ILocalization localization,
                                     DayPlan? day = null) : base(dialogService)
        {
            Debug.Assert(recipeService != null);
            Debug.Assert(mapper != null);
            Debug.Assert(recipeFiltrator != null);
            Debug.Assert(localization != null);

            this.recipeFiltrator = recipeFiltrator;
            this.localization = localization;

            _recipies = recipeService.GetProjected<RecipeSelectDto>(mapper);

            RecipiesSource = new CollectionViewSource() { Source = _recipies };
            RecipiesSource.Filter += RecipiesSource_Filter;

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

        protected override bool CanOk() => SelectedRecipe != null;

        private void RecipiesSource_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(filterText))
                return;

            if (e.Item is RecipeSelectDto recipe)
            {
                e.Accepted = recipeFiltrator.FilterObject(recipe);
            }
        }

        private string? filterText;
        public string? FilterText
        {
            get => filterText;
            set
            {
                if (filterText != value)
                {
                    filterText = value;
                    recipeFiltrator.OnFilterTextChanged(value);
                    RecipiesSource.View.Refresh();
                }
            }
        }

        private readonly List<RecipeSelectDto> _recipies;
        private readonly RecipeFiltrator recipeFiltrator;
        private readonly ILocalization localization;

        public CollectionViewSource RecipiesSource { get; }
    }
}