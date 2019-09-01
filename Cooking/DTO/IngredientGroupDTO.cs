using Cooking.ServiceLayer;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class IngredientGroupMain : IngredientGroupData
    {
        public new ObservableCollection<RecipeIngredientMain> Ingredients { get; set; }
    }
}
