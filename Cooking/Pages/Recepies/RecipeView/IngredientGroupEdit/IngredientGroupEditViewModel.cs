namespace Cooking.Pages
{
    public partial class IngredientGroupEditViewModel : OkCancelViewModel
    {
        public IngredientGroupEditViewModel(DialogService dialogService, DTO.IngredientGroupEdit? ingredientGroup = null) : base(dialogService)
        {
            IngredientGroup = ingredientGroup ?? new DTO.IngredientGroupEdit();
        }

        public DTO.IngredientGroupEdit IngredientGroup { get; }
    }
}