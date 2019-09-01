using Cooking.Commands;
using Cooking.DTO;
using Cooking.ServiceLayer;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using ServiceLayer;
using System;

namespace Cooking.Pages.Recepies
{
    public partial class RecipeViewModel : DialogViewModel
    {
        public RecipeViewModel() : base() { }

        public RecipeViewModel(Guid recipeId) : base()
        {
            var recipeDb = RecipeService.GetRecipe<RecipeFull>(recipeId).Result;
            Recipe = MapperService.Mapper.Map<RecipeMain>(recipeDb);
        }
        
        public RecipeFull Recipe { get; set; }
    }
}