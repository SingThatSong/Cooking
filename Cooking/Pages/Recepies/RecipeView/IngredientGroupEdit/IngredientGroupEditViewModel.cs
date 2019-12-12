namespace Cooking.Pages
{
    public partial class IngredientGroupEditViewModel : OkCancelViewModel
    {
        public IngredientGroupEditViewModel() : this(null) { }
        public IngredientGroupEditViewModel(DTO.IngredientGroupEdit? ingredientGroup = null)
        {
            IngredientGroup = ingredientGroup ?? new DTO.IngredientGroupEdit();
        }

        public DTO.IngredientGroupEdit IngredientGroup { get; }
    }
}