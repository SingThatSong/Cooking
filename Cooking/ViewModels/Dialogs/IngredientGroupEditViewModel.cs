using Cooking.Commands;
using System.ComponentModel;

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
            string? backup = IngredientGroup.Name;
            IngredientGroup.Name = "123";
            IngredientGroup.Name = backup;
        }

        protected override bool CanOk()
        {
            if (IngredientGroup is INotifyDataErrorInfo dataErrorInfo)
            {
                return !dataErrorInfo.HasErrors;
            }
            else
            {
                return true;
            }
        }

        public DTO.IngredientGroupEdit IngredientGroup { get; private set; }
    }
}