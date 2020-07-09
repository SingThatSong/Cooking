using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using System.ComponentModel;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for creating/editing ingredient group.
    /// </summary>
    public partial class IngredientGroupEditViewModel : OkCancelViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientGroupEditViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="ingredientGroup">Ingredient group for editin. Null means group creation.</param>
        public IngredientGroupEditViewModel(DialogService dialogService, IngredientGroupEdit? ingredientGroup = null)
            : base(dialogService)
        {
            IngredientGroup = ingredientGroup ?? new IngredientGroupEdit();
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        /// <summary>
        /// Gets ingredient group to edit.
        /// </summary>
        public IngredientGroupEdit IngredientGroup { get; private set; }

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public DelegateCommand LoadedCommand { get; }

        /// <inheritdoc/>
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

        // WARNING: this is a crunch
        // When we open ingredient creation dialog second+ time, validation cannot see Ingredient being a required property, but when we change it's value - everything is ok
        // There is no such behaviour when using navigation, so it seems it's something Mahapps-related
        private void OnLoaded()
        {
            string? backup = IngredientGroup.Name;
            IngredientGroup.Name = "123";
            IngredientGroup.Name = backup;
        }
    }
}