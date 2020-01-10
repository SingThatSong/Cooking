using AutoMapper;
using Cooking.Commands;
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

namespace Cooking.Pages
{
    public partial class RecipeSelectViewModel : OkCancelViewModel
    {

        public Guid SelectedRecipeID { get; set; }

        public RecipeSelectViewModel(DialogService dialogService, 
                                     RecipeService recipeService, 
                                     IMapper mapper, 
                                     RecipeFiltrator recipeFiltrator,
                                     DayPlan? day = null) : base(dialogService)
        {
            Debug.Assert(recipeService != null);
            Debug.Assert(mapper != null);
            Debug.Assert(recipeFiltrator != null);

            this.recipeFiltrator = recipeFiltrator;

            _recipies = recipeService.GetProjected<RecipeSelectDto>(mapper);

            RecipiesSource = new CollectionViewSource() { Source = _recipies };
            RecipiesSource.Filter += RecipiesSource_Filter;

            SelectRecipeCommand = new DelegateCommand<RecipeSelectDto>(recipe =>
            {
                foreach (var r in _recipies.Where(x => x.IsSelected))
                {
                    r.IsSelected = false;
                }

                recipe.IsSelected = true;
                SelectedRecipeID = recipe.ID;
            });

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

        public RecipeSelectDto? SelectedRecipe => _recipies.FirstOrDefault(x => x.IsSelected);

        public CollectionViewSource RecipiesSource { get; }

        public DelegateCommand<RecipeSelectDto> SelectRecipeCommand { get; }
    }
}