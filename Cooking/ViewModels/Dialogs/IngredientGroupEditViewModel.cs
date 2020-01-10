using Cooking.Commands;
using System;

namespace Cooking.Pages
{
    public partial class IngredientGroupEditViewModel : OkCancelViewModel
    {
        public DelegateCommand LoadedCommand { get; }
        public IngredientGroupEditViewModel(DialogService dialogService, DTO.IngredientGroupEdit? ingredientGroup = null) : base(dialogService)
        {
            IngredientGroup = ingredientGroup ?? new DTO.IngredientGroupEdit();
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        // WARNING: this is a crunch
        // When we open ingredient creation dialog second+ time, validation cannot see Ingredient being a required property, but when we change it's value - everything is ok
        // There is no such behaviour when using navigation, so it seems it's something Mahapps-related
        private void OnLoaded()
        {
            var backup = IngredientGroup;
            IngredientGroup = new DTO.IngredientGroupEdit();
            IngredientGroup = backup;
        }

        public DTO.IngredientGroupEdit IngredientGroup { get; private set; }
    }
}