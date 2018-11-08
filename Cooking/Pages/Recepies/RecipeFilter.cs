using Cooking.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cooking.Pages.Recepies
{
    public class RecipeFilter
    {
        static char[] filterChars = { '~', '#' };


        public RecipeFilter()
        {

        }

        public RecipeFilter(string filter)
        {
            FilterText = filter;
        }

        private string filter;
        public string FilterText
        {
            get => filter;
            set
            {
                if (value == null)
                    return;

                filter = value;

                var filters = GetFilterIndexes(value);

                if (filters.Count > 0)
                {
                    StringBuilder sb = new StringBuilder(value);
                    for (int i = filters.Count - 1; i >= 0; i--)
                    {
                        if (filters[i] > 0 && char.IsWhiteSpace(sb[filters[i] - 1]))
                        {
                            sb.Remove(filters[i] - 1, 1);
                        }
                    }
                    var res = StringSplitKeepSeparator(sb.ToString());

                    RecipeName = res[0];

                    if (res.Count > 1)
                    {
                        tags = new List<string>();
                        ingredients = new List<string>();
                        for (int i = 1; i < res.Count; i++)
                        {
                            if (res[i].StartsWith("~"))
                            {
                                tags.AddRange(res[i].Substring(1).Split(',').Select(x => x.Trim()));
                            }

                            if (res[i].StartsWith("#"))
                            {
                                ingredients.AddRange(res[i].Substring(1).Split(',').Select(x => x.Trim()));
                            }
                        }
                    }
                }
                else
                {
                    RecipeName = value;
                }
            }
        }

        private List<int> GetFilterIndexes(string text)
        {
            var result = new List<int>();
            for (int i = 0; i < text.Length; i++)
            {
                if (filterChars.Contains(text[i]))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        private List<string> StringSplitKeepSeparator(string text)
        {
            var result = new List<string>();
            int sumIndex = 0;
            foreach (var charIndex in GetFilterIndexes(text))
            {
                var length = charIndex - sumIndex;
                result.Add(text.Substring(sumIndex, length));
                sumIndex += length;
            }

            result.Add(text.Substring(sumIndex));

            return result;
        }

        internal List<string> Artists;
        internal string RecipeName;
        internal List<string> tags;
        internal List<string> ingredients;


        public bool FilterRecipe(RecipeDTO recipe)
        {
            // Проверка по названию

            if (RecipeName != null && (recipe.Name?.IndexOf(RecipeName, StringComparison.OrdinalIgnoreCase) < 0))
            {
                return false;
            }

            if (tags != null && tags.Count > 0)
            {
                foreach (var filter in tags)
                {
                    if (!HasTag(recipe, filter))
                    {
                        return false;
                    }
                }
            }


            if (ingredients != null && ingredients.Count > 0)
            { 
                foreach (var filter in ingredients)
                {
                    if (!HasIngredient(recipe, filter))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool HasTag(RecipeDTO recipe, string category)
        {
            return recipe.Tags != null && recipe.Tags.Any(x => x.Name.ToUpperInvariant() == category.ToUpperInvariant());
        }

        private bool HasIngredient(RecipeDTO recipe, string category)
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
                foreach(var group in recipe.IngredientGroups)
                {
                    if (group.Ingredients.Any(x => x.Ingredient.Name.ToUpperInvariant() == category.ToUpperInvariant()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
